#define THROW_IF_NOT_OPTIMIZABLE

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Collections.Generic;

// ReSharper disable PossibleInvalidCastException
// ReSharper disable PossibleNullReferenceException
// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode

namespace Framework.Serialization
{
	/// <summary>
	/// Class which defines the writer for serialized data using the fast serialization optimization.
	/// A SerializationWriter instance is used to store values and objects in a byte array.
	/// <br/><br/>
	/// Once an instance is created, use the various methods to store the required data.
	/// ToArray() will return a byte[] containing all of the data required for deserialization.
	/// This can be stored in the SerializationInfo parameter in an ISerializable.GetObjectData() method.
	/// <para/>
	/// As an alternative to ToArray(), if you want to apply some post-processing to the serialized bytes, 
	/// such as compression, call UpdateHeader() first to ensure that the string and object token table 
	/// sizes are updated in the header, and then cast BaseStream to MemoryStream. You can then access the
	/// MemoryStream's internal buffer as follows:
	/// <para/>
	/// <example><code>
	/// writer.UpdateHeader();
	/// MemoryStream stream = (MemoryStream) writer.BaseStream;
	///	serializedData = MyCompressor.Compress(stream.GetBuffer(), (int) stream.Length);
	/// </code></example>
	/// </summary>
	public sealed class SerializationWriter: BinaryWriter
	{
		#region Statics and constants
		/// <summary>
		/// Default capacity for the underlying MemoryStream
		/// </summary>
		public static int DefaultCapacity = 1024;

		/// <summary>
		/// The Default setting for the OptimizeForSize property.
		/// </summary>
		public static bool DefaultOptimizeForSize = true;

		/// <summary>
		/// The Default setting for the PreserveDecimalScale property.
		/// </summary>
		public static bool DefaultPreserveDecimalScale;

		/// <summary>
		/// Holds a list of optional IFastSerializationTypeSurrogate instances which SerializationWriter and SerializationReader will use to serialize objects
		/// not directly supported. It is important to use the same list on both client and server ends to ensure
		/// that the same surrogated-types are supported.
		/// </summary>
		static readonly List<IFastSerializationTypeSurrogate> typeSurrogates = new List<IFastSerializationTypeSurrogate>();

		/// <summary>
		/// Section masks used for packing DateTime values
		/// </summary>
		internal static readonly BitVector32.Section DateYearMask = BitVector32.CreateSection(9999); //14 bits
		internal static readonly BitVector32.Section DateMonthMask = BitVector32.CreateSection(12, DateYearMask); // 4 bits
		internal static readonly BitVector32.Section DateDayMask = BitVector32.CreateSection(31, DateMonthMask); // 5 bits
		internal static readonly BitVector32.Section DateHasTimeOrKindMask = BitVector32.CreateSection(1, DateDayMask); // 1 bit  total= 3 bytes

		/// <summary>
		/// Section masks used for packing TimeSpan values
		/// </summary>
		internal static readonly BitVector32.Section IsNegativeSection = BitVector32.CreateSection(1); //1 bit
		internal static readonly BitVector32.Section HasDaysSection = BitVector32.CreateSection(1, IsNegativeSection); //1 bit
		internal static readonly BitVector32.Section HasTimeSection = BitVector32.CreateSection(1, HasDaysSection); //1 bit
		internal static readonly BitVector32.Section HasSecondsSection = BitVector32.CreateSection(1, HasTimeSection); //1 bit
		internal static readonly BitVector32.Section HasMillisecondsSection = BitVector32.CreateSection(1, HasSecondsSection); //1 bit
		internal static readonly BitVector32.Section HoursSection = BitVector32.CreateSection(23, HasMillisecondsSection); // 5 bits
		internal static readonly BitVector32.Section MinutesSection = BitVector32.CreateSection(59, HoursSection); // 6 bits  total = 2 bytes
		internal static readonly BitVector32.Section SecondsSection = BitVector32.CreateSection(59, MinutesSection); // 6 bits total = 3 bytes
		internal static readonly BitVector32.Section MillisecondsSection = BitVector32.CreateSection(1024, SecondsSection); // 10 bits - total 31 bits = 4 bytes

		/// <summary>
		/// Holds the highest Int16 that can be optimized into less than the normal 2 bytes
		/// </summary>
		public const short HighestOptimizable16BitValue = 127; // 0x7F

		/// <summary>
		/// Holds the highest Int32 that can be optimized into less than the normal 4 bytes
		/// </summary>
		public const int HighestOptimizable32BitValue = 2097151; // 0x001FFFFF

		/// <summary>
		/// Holds the highest Int64 that can be optimized into less than the normal 8 bytes
		/// </summary>
		public const long HighestOptimizable64BitValue = 562949953421311; // 0x0001FFFFFFFFFFFF

		// The short at which optimization fails because it takes more than 2 bytes
		internal const short OptimizationFailure16BitValue = 16384;

		// The int at which optimization fails because it takes more than 4 bytes
		internal const int OptimizationFailure32BitValue = 268435456; // 0x10000000

		// The long at which optimization fails because it takes more than 8 bytes
		internal const long OptimizationFailure64BitValue = 72057594037927936; // 0x0100000000000000

		// Marker to denote that all elements in a typed array are optimizable
		static readonly BitArray FullyOptimizableTypedArray = new BitArray(0);
		#endregion

		readonly UniqueStringList stringLookup;
		readonly Hashtable objectTokenLookup;
		readonly bool allowUpdateHeader;
		readonly int startPosition;
		bool optimizeForSize;
		bool preserveDecimalScale;

		#region Type Usage related code (Debug mode only)
#if DEBUG
		// type usage member in which counters are stored of all the known types emitted into the output stream. For debugging purposes.
		public int[] TypeUsage
		{
			get { return typeUsage; }
		} readonly int[] typeUsage = new int[256];

		// stores the number of bytes used for tokenized strings and objects
		public int TableBytes
		{
			get { return tableBytes; }
		} int tableBytes;
#endif

		/// <summary>
		/// Dumps the type usage.
		/// </summary>
		[Conditional("DEBUG")]
		public void DumpTypeUsage()
		{
			var sb = new StringBuilder("Type Usage Dump\r\n---------------\r\n");

			for(var i = 0; i < 256; i++)
			{
#if DEBUG
				if (typeUsage[i] != 0)
				{
					sb.AppendFormat("{0, 8:n0}: {1}\r\n", typeUsage[i], (SerializedType) i);
				}
#endif
			}

			Console.WriteLine(sb);
		}
		#endregion

		/// <summary>
		/// Creates a FastSerializer with the Default Capacity (1kb)
		/// </summary>
		public SerializationWriter(): this(new MemoryStream(DefaultCapacity)) 
		{
		}

		/// <summary>
		/// Creates a FastSerializer with the specified capacity
		/// </summary>
		/// <param name="capacity"></param>
		public SerializationWriter(int capacity): this(new MemoryStream(capacity)) 
		{
		}

		/// <summary>
		/// Creates a FastSerializer around the specified stream
		/// Will allow updating of header info if the stream is seekable
		/// </summary>
		/// <param name="stream"></param>
		public SerializationWriter(Stream stream): this(stream, true)
		{
		}

		/// <summary>
		/// Creates a FastSerializer around the specified stream
		/// Notes:
		/// If the stream is not seekable then the allowUpdateHeader parameter is ignored
		/// </summary>
		/// <param name="stream">The stream in which to store data</param>
		/// <param name="allowUpdateHeader">true if token table presize 
		/// information can be stored; false otherwise</param>
		public SerializationWriter(Stream stream, bool allowUpdateHeader): base(stream)
		{
			// Store the start position of the stream if seekable
			startPosition = stream.CanSeek ? (int) stream.Position : 0;

			// Stream must also be seekable for this field to be set to true
			this.allowUpdateHeader = allowUpdateHeader && stream.CanSeek;

			// Always write an Int32 placeholder;
			// it will store either the stream length or remain as 0 if allowUpdateHeader is false
			Write(0);

			if (this.allowUpdateHeader)
			{
				// Write additional placeholders for tokenized string count and tokenized object count
				Write(0);
				Write(0);
			}

			objectTokenLookup = new Hashtable();
			stringLookup = new UniqueStringList();
			optimizeForSize = DefaultOptimizeForSize;
			preserveDecimalScale = DefaultPreserveDecimalScale;
		}

		/// <summary>
		/// Writes an ArrayList into the stream using the fewest number of bytes possible.
		/// Stored Size: 1 byte upwards depending on data content
		/// Notes:
		/// A null Arraylist takes 1 byte.
		/// An empty ArrayList takes 2 bytes.
		/// The contents are stored using WriteOptimized(ArrayList) which should be used
		/// if the ArrayList is guaranteed never to be null.
		/// </summary>
		/// <param name="value">The ArrayList to store.</param>
		public void Write(ArrayList value)
		{
			if (value == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else
			{
				WriteTypeCode(SerializedType.ArrayListType);
				WriteOptimized(value);
			}
		}

		/// <summary>
		/// Writes a BitArray value into the stream using the fewest number of bytes possible.
		/// Stored Size: 1 byte upwards depending on data content
		/// Notes:
		/// A null BitArray takes 1 byte.
		/// An empty BitArray takes 2 bytes.
		/// </summary>
		/// <param name="value">The BitArray value to store.</param>
		public void Write(BitArray value)
		{
			if (value == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else
			{
				WriteTypeCode(SerializedType.BitArrayType);
				WriteOptimized(value);
			}
		}

		/// <summary>
		/// Writes a BitVector32 into the stream.
		/// Stored Size: 4 bytes.
		/// </summary>
		/// <param name="value">The BitVector32 to store.</param>
		public void Write(BitVector32 value)
		{
			Write(value.Data);
		}

		/// <summary>
		/// Writes a DateTime value into the stream.
		/// Stored Size: 8 bytes
		/// </summary>
		/// <param name="value">The DateTime value to store.</param>
		public void Write(DateTime value)
		{
			Write(value.ToBinary());
		}

		/// <summary>
		/// Writes a Guid into the stream.
		/// Stored Size: 16 bytes.
		/// </summary>
		/// <param name="value"></param>
		public void Write(Guid value)
		{
			base.Write(value.ToByteArray());
		}

		/// <summary>
		/// Allows any object implementing IOwnedDataSerializable to serialize itself
		/// into this SerializationWriter.
		/// A context may also be used to give the object an indication of what data
		/// to store. As an example, using a BitVector32 gives a list of flags and
		/// the object can conditionally store data depending on those flags.
		/// </summary>
		/// <param name="target">The IOwnedDataSerializable object to ask for owned data</param>
		/// <param name="context">An arbtritrary object but BitVector32 recommended</param>
		public void Write(IOwnedDataSerializable target, object context)
		{
			target.SerializeOwnedData(this, context);
		}

		/// <summary>
		/// Stores an object into the stream using the fewest number of bytes possible.
		/// Stored Size: 1 byte upwards depending on type and/or content.
		/// 
		/// 1 byte: null, DBNull.Value, Boolean
		/// 
		/// 1 to 2 bytes: Int16, UInt16, Byte, SByte, Char, 
		/// 
		/// 1 to 4 bytes: Int32, UInt32, Single, BitVector32
		/// 
		/// 1 to 8 bytes: DateTime, TimeSpan, Double, Int64, UInt64
		/// 
		/// 1 or 16 bytes: Guid
		/// 
		/// 1 plus content: string, object[], byte[], char[], BitArray, Type, ArrayList
		/// 
		/// Any other object be stored using a .Net Binary formatter but this should 
		/// only be allowed as a last resort:
		/// Since this is effectively a different serialization session, there is a 
		/// possibility of the same shared object being serialized twice or, if the 
		/// object has a reference directly or indirectly back to the parent object, 
		/// there is a risk of looping which will throw an exception.
		/// 
		/// The type of object is checked with the most common types being checked first.
		/// Each 'section' can be reordered to provide optimum speed but the check for
		/// null should always be first and the default serialization always last.
		/// 
		/// Once the type is identified, a SerializedType byte is stored in the stream
		/// followed by the data for the object (certain types/values may not require
		/// storage of data as the SerializedType may imply the value).
		/// 
		/// For certain objects, if the value is within a certain range then optimized
		/// storage may be used. If the value doesn't meet the required optimization
		/// criteria then the value is stored directly.
		/// The checks for optimization may be disabled by setting the OptimizeForSize
		/// property to false in which case the value is stored directly. This could 
		/// result in a slightly larger stream but there will be a speed increate to
		/// compensate.
		/// </summary>
		/// <param name="value">The object to store.</param>
		public void WriteObject(object value)
		{
			// The following routine uses a main if-else tree which is somewhat flattened. Every if/else branch simply
			// tests for the type of value. If a type match is found, the code uses a normal if/else tree. 

			if (value == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (value is string)
			{
				WriteOptimized((string) value);
			}
			else if (value is Int32)
			{
				var int32Value = (int) value;

				switch (int32Value)
				{
					case 0:
						WriteTypeCode(SerializedType.ZeroInt32Type);
						return;

					case -1:
						WriteTypeCode(SerializedType.MinusOneInt32Type);
						return;

					case 1:
						WriteTypeCode(SerializedType.OneInt32Type);
						return;

					default:
						if (optimizeForSize)
						{
							if (int32Value > 0)
							{
								if (int32Value <= HighestOptimizable32BitValue) 
								{
									WriteTypeCode(SerializedType.OptimizedInt32Type);
									Write7BitEncodedSigned32BitValue(int32Value);
									return;
								}
							}
							else
							{
								var positiveInt32Value = -(int32Value + 1);

								if (positiveInt32Value <= HighestOptimizable32BitValue)
								{
									WriteTypeCode(SerializedType.OptimizedInt32NegativeType);
									Write7BitEncodedSigned32BitValue(positiveInt32Value);
									return;
								}
							}
						}

						WriteTypeCode(SerializedType.Int32Type);
						Write(int32Value);
						return;
				}
			}
			else if (value == DBNull.Value)
			{
				WriteTypeCode(SerializedType.DBNullType);
			}
			else if (value is Boolean)
			{
				WriteTypeCode((bool) value ? SerializedType.BooleanTrueType : SerializedType.BooleanFalseType);
			}
			else if (value is Decimal)
			{
				var decimalValue = (Decimal) value;

				if (decimalValue == 0)
				{
					WriteTypeCode(SerializedType.ZeroDecimalType);
				}
				else if (decimalValue == 1)
				{
					WriteTypeCode(SerializedType.OneDecimalType);
				}
				else
				{
					WriteTypeCode(SerializedType.DecimalType);
					WriteOptimized(decimalValue);
				}
			}
			else if (value is DateTime)
			{
				var dateTimeValue = (DateTime) value;

				if (dateTimeValue == DateTime.MinValue)
				{
					WriteTypeCode(SerializedType.MinDateTimeType);
				}
				else if (dateTimeValue == DateTime.MaxValue)
				{
					WriteTypeCode(SerializedType.MaxDateTimeType);
				}
				else if (optimizeForSize && ((dateTimeValue.Ticks % TimeSpan.TicksPerMillisecond) == 0))
				{
					WriteTypeCode(SerializedType.OptimizedDateTimeType);
					WriteOptimized(dateTimeValue);
				}
				else
				{
					WriteTypeCode(SerializedType.DateTimeType);
					Write(dateTimeValue);
				}
			}
			else if (value is Double)
			{
				var doubleValue = (Double) value;

				if (doubleValue == 0)
				{
					WriteTypeCode(SerializedType.ZeroDoubleType);
				}
				else if (doubleValue == 1)
				{
					WriteTypeCode(SerializedType.OneDoubleType);
				}
				else
				{
					WriteTypeCode(SerializedType.DoubleType);
					Write(doubleValue);
				}
			}
			else if (value is Single)
			{
				var singleValue = (Single) value;

				if (singleValue == 0)
				{
					WriteTypeCode(SerializedType.ZeroSingleType);
				}
				else if (singleValue == 1)
				{
					WriteTypeCode(SerializedType.OneSingleType);
				}
				else
				{
					WriteTypeCode(SerializedType.SingleType);
					Write(singleValue);
				}
			}
			else if (value is Int16)
			{
				var int16Value = (Int16) value;

				switch (int16Value)
				{
					case 0:
						WriteTypeCode(SerializedType.ZeroInt16Type);
						return;

					case -1:
						WriteTypeCode(SerializedType.MinusOneInt16Type);
						return;

					case 1:
						WriteTypeCode(SerializedType.OneInt16Type);
						return;

					default:
						if (optimizeForSize)
						{
							if (int16Value > 0)
							{
								if (int16Value <= HighestOptimizable16BitValue) 
								{
									WriteTypeCode(SerializedType.OptimizedInt16Type);
									Write7BitEncodedSigned32BitValue(int16Value);
									return;
								}
							}
							else
							{
								var positiveInt16Value = (-(int16Value + 1));

								if (positiveInt16Value <= HighestOptimizable16BitValue) 
								{
									WriteTypeCode(SerializedType.OptimizedInt16NegativeType);
									Write7BitEncodedSigned32BitValue(positiveInt16Value);
									return;
								}
							}
						}

						WriteTypeCode(SerializedType.Int16Type);
						Write(int16Value);
						return;
				}
			}

			else if (value is Guid)
			{
				var guidValue = (Guid) value;

				if (guidValue == Guid.Empty)
				{
					WriteTypeCode(SerializedType.EmptyGuidType);
				}
				else
				{
					WriteTypeCode(SerializedType.GuidType);
					Write(guidValue);
				}

				return;
			}

			else if (value is Int64)
			{
				var int64Value = (Int64) value;

				switch (int64Value)
				{
					case 0:
						WriteTypeCode(SerializedType.ZeroInt64Type);
						return;

					case -1:
						WriteTypeCode(SerializedType.MinusOneInt64Type);
						return;

					case 1:
						WriteTypeCode(SerializedType.OneInt64Type);
						return;

					default:
						if (optimizeForSize)
						{
							if (int64Value > 0)
							{
								if (int64Value <= HighestOptimizable64BitValue)
								{
									WriteTypeCode(SerializedType.OptimizedInt64Type);
									Write7BitEncodedSigned64BitValue(int64Value);
									return;
								}
							}
							else
							{
								var positiveInt64Value = -(int64Value + 1);

								if (positiveInt64Value <= HighestOptimizable64BitValue)
								{
									WriteTypeCode(SerializedType.OptimizedInt64NegativeType);
									Write7BitEncodedSigned64BitValue(positiveInt64Value);
									return;
								}
							}
						}

						WriteTypeCode(SerializedType.Int64Type);
						Write(int64Value);
						return;
				}
			}

			else if (value is Byte)
			{
				var byteValue = (Byte) value;

				switch (byteValue)
				{
					case 0:
						WriteTypeCode(SerializedType.ZeroByteType);
						return;

					case 1:
						WriteTypeCode(SerializedType.OneByteType);
						return;

					default:
						WriteTypeCode(SerializedType.ByteType);
						Write(byteValue);
						return;
				}
			}
			else if (value is Char)
			{
				var charValue = (Char) value;

				switch (charValue)
				{
					case (Char) 0:
						WriteTypeCode(SerializedType.ZeroCharType);
						return;

					case (Char) 1:
						WriteTypeCode(SerializedType.OneCharType);
						return;

					default:
						WriteTypeCode(SerializedType.CharType);
						Write(charValue);
						return;
				}
			}
			else if (value is SByte)
			{
				var sbyteValue = (SByte) value;

				switch (sbyteValue)
				{
					case 0:
						WriteTypeCode(SerializedType.ZeroSByteType);
						return;

					case 1:
						WriteTypeCode(SerializedType.OneSByteType);
						return;

					default:
						WriteTypeCode(SerializedType.SByteType);
						Write(sbyteValue);
						return;
				}
			}
			else if (value is UInt32)
			{
				var uint32Value = (UInt32) value;

				switch (uint32Value)
				{
					case 0:
						WriteTypeCode(SerializedType.ZeroUInt32Type);
						return;

					case 1:
						WriteTypeCode(SerializedType.OneUInt32Type);
						return;

					default:
						if (optimizeForSize && uint32Value <= HighestOptimizable32BitValue)
						{
							WriteTypeCode(SerializedType.OptimizedUInt32Type);
							Write7BitEncodedUnsigned32BitValue(uint32Value);
						}
						else
						{
							WriteTypeCode(SerializedType.UInt32Type);
							Write(uint32Value);
						}
						return;
				}
			}

			else if (value is UInt16)
			{
				var uint16Value = (UInt16) value;

				switch (uint16Value)
				{
					case 0:
						WriteTypeCode(SerializedType.ZeroUInt16Type);
						return;

					case 1:
						WriteTypeCode(SerializedType.OneUInt16Type);
						return;

					default:
						if (optimizeForSize && uint16Value <= HighestOptimizable16BitValue)
						{
							WriteTypeCode(SerializedType.OptimizedUInt16Type);
							Write7BitEncodedUnsigned32BitValue(uint16Value);
						}
						else
						{
							WriteTypeCode(SerializedType.UInt16Type);
							Write(uint16Value);
						}

						return;
				}
			}
			else if (value is UInt64)
			{
				var uint64Value = (UInt64) value;

				switch (uint64Value)
				{
					case 0:
						WriteTypeCode(SerializedType.ZeroUInt64Type);
						return;

					case 1:
						WriteTypeCode(SerializedType.OneUInt64Type);
						return;

					default:
						if (optimizeForSize && uint64Value <= HighestOptimizable64BitValue)
						{
							WriteTypeCode(SerializedType.OptimizedUInt64Type);
							WriteOptimized(uint64Value);
						}
						else
						{
							WriteTypeCode(SerializedType.UInt64Type);
							Write(uint64Value);
						}

						return;
				}
			}

			else if (value is TimeSpan)
			{
				var timeSpanValue = (TimeSpan) value;

				if (timeSpanValue == TimeSpan.Zero)
				{
					WriteTypeCode(SerializedType.ZeroTimeSpanType);
				}
				else if (optimizeForSize && (timeSpanValue.Ticks % TimeSpan.TicksPerMillisecond) == 0)
				{
					WriteTypeCode(SerializedType.OptimizedTimeSpanType);
					WriteOptimized(timeSpanValue);
				}
				else
				{
					WriteTypeCode(SerializedType.TimeSpanType);
					Write(timeSpanValue);
				}

				return;
			}

			else if (value is Array)
			{
				WriteTypedArray((Array) value, true);
			}

			else if (value is Type)
			{
				WriteTypeCode(SerializedType.TypeType);
				WriteOptimized((value as Type));
			}

			else if (value is BitArray)
			{
				WriteTypeCode(SerializedType.BitArrayType);
				WriteOptimized((BitArray) value);
			}

			else if (value is BitVector32)
			{
				WriteTypeCode(SerializedType.BitVector32Type);
				Write((BitVector32) value);
			}

			else if (IsTypeRecreatable(value.GetType()))
			{
				WriteTypeCode(SerializedType.OwnedDataSerializableAndRecreatableType);
				WriteOptimized(value.GetType());
				Write((IOwnedDataSerializable) value, null);
			}

			else if (value is SingletonTypeWrapper)
			{
				WriteTypeCode(SerializedType.SingleInstanceType);

				WriteStringDirect((value as SingletonTypeWrapper).WrappedType.AssemblyQualifiedName);
			}

			else if (value is ArrayList)
			{
				WriteTypeCode(SerializedType.ArrayListType);
				WriteOptimized((value as ArrayList));
			}

			else if (value is Enum) 
			{
				var enumType = value.GetType();
				var underlyingType = Enum.GetUnderlyingType(enumType);

				switch(Type.GetTypeCode(underlyingType))
				{
					case TypeCode.Int32:
					case TypeCode.UInt32:
						var uint32Value = underlyingType == typeof(int) ? (uint) (int) value : (uint) value;

						if (uint32Value <= HighestOptimizable32BitValue)
						{
							WriteTypeCode(SerializedType.OptimizedEnumType);
							WriteOptimized(enumType);
							Write7BitEncodedUnsigned32BitValue(uint32Value);
						} 
						else 
						{
							WriteTypeCode(SerializedType.EnumType);
							WriteOptimized(enumType);
							Write(uint32Value);
						}
						return;

					case TypeCode.Int64:
					case TypeCode.UInt64:
						var uint64Value = underlyingType == typeof(long) ? (ulong) (long) value : (ulong) value;

						if (uint64Value <= HighestOptimizable64BitValue)
						{
							WriteTypeCode(SerializedType.OptimizedEnumType);
							WriteOptimized(enumType);
							Write7BitEncodedUnsigned64BitValue(uint64Value);
						} 
						else 
						{
							WriteTypeCode(SerializedType.EnumType);
							WriteOptimized(enumType);
							Write(uint64Value);
						}
						return;

					case TypeCode.Byte:
						WriteTypeCode(SerializedType.EnumType);
						WriteOptimized(enumType);
						Write((byte) value);
						return;

					case TypeCode.SByte:
						WriteTypeCode(SerializedType.EnumType);
						WriteOptimized(enumType);
						Write((sbyte) value);
						return;

					case TypeCode.Int16:
						WriteTypeCode(SerializedType.EnumType);
						WriteOptimized(enumType);
						Write((short) value);
						return;

					default:
						WriteTypeCode(SerializedType.EnumType);
						WriteOptimized(enumType);
						Write((ushort) value);
						return;
				}
			}

			else
			{
				var valueType = value.GetType();
				var typeSurrogate = FindSurrogateForType(valueType);

				if (typeSurrogate != null)
				{
					WriteTypeCode(SerializedType.SurrogateHandledType);
					WriteOptimized(valueType);
					typeSurrogate.Serialize(this, value);
				}
				else
				{
					WriteTypeCode(SerializedType.OtherType);
					CreateBinaryFormatter().Serialize(BaseStream, value);
				}
			}
		}

		/// <summary>
		/// Calls WriteOptimized(string).
		/// This override to hide base BinaryWriter.Write(string).
		/// </summary>
		/// <param name="value">The string to store.</param>
		public override void Write(string value)
		{
			WriteOptimized(value);
		}

		/// <summary>
		/// Writes a TimeSpan value into the stream.
		/// Stored Size: 8 bytes
		/// </summary>
		/// <param name="value">The TimeSpan value to store.</param>
		public void Write(TimeSpan value)
		{
			Write(value.Ticks);
		}

		/// <summary>
		/// Stores a Type object into the stream.
		/// Stored Size: Depends on the length of the Type's name and whether the fullyQualified parameter is set.
		/// A null Type takes 1 byte.
		/// </summary>
		/// <param name="value">The Type to store.</param>
		/// <param name="fullyQualified">true to store the AssemblyQualifiedName or false to store the FullName. </param>
		public void Write(Type value, bool fullyQualified)
		{
			if (value == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else
			{
				WriteTypeCode(SerializedType.TypeType);
				WriteOptimized(fullyQualified ? value.AssemblyQualifiedName : value.FullName);
			}
		}

		/// <summary>
		/// Writes an non-null ArrayList into the stream using the fewest number of bytes possible.
		/// Stored Size: 1 byte upwards depending on data content
		/// Notes:
		/// An empty ArrayList takes 1 byte.
		/// </summary>
		/// <param name="value">The ArrayList to store. Must not be null.</param>
		public void WriteOptimized(ArrayList value)
		{
			CheckOptimizable(value != null, "Cannot optimize a null ArrayList");

			WriteObjectArray(value.ToArray());
		}

		/// <summary>
		/// Writes a BitArray into the stream using the fewest number of bytes possible.
		/// Stored Size: 1 byte upwards depending on data content
		/// Notes:
		/// An empty BitArray takes 1 byte.
		/// </summary>
		/// <param name="value">The BitArray value to store. Must not be null.</param>
		public void WriteOptimized(BitArray value)
		{
			CheckOptimizable(value != null, "Cannot optimize a null BitArray");

			Write7BitEncodedSigned32BitValue(value.Length);

			if (value.Length > 0)
			{
				var data = new byte[(value.Length + 7) / 8];
				value.CopyTo(data, 0);
				Write(data, 0, data.Length);
			}
		}

		/// <summary>
		/// Writes a BitVector32 into the stream using the fewest number of bytes possible.
		/// Stored Size: 1 to 4 bytes. (.Net is 4 bytes)
		///  1 to  7 bits takes 1 byte
		///  8 to 14 bits takes 2 bytes
		/// 15 to 21 bits takes 3 bytes
		/// 22 to 28 bits takes 4 bytes
		/// -------------------------------------------------------------------
		/// 29 to 32 bits takes 5 bytes - use Write(BitVector32) method instead
		/// 
		/// Try to order the BitVector32 masks so that the highest bits are least-likely
		/// to be set.
		/// </summary>
		/// <param name="value">The BitVector32 to store. Must not use more than 28 bits.</param>
		public void WriteOptimized(BitVector32 value)
		{
			CheckOptimizable(value.Data < OptimizationFailure32BitValue && value.Data >= 0, "BitVector32 value is not optimizable");
			Write7BitEncodedSigned32BitValue(value.Data);
		}

		/// <summary>
		/// Writes a DateTime value into the stream using the fewest number of bytes possible.
		/// Stored Size: 3 bytes to 7 bytes (.Net is 8 bytes)
		/// Notes:
		/// A DateTime containing only a date takes 3 bytes
		/// (except a .NET 2.0 Date with a specified DateTimeKind which will take a minimum
		/// of 5 bytes - no further optimization for this situation felt necessary since it
		/// is unlikely that a DateTimeKind would be specified without hh:mm also)
		/// Date plus hh:mm takes 5 bytes.
		/// Date plus hh:mm:ss takes 6 bytes.
		/// Date plus hh:mm:ss.fff takes 7 bytes.
		/// </summary>
		/// <param name="value">The DateTime value to store. Must not contain sub-millisecond data.</param>
		public void WriteOptimized(DateTime value)
		{
			CheckOptimizable((value.Ticks % TimeSpan.TicksPerMillisecond) == 0, "Cannot optimize a DateTime with sub-millisecond accuracy");

			var dateMask = new BitVector32();
			dateMask[DateYearMask] = value.Year;
			dateMask[DateMonthMask] = value.Month;
			dateMask[DateDayMask] = value.Day;

			var initialData = (int) value.Kind;
			var writeAdditionalData = value != value.Date;

			writeAdditionalData |= initialData != 0;
			dateMask[DateHasTimeOrKindMask] = writeAdditionalData ? 1 : 0;

			// Store 3 bytes of Date information
			var dateMaskData = dateMask.Data;
			Write((byte) dateMaskData);
			Write((byte) (dateMaskData >> 8));
			Write((byte) (dateMaskData >> 16));

			if (writeAdditionalData)
			{
				CheckOptimizable(((value.Ticks % TimeSpan.TicksPerMillisecond) == 0), "Cannot optimize a DateTime with sub-millisecond accuracy");
				EncodeTimeSpan(value.TimeOfDay, true, initialData);
			}
		}

		/// <summary>
		/// Writes a Decimal value into the stream using the fewest number of bytes possible.
		/// Stored Size: 1 byte to 14 bytes (.Net is 16 bytes)
		/// Restrictions: None
		/// </summary>
		/// <param name="value">The Decimal value to store</param>
		public void WriteOptimized(Decimal value)
		{
			var data = Decimal.GetBits(value);
			var scale = (byte) (data[3] >> 16);
			byte flags = 0;

			if (scale != 0 && !preserveDecimalScale && optimizeForSize)
			{
				var normalized = Decimal.Truncate(value);

				if (normalized == value)
				{
					data = Decimal.GetBits(normalized);
					scale = 0;
				}
			}

			if ((data[3] & -2147483648) != 0)
			{
				flags |= 0x01;
			}

			if (scale != 0)
			{
				flags |= 0x02;
			}

			if (data[0] == 0)
			{
				flags |= 0x04;
			}
			else if (data[0] <= HighestOptimizable32BitValue && data[0] >= 0)
			{
				flags |= 0x20;
			}

			if (data[1] == 0)
			{
				flags |= 0x08;
			}
			else if (data[1] <= HighestOptimizable32BitValue && data[1] >= 0)
			{
				flags |= 0x40;
			}

			if (data[2] == 0)
			{
				flags |= 0x10;
			}
			else if (data[2] <= HighestOptimizable32BitValue && data[2] >= 0)
			{
				flags |= 0x80;
			}

			Write(flags);

			if (scale != 0)
			{
				Write(scale);
			}

			if ((flags & 0x04) == 0)
			{
				if ((flags & 0x20) != 0)
				{
					Write7BitEncodedSigned32BitValue(data[0]);
				}
				else
				{
					Write(data[0]);
				}
			}

			if ((flags & 0x08) == 0)
			{
				if ((flags & 0x40) != 0)
				{
					Write7BitEncodedSigned32BitValue(data[1]);
				}
				else
				{
					Write(data[1]);
				}
			}

			if ((flags & 0x10) == 0)
			{
				if ((flags & 0x80) != 0)
				{
					Write7BitEncodedSigned32BitValue(data[2]);
				}
				else
				{
					Write(data[2]);
				}
			}
		}

		/// <summary>
		/// Write an Int16 value using the fewest number of bytes possible.
		/// </summary>
		/// <remarks>
		/// 0x0000 - 0x007f (0 to 127) takes 1 byte
		/// 0x0080 - 0x03FF (128 to 16,383) takes 2 bytes
		/// ----------------------------------------------------------------
		/// 0x0400 - 0x7FFF (16,384 to 32,767) takes 3 bytes
		/// All negative numbers take 3 bytes
		/// 
		/// Only call this method if the value is known to be between 0 and 
		/// 16,383 otherwise use Write(Int16 value)
		/// </remarks>
		/// <param name="value">The Int16 to store. Must be between 0 and 16,383 inclusive.</param>
		public void WriteOptimized(short value) 
		{
			CheckOptimizable(value < OptimizationFailure16BitValue && value >= 0, "Int16 value is not optimizable");

			Write7BitEncodedSigned32BitValue(value);
		}

		/// <summary>
		/// Write an Int32 value using the fewest number of bytes possible.
		/// </summary>
		/// <remarks>
		/// 0x00000000 - 0x0000007f (0 to 127) takes 1 byte
		/// 0x00000080 - 0x000003FF (128 to 16,383) takes 2 bytes
		/// 0x00000400 - 0x001FFFFF (16,384 to 2,097,151) takes 3 bytes
		/// 0x00200000 - 0x0FFFFFFF (2,097,152 to 268,435,455) takes 4 bytes
		/// ----------------------------------------------------------------
		/// 0x10000000 - 0x07FFFFFF (268,435,456 and above) takes 5 bytes
		/// All negative numbers take 5 bytes
		/// 
		/// Only call this method if the value is known to be between 0 and 
		/// 268,435,455 otherwise use Write(Int32 value)
		/// </remarks>
		/// <param name="value">The Int32 to store. Must be between 0 and 268,435,455 inclusive.</param>
		public void WriteOptimized(int value)
		{
			CheckOptimizable(value < OptimizationFailure32BitValue && value >= 0, "Int32 value is not optimizable");

			Write7BitEncodedSigned32BitValue(value);
		}

		/// <summary>
		/// Write an Int64 value using the fewest number of bytes possible.
		/// </summary>
		/// <remarks>
		/// 0x0000000000000000 - 0x000000000000007f (0 to 127) takes 1 byte
		/// 0x0000000000000080 - 0x00000000000003FF (128 to 16,383) takes 2 bytes
		/// 0x0000000000000400 - 0x00000000001FFFFF (16,384 to 2,097,151) takes 3 bytes
		/// 0x0000000000200000 - 0x000000000FFFFFFF (2,097,152 to 268,435,455) takes 4 bytes
		/// 0x0000000010000000 - 0x00000007FFFFFFFF (268,435,456 to 34,359,738,367) takes 5 bytes
		/// 0x0000000800000000 - 0x000003FFFFFFFFFF (34,359,738,368 to 4,398,046,511,103) takes 6 bytes
		/// 0x0000040000000000 - 0x0001FFFFFFFFFFFF (4,398,046,511,104 to 562,949,953,421,311) takes 7 bytes
		/// 0x0002000000000000 - 0x00FFFFFFFFFFFFFF (562,949,953,421,312 to 72,057,594,037,927,935) takes 8 bytes
		/// ------------------------------------------------------------------
		/// 0x0100000000000000 - 0x7FFFFFFFFFFFFFFF (72,057,594,037,927,936 to 9,223,372,036,854,775,807) takes 9 bytes
		/// 0x7FFFFFFFFFFFFFFF - 0xFFFFFFFFFFFFFFFF (9,223,372,036,854,775,807 and above) takes 10 bytes
		/// All negative numbers take 10 bytes
		/// 
		/// Only call this method if the value is known to be between 0 and
		/// 72,057,594,037,927,935 otherwise use Write(Int64 value)
		/// </remarks>
		/// <param name="value">The Int64 to store. Must be between 0 and 72,057,594,037,927,935 inclusive.</param>
		public void WriteOptimized(long value)
		{
			CheckOptimizable(value < OptimizationFailure64BitValue && value >= 0, "long value is not optimizable");

			Write7BitEncodedSigned64BitValue(value);
		}

		/// <summary>
		/// Writes a string value into the stream using the fewest number of bytes possible.
		/// Stored Size: 1 byte upwards depending on string length
		/// Notes:
		/// Encodes null, Empty, 'Y', 'N', ' ' values as a single byte
		/// Any other single char string is stored as two bytes
		/// All other strings are stored in a string token list:
		/// 
		/// The TypeCode representing the current string token list is written first (1 byte), 
		/// followed by the string token itself (1-4 bytes)
		/// 
		/// When the current string list has reached 128 values then a new string list
		/// is generated and that is used for generating future string tokens. This continues
		/// until the maximum number (128) of string lists is in use, after which the string 
		/// lists are used in a round-robin fashion.
		/// By doing this, more lists are created with fewer items which allows a smaller 
		/// token size to be used for more strings.
		/// 
		/// The first 16,384 strings will use a 1 byte token.
		/// The next 2,097,152 strings will use a 2 byte token. (This should suffice for most uses!)
		/// The next 268,435,456 strings will use a 3 byte token. (My, that is a lot!!)
		/// The next 34,359,738,368 strings will use a 4 byte token. (only shown for completeness!!!)
		/// </summary>
		/// <param name="value">The string to store.</param>
		public void WriteOptimized(string value)
		{
			if (value == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else
			{
				if (value.Length == 1)
				{
					var singleChar = value[0];

					switch (singleChar)
					{
						case 'Y':
							WriteTypeCode(SerializedType.YStringType);
							return;

						case 'N':
							WriteTypeCode(SerializedType.NStringType);
							return;

						case ' ':
							WriteTypeCode(SerializedType.SingleSpaceType);
							return;

						default:
							WriteTypeCode(SerializedType.SingleCharStringType);
							Write(singleChar);
							return;
					}
				}
				
				if (value.Length == 0)
				{
					WriteTypeCode(SerializedType.EmptyStringType);
					return;
				}

				int stringIndex;
				var isNew = stringLookup.Add(value, out stringIndex);

				Write((byte) (stringIndex % 128));
				Write7BitEncodedSigned32BitValue(stringIndex >> 7);

				if (isNew)
				{
#if DEBUG
					var currentPosition = OutStream.CanSeek ? OutStream.Position : 0;
#endif
					base.Write(value);
#if DEBUG
					if (OutStream.CanSeek)
					{
						tableBytes += (int) (OutStream.Position - currentPosition);
					}
#endif
				}
			}
		}

		/// <summary>
		/// Writes a TimeSpan value into the stream using the fewest number of bytes possible.
		/// Stored Size: 2 bytes to 8 bytes (.Net is 8 bytes)
		/// Notes:
		/// hh:mm (time) are always stored together and take 2 bytes.
		/// If seconds are present then 3 bytes unless (time) is not present in which case 2 bytes
		/// since the seconds are stored in the minutes position.
		/// If milliseconds are present then 4 bytes.
		/// In addition, if days are present they will add 1 to 4 bytes to the above.
		/// </summary>
		/// <param name="value">The TimeSpan value to store. Must not contain sub-millisecond data.</param>
		public void WriteOptimized(TimeSpan value)
		{
			CheckOptimizable(((value.Ticks % TimeSpan.TicksPerMillisecond) == 0), "Cannot optimize a TimeSpan with sub-millisecond accuracy");

			EncodeTimeSpan(value, false, 0);
		}
		/// <summary>
		/// Stores a non-null Type object into the stream.
		/// Stored Size: Depends on the length of the Type's name.
		/// If the type is a System type (mscorlib) then it is stored without assembly name information,
		/// otherwise the Type's AssemblyQualifiedName is used.
		/// </summary>
		/// <param name="value">The Type to store. Must not be null.</param>
		public void WriteOptimized(Type value)
		{
			CheckOptimizable((value != null), "Cannot optimize a null Type");

			WriteOptimized(value.AssemblyQualifiedName.IndexOf(", mscorlib,") == -1 ? value.AssemblyQualifiedName : value.FullName);
		}

		/// <summary>
		/// Write a UInt16 value using the fewest number of bytes possible.
		/// </summary>
		/// <remarks>
		/// 0x0000 - 0x007f (0 to 127) takes 1 byte
		/// 0x0080 - 0x03FF (128 to 16,383) takes 2 bytes
		/// ----------------------------------------------------------------
		/// 0x0400 - 0xFFFF (16,384 to 65,536) takes 3 bytes
		/// 
		/// Only call this method if the value is known to  be between 0 and 
		/// 16,383 otherwise use Write(UInt16 value)
		/// </remarks>
		/// <param name="value">The UInt16 to store. Must be between 0 and 16,383 inclusive.</param>
		[CLSCompliant(false)]
		public void WriteOptimized(ushort value)
		{
			CheckOptimizable(value < OptimizationFailure16BitValue, "UInt16 value is not optimizable");

			Write7BitEncodedUnsigned32BitValue(value);
		}

		/// <summary>
		/// Write a UInt32 value using the fewest number of bytes possible.
		/// </summary>
		/// <remarks>
		/// 0x00000000 - 0x0000007f (0 to 127) takes 1 byte
		/// 0x00000080 - 0x000003FF (128 to 16,383) takes 2 bytes
		/// 0x00000400 - 0x001FFFFF (16,384 to 2,097,151) takes 3 bytes
		/// 0x00200000 - 0x0FFFFFFF (2,097,152 to 268,435,455) takes 4 bytes
		/// ----------------------------------------------------------------
		/// 0x10000000 - 0xFFFFFFFF (268,435,456 and above) takes 5 bytes
		/// 
		/// Only call this method if the value is known to  be between 0 and 
		/// 268,435,455 otherwise use Write(UInt32 value)
		/// </remarks>
		/// <param name="value">The UInt32 to store. Must be between 0 and 268,435,455 inclusive.</param>
		[CLSCompliant(false)]
		public void WriteOptimized(uint value)
		{
			CheckOptimizable(value < OptimizationFailure32BitValue, "UInt32 value is not optimizable");

			Write7BitEncodedUnsigned32BitValue(value);
		}

		/// <summary>
		/// Write a UInt64 value using the fewest number of bytes possible.
		/// </summary>
		/// <remarks>
		/// 0x0000000000000000 - 0x000000000000007f (0 to 127) takes 1 byte
		/// 0x0000000000000080 - 0x00000000000003FF (128 to 16,383) takes 2 bytes
		/// 0x0000000000000400 - 0x00000000001FFFFF (16,384 to 2,097,151) takes 3 bytes
		/// 0x0000000000200000 - 0x000000000FFFFFFF (2,097,152 to 268,435,455) takes 4 bytes
		/// 0x0000000010000000 - 0x00000007FFFFFFFF (268,435,456 to 34,359,738,367) takes 5 bytes
		/// 0x0000000800000000 - 0x000003FFFFFFFFFF (34,359,738,368 to 4,398,046,511,103) takes 6 bytes
		/// 0x0000040000000000 - 0x0001FFFFFFFFFFFF (4,398,046,511,104 to 562,949,953,421,311) takes 7 bytes
		/// 0x0002000000000000 - 0x00FFFFFFFFFFFFFF (562,949,953,421,312 to 72,057,594,037,927,935) takes 8 bytes
		/// ------------------------------------------------------------------
		/// 0x0100000000000000 - 0x7FFFFFFFFFFFFFFF (72,057,594,037,927,936 to 9,223,372,036,854,775,807) takes 9 bytes
		/// 0x7FFFFFFFFFFFFFFF - 0xFFFFFFFFFFFFFFFF (9,223,372,036,854,775,807 and above) takes 10 bytes
		/// 
		/// Only call this method if the value is known to be between 0 and
		/// 72,057,594,037,927,935 otherwise use Write(UInt64 value)
		/// </remarks>
		/// <param name="value">The UInt64 to store. Must be between 0 and 72,057,594,037,927,935 inclusive.</param>
		[CLSCompliant(false)]
		public void WriteOptimized(ulong value)
		{
			CheckOptimizable(value < OptimizationFailure64BitValue, "ulong value is not optimizable");

			Write7BitEncodedUnsigned64BitValue(value);
		}

		/// <summary>
		/// Writes a Boolean[] into the stream.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// Calls WriteOptimized(Boolean[]).
		/// </summary>
		/// <param name="values">The Boolean[] to store.</param>
		public void Write(bool[] values)
		{
			WriteOptimized(values);
		}

		/// <summary>
		/// Writes a Byte[] into the stream.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The Byte[] to store.</param>
		public override void Write(byte[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				WriteTypeCode(SerializedType.NonOptimizedTypedArrayType);
				WriteArray(values);
			}
		}

		/// <summary>
		/// Writes a Char[] into the stream.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The Char[] to store.</param>
		public override void Write(char[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				WriteTypeCode(SerializedType.NonOptimizedTypedArrayType);
				WriteArray(values);
			}
		}

		/// <summary>
		/// Writes a DateTime[] into the stream.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The DateTime[] to store.</param>
		public void Write(DateTime[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				WriteArray(values, null);
			}
		}

		/// <summary>
		/// Writes a Decimal[] into the stream.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// Calls WriteOptimized(Decimal[]).
		/// </summary>
		/// <param name="values">The Decimal[] to store.</param>
		public void Write(decimal[] values)
		{
			WriteOptimized(values);
		}

		/// <summary>
		/// Writes a Double[] into the stream.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The Double[] to store.</param>
		public void Write(double[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				WriteTypeCode(SerializedType.NonOptimizedTypedArrayType);
				WriteArray(values);
			}
		}

		/// <summary>
		/// Writes a Single[] into the stream.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The Single[] to store.</param>
		public void Write(float[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				WriteTypeCode(SerializedType.NonOptimizedTypedArrayType);
				WriteArray(values);
			}
		}

		/// <summary>
		/// Writes a Guid[] into the stream.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The Guid[] to store.</param>
		public void Write(Guid[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				WriteTypeCode(SerializedType.NonOptimizedTypedArrayType);
				WriteArray(values);
			}
		}
		/// <summary>
		/// Writes an Int32[] into the stream.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The Int32[] to store.</param>
		public void Write(int[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				WriteArray(values, null);
			}
		}

		/// <summary>
		/// Writes an Int64[] into the stream.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The Int64[] to store.</param>
		public void Write(long[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				WriteArray(values, null);
			}
		}

		/// <summary>
		/// Writes an object[] into the stream.
		/// Stored Size: 2 bytes upwards depending on data content
		/// Notes:
		/// A null object[] takes 1 byte.
		/// An empty object[] takes 2 bytes.
		/// The contents of the array will be stored optimized.
		/// </summary>
		/// <param name="values">The object[] to store.</param>
		public void Write(object[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyObjectArrayType);
			}
			else
			{
				WriteTypeCode(SerializedType.ObjectArrayType);
				WriteObjectArray(values);
			}
		}

		/// <summary>
		/// Writes an SByte[] into the stream.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The SByte[] to store.</param>
		[CLSCompliant(false)]
		public void Write(sbyte[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				WriteTypeCode(SerializedType.NonOptimizedTypedArrayType);
				WriteArray(values);
			}
		}
		/// <summary>
		/// Writes an Int16[]or a null into the stream.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// Calls WriteOptimized(decimal[]).
		/// </summary>
		/// <param name="values">The Int16[] to store.</param>
		public void Write(short[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				WriteTypeCode(SerializedType.NonOptimizedTypedArrayType);
				WriteArray(values);
			}
		}

		/// <summary>
		/// Writes a TimeSpan[] into the stream.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The TimeSpan[] to store.</param>
		public void Write(TimeSpan[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				WriteArray(values, null);
			}
		}

		/// <summary>
		/// Writes a UInt32[] into the stream.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The UInt32[] to store.</param>
		[CLSCompliant(false)]
		public void Write(uint[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				WriteArray(values, null);
			}
		}
		/// <summary>
		/// Writes a UInt64[] into the stream.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The UInt64[] to store.</param>
		[CLSCompliant(false)]
		public void Write(ulong[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				WriteArray(values, null);
			}
		}

		/// <summary>
		/// Writes a UInt16[] into the stream.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The UInt16[] to store.</param>
		[CLSCompliant(false)]
		public void Write(ushort[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				WriteTypeCode(SerializedType.NonOptimizedTypedArrayType);
				WriteArray(values);
			}
		}

		/// <summary>
		/// Writes an optimized Boolean[] into the stream using the fewest possible bytes.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// Stored as a BitArray.
		/// </summary>
		/// <param name="values">The Boolean[] to store.</param>
		public void WriteOptimized(bool[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				WriteTypeCode(SerializedType.FullyOptimizedTypedArrayType);
				WriteArray(values);
			}
		}

		/// <summary>
		/// Writes a DateTime[] into the stream using the fewest possible bytes.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The DateTime[] to store.</param>
		public void WriteOptimized(DateTime[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				BitArray optimizeFlags = null;
				var notOptimizable = 0;
				var notWorthOptimizingLimit = 1 + (int) (values.Length * (optimizeForSize ? 0.8f : 0.6f));

				for (var i = 0; (i < values.Length) && (notOptimizable < notWorthOptimizingLimit); i++)
				{
					if (values[i].Ticks % TimeSpan.TicksPerMillisecond != 0)
					{
						notOptimizable++;
					}
					else
					{
						if (optimizeFlags == null)
						{
							optimizeFlags = new BitArray(values.Length);
						}

						optimizeFlags[i] = true;
					}
				}

				if (notOptimizable == 0)
				{
					optimizeFlags = FullyOptimizableTypedArray;
				}
				else if (notOptimizable >= notWorthOptimizingLimit)
				{
					optimizeFlags = null;
				}

				WriteArray(values, optimizeFlags);
			}
		}

		/// <summary>
		/// Writes a Decimal[] into the stream using the fewest possible bytes.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The Decimal[] to store.</param>
		public void WriteOptimized(decimal[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				WriteTypeCode(SerializedType.FullyOptimizedTypedArrayType);
				WriteArray(values);
			}
		}

		/// <summary>
		/// Writes a not-null object[] into the stream using the fewest number of bytes possible.
		/// Stored Size: 2 bytes upwards depending on data content
		/// Notes:
		/// An empty object[] takes 1 byte.
		/// The contents of the array will be stored optimized.
		/// </summary>
		/// <param name="values">The object[] to store. Must not be null.</param>
		public void WriteOptimized(object[] values)
		{
			CheckOptimizable(values != null, "Cannot optimize a null object[]");

			WriteObjectArray(values);
		}

		
		/// <summary>
		/// Writes a pair of object[] arrays into the stream using the fewest number of bytes possible.
		/// The arrays must not be null and must have the same length
		/// The first array's values are written optimized
		/// The second array's values are compared against the first and, where identical, will be stored
		/// using a single byte.
		/// Useful for storing entity data where there is a before-change and after-change set of value pairs
		/// and, typically, only a few of the values will have changed.
		/// </summary>
		/// <param name="values1">The first object[] value which must not be null and must have the same length as values2</param>
		/// <param name="values2">The second object[] value which must not be null and must have the same length as values1</param>
		public void WriteOptimized(object[] values1, object[] values2)
		{
			CheckOptimizable(((values1 != null) && (values2 != null)), "Cannot optimimize an object[] pair that is null");
			CheckOptimizable((values1.Length == values2.Length), "Cannot optimize an object[] pair with different lengths");

			WriteObjectArray(values1);

			var lastIndex = values2.Length - 1;

			for (var i = 0; i < values2.Length; i++)
			{
				var value2 = values2[i];
				
				// if value2 is null, we've to check if values1 is null, otherwise we've to compare value2 with the value at the same spot in values1
				if (value2 == null ? values1[i] == null : value2.Equals(values1[i]))
				{
					var duplicates = 0;
					for (;
						i < lastIndex && (values2[i + 1] == null ? values1[i + 1] == null : values2[i + 1].Equals(values1[i + 1]));
						i++)
					{
						duplicates++;
					}

					if (duplicates == 0)
					{
						WriteTypeCode(SerializedType.DuplicateValueType);
					}
					else
					{
						WriteTypeCode(SerializedType.DuplicateValueSequenceType);
						Write7BitEncodedSigned32BitValue(duplicates);
					}
				}
				else if (value2 == null)
				{
					var duplicates = 0;

					for (; i < lastIndex && values2[i + 1] == null; i++)
					{
						duplicates++;
					}

					if (duplicates == 0)
					{
						WriteTypeCode(SerializedType.NullType);
					}
					else
					{
						WriteTypeCode(SerializedType.NullSequenceType);
						Write7BitEncodedSigned32BitValue(duplicates);
					}
				}
				else
				{
					if (value2 == DBNull.Value)
					{
						var duplicates = 0;

						for (; i < lastIndex && values2[i + 1] == DBNull.Value; i++)
						{
							duplicates++;
						}

						if (duplicates == 0)
						{
							WriteTypeCode(SerializedType.DBNullType);
						}
						else
						{
							WriteTypeCode(SerializedType.DBNullSequenceType);
							Write7BitEncodedSigned32BitValue(duplicates);
						}
					}
					else
					{
						WriteObject(value2);
					}
				}
			}
		}

		/// <summary>
		/// Writes an Int16[] into the stream using the fewest possible bytes.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The Int16[] to store.</param>
		public void WriteOptimized(short[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				BitArray optimizeFlags = null;
				var notOptimizable = 0;
				var notWorthOptimizingLimit = 1 + (int) (values.Length * (optimizeForSize ? 0.8f : 0.6f));

				for (var i = 0; i < values.Length && notOptimizable < notWorthOptimizingLimit; i++)
				{
					if (values[i] < 0 || values[i] > HighestOptimizable16BitValue)
					{
						notOptimizable++;
					}
					else
					{
						if (optimizeFlags == null)
						{
							optimizeFlags = new BitArray(values.Length);
						}

						optimizeFlags[i] = true;
					}
				}

				if (notOptimizable == 0)
				{
					optimizeFlags = FullyOptimizableTypedArray;
				}
				else if (notOptimizable >= notWorthOptimizingLimit)
				{
					optimizeFlags = null;
				}

				WriteArray(values, optimizeFlags);
			}
		}
		/// <summary>
		/// Writes an Int32[] into the stream using the fewest possible bytes.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The Int32[] to store.</param>
		public void WriteOptimized(int[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				BitArray optimizeFlags = null;
				var notOptimizable = 0;
				var notWorthOptimizingLimit = 1 + (int) (values.Length * (optimizeForSize ? 0.8f : 0.6f));

				for (var i = 0; i < values.Length && notOptimizable < notWorthOptimizingLimit; i++)
				{
					if (values[i] < 0 || values[i] > HighestOptimizable32BitValue)
					{
						notOptimizable++;
					}
					else
					{
						if (optimizeFlags == null)
						{
							optimizeFlags = new BitArray(values.Length);
						}

						optimizeFlags[i] = true;
					}
				}

				if (notOptimizable == 0)
				{
					optimizeFlags = FullyOptimizableTypedArray;
				}
				else if (notOptimizable >= notWorthOptimizingLimit)
				{
					optimizeFlags = null;
				}

				WriteArray(values, optimizeFlags);
			}
		}

		/// <summary>
		/// Writes an Int64[] into the stream using the fewest possible bytes.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The Int64[] to store.</param>
		public void WriteOptimized(long[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				BitArray optimizeFlags = null;
				var notOptimizable = 0;
				var notWorthOptimizingLimit = 1 + (int) (values.Length * (optimizeForSize ? 0.8f : 0.6f));

				for (var i = 0; i < values.Length && notOptimizable < notWorthOptimizingLimit; i++)
				{
					if (values[i] < 0 || values[i] > HighestOptimizable64BitValue)
					{
						notOptimizable++;
					}
					else
					{
						if (optimizeFlags == null)
						{
							optimizeFlags = new BitArray(values.Length);
						}

						optimizeFlags[i] = true;
					}
				}

				if (notOptimizable == 0)
				{
					optimizeFlags = FullyOptimizableTypedArray;
				}
				else if (notOptimizable >= notWorthOptimizingLimit)
				{
					optimizeFlags = null;
				}

				WriteArray(values, optimizeFlags);
			}
		}

		/// <summary>
		/// Writes a TimeSpan[] into the stream using the fewest possible bytes.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The TimeSpan[] to store.</param>
		public void WriteOptimized(TimeSpan[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				BitArray optimizeFlags = null;
				var notOptimizable = 0;
				var notWorthOptimizingLimit = 1 + (int) (values.Length * (optimizeForSize ? 0.8f : 0.6f));

				for (var i = 0; i < values.Length && notOptimizable < notWorthOptimizingLimit; i++)
				{
					if (values[i].Ticks % TimeSpan.TicksPerMillisecond != 0)
					{
						notOptimizable++;
					}
					else
					{
						if (optimizeFlags == null)
						{
							optimizeFlags = new BitArray(values.Length);
						}

						optimizeFlags[i] = true;
					}
				}

				if (notOptimizable == 0)
				{
					optimizeFlags = FullyOptimizableTypedArray;
				}
				else if (notOptimizable >= notWorthOptimizingLimit)
				{
					optimizeFlags = null;
				}

				WriteArray(values, optimizeFlags);
			}
		}

		/// <summary>
		/// Writes a UInt16[] into the stream using the fewest possible bytes.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The UInt16[] to store.</param>
		[CLSCompliant(false)]
		public void WriteOptimized(ushort[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				BitArray optimizeFlags = null;
				var notOptimizable = 0;
				var notWorthOptimizingLimit = 1 + (int) (values.Length * (optimizeForSize ? 0.8f : 0.6f));

				for (var i = 0; i < values.Length && notOptimizable < notWorthOptimizingLimit; i++)
				{
					if (values[i] > HighestOptimizable16BitValue)
					{
						notOptimizable++;
					}
					else
					{
						if (optimizeFlags == null)
						{
							optimizeFlags = new BitArray(values.Length);
						}

						optimizeFlags[i] = true;
					}
				}

				if (notOptimizable == 0)
				{
					optimizeFlags = FullyOptimizableTypedArray;
				}
				else if (notOptimizable >= notWorthOptimizingLimit)
				{
					optimizeFlags = null;
				}

				WriteArray(values, optimizeFlags);
			}
		}

		/// <summary>
		/// Writes a UInt32[] into the stream using the fewest possible bytes.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The UInt32[] to store.</param>
		[CLSCompliant(false)]
		public void WriteOptimized(uint[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				BitArray optimizeFlags = null;
				var notOptimizable = 0;
				var notWorthOptimizingLimit = 1 + (int) (values.Length * (optimizeForSize ? 0.8f : 0.6f));

				for (var i = 0; i < values.Length && notOptimizable < notWorthOptimizingLimit; i++)
				{
					if (values[i] > HighestOptimizable32BitValue)
					{
						notOptimizable++;
					}
					else
					{
						if (optimizeFlags == null)
						{
							optimizeFlags = new BitArray(values.Length);
						}

						optimizeFlags[i] = true;
					}
				}

				if (notOptimizable == 0)
				{
					optimizeFlags = FullyOptimizableTypedArray;
				}
				else if (notOptimizable >= notWorthOptimizingLimit)
				{
					optimizeFlags = null;
				}

				WriteArray(values, optimizeFlags);
			}
		}

		/// <summary>
		/// Writes a UInt64[] into the stream using the fewest possible bytes.
		/// Notes:
		/// A null or empty array will take 1 byte.
		/// </summary>
		/// <param name="values">The UInt64[] to store.</param>
		[CLSCompliant(false)]
		public void WriteOptimized(ulong[] values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else if (values.Length == 0)
			{
				WriteTypeCode(SerializedType.EmptyTypedArrayType);
			}
			else
			{
				BitArray optimizeFlags = null;
				var notOptimizable = 0;
				var notWorthOptimizingLimit = 1 + (int) (values.Length * (optimizeForSize ? 0.8f : 0.6f));

				for (var i = 0; i < values.Length && notOptimizable < notWorthOptimizingLimit; i++)
				{
					if (values[i] > HighestOptimizable64BitValue)
					{
						notOptimizable++;
					}
					else
					{
						if (optimizeFlags == null)
						{
							optimizeFlags = new BitArray(values.Length);
						}

						optimizeFlags[i] = true;
					}
				}

				if (notOptimizable == 0)
				{
					optimizeFlags = FullyOptimizableTypedArray;
				}
				else if (notOptimizable >= notWorthOptimizingLimit)
				{
					optimizeFlags = null;
				}

				WriteArray(values, optimizeFlags);
			}
		}

		
		/// <summary>
		/// Writes a Nullable type into the stream.
		/// Synonym for WriteObject().
		/// </summary>
		/// <param name="value">The Nullable value to store.</param>
		public void WriteNullable(ValueType value)
		{
			WriteObject(value);
		}
		
		/// <summary>
		/// Writes a non-null generic Dictionary into the stream.
		/// </summary>
		/// <remarks>
		/// The key and value types themselves are not stored - they must be 
		/// supplied at deserialization time.
		/// <para/>
		/// An array of keys is stored followed by an array of values.
		/// </remarks>
		/// <typeparam name="K">The key Type.</typeparam>
		/// <typeparam name="V">The value Type.</typeparam>
		/// <param name="value">The generic dictionary.</param>
		public void Write<K, V>(Dictionary<K, V> value)
		{
			var keys = new K[value.Count];
			value.Keys.CopyTo(keys, 0);
			
			var values = new V[value.Count];
			value.Values.CopyTo(values, 0);
			
			WriteTypedArray(keys, false);
			WriteTypedArray(values, false);
		}
		

		/// <summary>
		/// Writes a non-null generic List into the stream.
		/// </summary>
		/// <remarks>
		/// The list type itself is not stored - it must be supplied
		/// at deserialization time.
		/// <para/>
		/// The list contents are stored as an array.
		/// </remarks>
		/// <typeparam name="T">The list Type.</typeparam>
		/// <param name="value">The generic List.</param>
		public void Write<T>(List<T> value)
		{
			WriteTypedArray(value.ToArray(), false);
		}

		/// <summary>
		/// Writes a null or a typed array into the stream.
		/// </summary>
		/// <param name="values">The array to store.</param>
		public void WriteTypedArray(Array values)
		{
			if (values == null)
			{
				WriteTypeCode(SerializedType.NullType);
			}
			else
			{
				WriteTypedArray(values, true);
			}
		}

		/// <summary>
		/// Updates the header to store
		/// 1) The total length of serialized data
		/// 2) The number of string tokens
		/// 3) The number of object tokens
		/// 
		/// Does nothing if the stream is not seekable or the constructor
		/// specified not to update the header.
		///
		/// Notes:
		/// Called automatically by ToArray() otherwise must be called
		/// manually when serialization is complete.
		/// </summary>
		/// <returns>The total length of serialized data or 0 if the stream is not seekable</returns>
		public int UpdateHeader()
		{
			var result = BaseStream.CanSeek ? (int) BaseStream.Position - startPosition : 0;

			// BaseStream.CanSeek is implied if allowUpdateHeader is true
			if (allowUpdateHeader)
			{
				var currentPosition = BaseStream.Position;

				BaseStream.Position = startPosition;

				Write(result);
				Write(stringLookup.Count);
				Write(objectTokenLookup.Count);

				BaseStream.Position = currentPosition;
			}

			return result;
		}

		/// <summary>
		/// Returns a byte[] containing all of the serialized data
		/// where the underlying stream is a MemoryStream.
		/// 
		/// Only call this method once all of the data has been serialized.
		/// 
		/// </summary>
		/// <returns>A byte[] containing all serialized data.</returns>
		public byte[] ToArray()
		{
			var memoryStream = BaseStream as MemoryStream;
			if (memoryStream == null) throw new InvalidOperationException("Cannot call ToArray() where the underlying stream is not a MemoryStream");

			UpdateHeader();

			if (startPosition == 0)
			{
				return memoryStream.ToArray();
			}
			
			var length = (int) memoryStream.Position - startPosition;
			var result = new byte[length];
			var currentPosition = BaseStream.Position;

			BaseStream.Position = startPosition;

			memoryStream.Read(result, 0, length);

			BaseStream.Position = currentPosition;

			return result;
		}

		/// <summary>
		/// Writes a byte[] directly into the stream.
		/// The size of the array is not stored so only use this method when
		/// the number of bytes will be known at deserialization time.
		/// 
		/// A null value will throw an exception
		/// </summary>
		/// <param name="value">The byte[] to store. Must not be null.</param>
		public void WriteBytesDirect(byte[] value)
		{
			base.Write(value);
		}

		/// <summary>
		/// Writes a non-null string directly to the stream without tokenization.
		/// </summary>
		/// <param name="value">The string to store. Must not be null.</param>
		public void WriteStringDirect(string value)
		{
			CheckOptimizable(value != null, "Cannot directly write a null string");

			base.Write(value);
		}

		/// <summary>
		/// Writes a token (an Int32 taking 1 to 4 bytes) into the stream that represents the object instance.
		/// The same token will always be used for the same object instance.
		/// 
		/// The object will be serialized once and recreated at deserialization time.
		/// Calls to SerializationReader.ReadTokenizedObject() will retrieve the same object instance.
		/// 
		/// </summary>
		/// <param name="value">The object to tokenize. Must not be null and must not be a string.</param>
		public void WriteTokenizedObject(object value)
		{
			WriteTokenizedObject(value, false);
		}

		/// <summary>
		/// Writes a token (an Int32 taking 1 to 4 bytes) into the stream that represents the object instance.
		/// The same token will always be used for the same object instance.
		/// 
		/// When recreateFromType is set to true, the object's Type will be stored and the object recreated using 
		/// Activator.GetInstance with a parameterless contructor. This is useful for stateless, factory-type classes.
		/// 
		/// When recreateFromType is set to false, the object will be serialized once and recreated at deserialization time.
		/// 
		/// Calls to SerializationReader.ReadTokenizedObject() will retrieve the same object instance.
		/// </summary>
		/// <param name="value">The object to tokenize. Must not be null and must not be a string.</param>
		/// <param name="recreateFromType">true if the object can be recreated using a parameterless constructor; 
		/// false if the object should be serialized as-is</param>
		public void WriteTokenizedObject(object value, bool recreateFromType)
		{
			CheckOptimizable(value != null, "Cannot write a null tokenized object");
			CheckOptimizable(!(value is string), "Use Write(string) instead of WriteTokenizedObject()");

			if (recreateFromType)
			{
				CheckOptimizable(HasEmptyConstructor(value.GetType()), "Cannot recreate type is it doesn't have a default/empty constructor");
				value = new SingletonTypeWrapper(value);
			}

			var token = objectTokenLookup[value];

			if (token != null)
			{
				Write7BitEncodedSigned32BitValue((int) token);
			}
			else
			{
				var newToken = objectTokenLookup.Count;
				objectTokenLookup[value] = newToken;

				Write7BitEncodedSigned32BitValue(newToken);
#if DEBUG
				var currentPosition = OutStream.Position;
#endif
				WriteObject(value);
#if DEBUG
				tableBytes += (int) (OutStream.Position - currentPosition);
#endif
			}
		}
		

		/// <summary>
		/// Finds the surrogate type for the type passed in.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		internal static IFastSerializationTypeSurrogate FindSurrogateForType(Type type) 
		{
			foreach(var surrogate in TypeSurrogates) 
			{
				if (surrogate.SupportsType(type)) return surrogate;
			}

			return null;
		}

		/// <summary>
		/// Creates the binary formatter.
		/// </summary>
		/// <returns></returns>
		static BinaryFormatter CreateBinaryFormatter()
		{
			return new BinaryFormatter
			       	{
			       		AssemblyFormat = FormatterAssemblyStyle.Full
			       	};
		}
		

		/// <summary>
		/// Encodes a TimeSpan into the fewest number of bytes.
		/// Has been separated from the WriteOptimized(TimeSpan) method so that WriteOptimized(DateTime)
		/// can also use this for .NET 2.0 DateTimeKind information.
		/// By taking advantage of the fact that a DateTime's TimeOfDay portion will never use the IsNegative
		/// and HasDays flags, we can use these 2 bits to store the DateTimeKind and, since DateTimeKind is
		/// unlikely to be set without a Time, we need no additional bytes to support a .NET 2.0 DateTime.
		/// </summary>
		/// <param name="value">The TimeSpan to store.</param>
		/// <param name="partOfDateTime">True if the TimeSpan is the TimeOfDay from a DateTime; False if a real TimeSpan.</param>
		/// <param name="initialData">The intial data for the BitVector32 - contains DateTimeKind or 0</param>
		void EncodeTimeSpan(TimeSpan value, bool partOfDateTime, int initialData)
		{
			var packedData = new BitVector32(initialData);
			int days;
			var hours = Math.Abs(value.Hours);
			var minutes = Math.Abs(value.Minutes);
			var seconds = Math.Abs(value.Seconds);
			var milliseconds = Math.Abs(value.Milliseconds);
			var hasTime = hours != 0 || minutes != 0;
			var optionalBytes = 0;

			if (partOfDateTime)
			{
				days = 0;
			}
			else
			{
				days = Math.Abs(value.Days);
				packedData[IsNegativeSection] = value.Ticks < 0 ? 1 : 0;
				packedData[HasDaysSection] = days != 0 ? 1 : 0;
			}

			if (hasTime)
			{
				packedData[HasTimeSection] = 1;
				packedData[HoursSection] = hours;
				packedData[MinutesSection] = minutes;
			}

			if (seconds != 0)
			{
				packedData[HasSecondsSection] = 1;

				if (!hasTime && (milliseconds == 0)) // If only seconds are present then we can use the minutes slot to save a byte
				{
					packedData[MinutesSection] = seconds;
				}
				else
				{
					packedData[SecondsSection] = seconds;
					optionalBytes++;
				}
			}

			if (milliseconds != 0)
			{
				packedData[HasMillisecondsSection] = 1;
				packedData[MillisecondsSection] = milliseconds;
				optionalBytes = 2;
			}

			var data = packedData.Data;
			Write((byte) data);
			Write((byte) (data >> 8)); // Always write minimum of two bytes

			if (optionalBytes > 0)
			{
				Write((byte) (data >> 16));
			}

			if (optionalBytes > 1)
			{
				Write((byte) (data >> 24));
			}

			if (days != 0)
			{
				Write7BitEncodedSigned32BitValue(days);
			}
		}

		/// <summary>
		/// Checks whether an optimization condition has been met and throw an exception if not.
		/// 
		/// This method has been made conditional on THROW_IF_NOT_OPTIMIZABLE being set at compile time.
		/// By default, this isn't set but could be set explicitly if exceptions are required and
		/// the evaluation overhead is acceptable. 
		/// If not set, then this method and all references to it are removed at compile time.
		/// 
		/// Leave at the default for optimum usage.
		/// </summary>
		/// <param name="condition">An expression evaluating to true if the optimization condition is met, false otherwise.</param>
		/// <param name="message">The message to include in the exception should the optimization condition not be met.</param>
		[Conditional("THROW_IF_NOT_OPTIMIZABLE")]
		static void CheckOptimizable(bool condition, string message)
		{
			if (!condition)
			{
				throw new InvalidOperationException(message);
			}
		}

		/// <summary>
		/// Stores a 32-bit signed value into the stream using 7-bit encoding.
		/// 
		/// The value is written 7 bits at a time (starting with the least-significant bits) until there are no more bits to write.
		/// The eighth bit of each byte stored is used to indicate whether there are more bytes following this one.
		/// 
		/// See Write(Int32) for details of the values that are optimizable.
		/// </summary>
		/// <param name="value">The Int32 value to encode.</param>
		void Write7BitEncodedSigned32BitValue(int value)
		{
			var unsignedValue = unchecked((uint) value);

			while(unsignedValue >= 0x80)
			{
				Write((byte) (unsignedValue | 0x80));
				unsignedValue >>= 7;
			}

			Write((byte) unsignedValue);
		}

		/// <summary>
		/// Stores a 64-bit signed value into the stream using 7-bit encoding.
		/// 
		/// The value is written 7 bits at a time (starting with the least-significant bits) until there are no more bits to write.
		/// The eighth bit of each byte stored is used to indicate whether there are more bytes following this one.
		/// 
		/// See Write(Int64) for details of the values that are optimizable.
		/// </summary>
		/// <param name="value">The Int64 value to encode.</param>
		void Write7BitEncodedSigned64BitValue(long value)
		{
			var unsignedValue = unchecked((ulong) value);

			while(unsignedValue >= 0x80)
			{
				Write((byte) (unsignedValue | 0x80));
				unsignedValue >>= 7;
			}

			Write((byte) unsignedValue);
		}

		/// <summary>
		/// Stores a 32-bit unsigned value into the stream using 7-bit encoding.
		/// 
		/// The value is written 7 bits at a time (starting with the least-significant bits) until there are no more bits to write.
		/// The eighth bit of each byte stored is used to indicate whether there are more bytes following this one.
		/// 
		/// See Write(UInt32) for details of the values that are optimizable.
		/// </summary>
		/// <param name="value">The UInt32 value to encode.</param>
		void Write7BitEncodedUnsigned32BitValue(uint value)
		{
			while(value >= 0x80)
			{
				Write((byte) (value | 0x80));
				value >>= 7;
			}

			Write((byte) value);
		}

		/// <summary>
		/// Stores a 64-bit unsigned value into the stream using 7-bit encoding.
		/// 
		/// The value is written 7 bits at a time (starting with the least-significant bits) until there are no more bits to write.
		/// The eighth bit of each byte stored is used to indicate whether there are more bytes following this one.
		/// 
		/// See Write(ULong) for details of the values that are optimizable.
		/// </summary>
		/// <param name="value">The ULong value to encode.</param>
		void Write7BitEncodedUnsigned64BitValue(ulong value)
		{
			while(value >= 0x80)
			{
				Write((byte) (value | 0x80));
				value >>= 7;
			}

			Write((byte) value);
		}

		/// <summary>
		/// Internal implementation to store a non-null Boolean[].
		/// </summary>
		/// <remarks>
		/// Stored as a BitArray for optimization.
		/// </remarks>
		/// <param name="values">The Boolean[] to store.</param>
		void WriteArray(bool[] values) 
		{
			WriteOptimized(new BitArray(values));
		}

		/// <summary>
		/// Internal implementation to store a non-null Byte[].
		/// </summary>
		/// <param name="values">The Byte[] to store.</param>
		void WriteArray(byte[] values)
		{
			Write7BitEncodedSigned32BitValue(values.Length);

			if (values.Length > 0)
			{
				base.Write(values);
			}
		}

		/// <summary>
		/// Internal implementation to store a non-null Char[].
		/// </summary>
		/// <param name="values">The Char[] to store.</param>
		void WriteArray(char[] values)
		{
			Write7BitEncodedSigned32BitValue(values.Length);

			if (values.Length > 0)
			{
				base.Write(values);
			}
		}

		/// <summary>
		/// Internal implementation to write a non, null DateTime[] using a BitArray to 
		/// determine which elements are optimizable.
		/// </summary>
		/// <param name="values">The DateTime[] to store.</param>
		/// <param name="optimizeFlags">A BitArray indicating which of the elements which are optimizable; 
		/// a reference to constant FullyOptimizableValueArray if all the elements are optimizable; or null
		/// if none of the elements are optimizable.</param>
		void WriteArray(DateTime[] values, BitArray optimizeFlags)
		{
			WriteTypedArrayTypeCode(optimizeFlags, values.Length);

			for(var i = 0; i < values.Length; i++)
			{
				if ((optimizeFlags == null) || ((optimizeFlags != FullyOptimizableTypedArray) && !optimizeFlags[i]))
				{
					Write(values[i]);
				}
				else
				{
					WriteOptimized(values[i]);
				}
			}
		}

		/// <summary>
		/// Internal implementation to store a non-null Decimal[].
		/// </summary>
		/// <remarks>
		/// All elements are stored optimized.
		/// </remarks>
		/// <param name="values">The Decimal[] to store.</param>
		void WriteArray(decimal[] values)
		{
			Write7BitEncodedSigned32BitValue(values.Length);

			for(var i = 0; i < values.Length; i++)
			{
				WriteOptimized(values[i]);
			}
		}

		/// <summary>
		/// Internal implementation to store a non-null Double[].
		/// </summary>
		/// <param name="values">The Double[] to store.</param>
		void WriteArray(double[] values)
		{
			Write7BitEncodedSigned32BitValue(values.Length);

			foreach(var value in values)
			{
				Write(value);
			}
		}

		/// <summary>
		/// Internal implementation to store a non-null Single[].
		/// </summary>
		/// <param name="values">The Single[] to store.</param>
		void WriteArray(float[] values)
		{
			Write7BitEncodedSigned32BitValue(values.Length);

			foreach(var value in values)
			{
				Write(value);
			}
		}

		/// <summary>
		/// Internal implementation to store a non-null Guid[].
		/// </summary>
		/// <param name="values">The Guid[] to store.</param>
		void WriteArray(Guid[] values)
		{
			Write7BitEncodedSigned32BitValue(values.Length);

			foreach(var value in values)
			{
				Write(value);
			}
		}

		/// <summary>
		/// Internal implementation to write a non-null Int16[] using a BitArray to determine which elements are optimizable.
		/// </summary>
		/// <param name="values">The Int16[] to store.</param>
		/// <param name="optimizeFlags">A BitArray indicating which of the elements which are optimizable; 
		/// a reference to constant FullyOptimizableValueArray if all the elements are optimizable; or null
		/// if none of the elements are optimizable.</param>
		void WriteArray(short[] values, BitArray optimizeFlags)
		{
			WriteTypedArrayTypeCode(optimizeFlags, values.Length);

			for(var i = 0; i < values.Length; i++)
			{
				if ((optimizeFlags == null) || ((optimizeFlags != FullyOptimizableTypedArray) && !optimizeFlags[i]))
				{
					Write(values[i]);
				}
				else
				{
					Write7BitEncodedSigned32BitValue(values[i]);
				}
			}
		}

		/// <summary>
		/// Internal implementation to write a non-null Int32[] using a BitArray to determine which elements are optimizable.
		/// </summary>
		/// <param name="values">The Int32[] to store.</param>
		/// <param name="optimizeFlags">A BitArray indicating which of the elements which are optimizable; 
		/// a reference to constant FullyOptimizableValueArray if all the elements are optimizable; or null
		/// if none of the elements are optimizable.</param>
		void WriteArray(int[] values, BitArray optimizeFlags)
		{
			WriteTypedArrayTypeCode(optimizeFlags, values.Length);

			for(var i = 0; i < values.Length; i++)
			{
				if ((optimizeFlags == null) || ((optimizeFlags != FullyOptimizableTypedArray) && !optimizeFlags[i]))
				{
					Write(values[i]);
				}
				else
				{
					Write7BitEncodedSigned32BitValue(values[i]);
				}
			}
		}

		/// <summary>
		/// Internal implementation to writes a non-null Int64[] using a BitArray to determine which elements are optimizable.
		/// </summary>
		/// <param name="values">The Int64[] to store.</param>
		/// <param name="optimizeFlags">A BitArray indicating which of the elements which are optimizable; 
		/// a reference to constant FullyOptimizableValueArray if all the elements are optimizable; or null
		/// if none of the elements are optimizable.</param>
		void WriteArray(long[] values, BitArray optimizeFlags)
		{
			WriteTypedArrayTypeCode(optimizeFlags, values.Length);

			for(var i = 0; i < values.Length; i++)
			{
				if ((optimizeFlags == null) || ((optimizeFlags != FullyOptimizableTypedArray) && !optimizeFlags[i]))
				{
					Write(values[i]);
				}
				else
				{
					Write7BitEncodedSigned64BitValue(values[i]);
				}
			}
		}

		/// <summary>
		/// Internal implementation to store a non-null SByte[].
		/// </summary>
		/// <param name="values">The SByte[] to store.</param>
		void WriteArray(sbyte[] values)
		{
			Write7BitEncodedSigned32BitValue(values.Length);

			foreach(var value in values)
			{
				Write(value);
			}
		}

		/// <summary>
		/// Internal implementation to store a non-null Int16[].
		/// </summary>
		/// <param name="values">The Int16[] to store.</param>
		void WriteArray(short[] values)
		{
			Write7BitEncodedSigned32BitValue(values.Length);

			foreach(var value in values)
			{
				Write(value);
			}
		}

		/// <summary>
		/// Internal implementation to write a non-null TimeSpan[] using a BitArray to determine which elements are optimizable.
		/// </summary>
		/// <param name="values">The TimeSpan[] to store.</param>
		/// <param name="optimizeFlags">A BitArray indicating which of the elements which are optimizable; 
		/// a reference to constant FullyOptimizableValueArray if all the elements are optimizable; or null
		/// if none of the elements are optimizable.</param>
		void WriteArray(TimeSpan[] values, BitArray optimizeFlags)
		{
			WriteTypedArrayTypeCode(optimizeFlags, values.Length);

			for(var i = 0; i < values.Length; i++)
			{
				if ((optimizeFlags == null) || ((optimizeFlags != FullyOptimizableTypedArray) && !optimizeFlags[i]))
				{
					Write(values[i]);
				}
				else
				{
					WriteOptimized(values[i]);
				}
			}
		}

		/// <summary>
		/// Internal implementation to write a non-null UInt16[] using a BitArray to determine which elements are optimizable.
		/// </summary>
		/// <param name="values">The UInt16[] to store.</param>
		/// <param name="optimizeFlags">A BitArray indicating which of the elements which are optimizable; 
		/// a reference to constant FullyOptimizableValueArray if all the elements are optimizable; or null
		/// if none of the elements are optimizable.</param>
		void WriteArray(ushort[] values, BitArray optimizeFlags)
		{
			WriteTypedArrayTypeCode(optimizeFlags, values.Length);

			for(var i = 0; i < values.Length; i++)
			{
				if ((optimizeFlags == null) || ((optimizeFlags != FullyOptimizableTypedArray) && !optimizeFlags[i]))
				{
					Write(values[i]);
				}
				else
				{
					Write7BitEncodedUnsigned32BitValue(values[i]);
				}
			}
		}

		/// <summary>
		/// Internal implementation to write a non-null UInt32[] using a BitArray to determine which elements are optimizable.
		/// </summary>
		/// <param name="values">The UInt32[] to store.</param>
		/// <param name="optimizeFlags">A BitArray indicating which of the elements which are optimizable; 
		/// a reference to constant FullyOptimizableValueArray if all the elements are optimizable; or null
		/// if none of the elements are optimizable.</param>
		void WriteArray(uint[] values, BitArray optimizeFlags)
		{
			WriteTypedArrayTypeCode(optimizeFlags, values.Length);

			for(var i = 0; i < values.Length; i++)
			{
				if ((optimizeFlags == null) || ((optimizeFlags != FullyOptimizableTypedArray) && !optimizeFlags[i]))
				{
					Write(values[i]);
				}
				else
				{
					Write7BitEncodedUnsigned32BitValue(values[i]);
				}
			}
		}

		/// <summary>
		/// Internal implementation to store a non-null UInt16[].
		/// </summary>
		/// <param name="values">The UIn16[] to store.</param>
		void WriteArray(ushort[] values)
		{
			Write7BitEncodedSigned32BitValue(values.Length);

			foreach(var value in values)
			{
				Write(value);
			}
		}

		/// <summary>
		/// Internal implementation to write a non-null UInt64[] using a BitArray to determine which elements are optimizable.
		/// </summary>
		/// <param name="values">The UInt64[] to store.</param>
		/// <param name="optimizeFlags">A BitArray indicating which of the elements which are optimizable; 
		/// a reference to constant FullyOptimizableValueArray if all the elements are optimizable; or null
		/// if none of the elements are optimizable.</param>
		void WriteArray(ulong[] values, BitArray optimizeFlags)
		{
			WriteTypedArrayTypeCode(optimizeFlags, values.Length);

			for(var i = 0; i < values.Length; i++)
			{
				if ((optimizeFlags == null) || ((optimizeFlags != FullyOptimizableTypedArray) && !optimizeFlags[i]))
				{
					Write(values[i]);
				}
				else
				{
					Write7BitEncodedUnsigned64BitValue(values[i]);
				}
			}
		}

		/// <summary>
		/// Writes the values in the non-null object[] into the stream.
		/// 
		/// Sequences of null values and sequences of DBNull.Values are stored with a flag and optimized count.
		/// Other values are stored using WriteObject().
		/// 
		/// This routine is called by the Write(object[]), WriteOptimized(object[]) and Write(object[], object[])) methods.
		/// </summary>
		/// <param name="values"></param>
		void WriteObjectArray(object[] values)
		{
			Write7BitEncodedSigned32BitValue(values.Length);
			var lastIndex = values.Length - 1;

			for(var i = 0; i < values.Length; i++)
			{
				var value = values[i];
				
				if ((i < lastIndex) && (value == null ? values[i + 1] == null : value.Equals(values[i + 1])) )
				{
					var duplicates = 1;

					if (value == null)
					{
						WriteTypeCode(SerializedType.NullSequenceType);

						for (i++; i < lastIndex && values[i + 1] == null; i++)
						{
							duplicates++;
						}
					}
					else if (value == DBNull.Value)
					{
						WriteTypeCode(SerializedType.DBNullSequenceType);

						for (i++; i < lastIndex && values[i + 1] == DBNull.Value; i++)
						{
							duplicates++;
						}
					}
					else
					{
						WriteTypeCode(SerializedType.DuplicateValueSequenceType);

						for (i++; i < lastIndex && value.Equals(values[i + 1]); i++)
						{
							duplicates++;
						}

						WriteObject(value);
					}

					Write7BitEncodedSigned32BitValue(duplicates);
				} 
				else
				{
					WriteObject(value);
				}
			}
		}

		
		/// <summary>
		/// Stores the specified SerializedType code into the stream.
		/// 
		/// By using a centralized method, it is possible to collect statistics for the
		/// type of data being stored in DEBUG mode.
		/// 
		/// Use the DumpTypeUsage() method to show a list of used SerializedTypes and
		/// the number of times each has been used. This method and the collection code
		/// will be optimized out when compiling in Release mode.
		/// </summary>
		/// <param name="typeCode">The SerializedType to store.</param>
		void WriteTypeCode(SerializedType typeCode)
		{
			Write((byte) typeCode);
#if DEBUG
			typeUsage[(int) typeCode]++;
#endif
		}

		/// <summary>
		/// Internal implementation to write a non-null typed array into the stream.
		/// </summary>
		/// <remarks>
		/// Checks first to see if the element type is a primitive type and calls the 
		/// correct routine if so. Otherwise determines the best, optimized method
		/// to store the array contents.
		/// <para/>
		/// An array of object elements never stores its type.
		/// </remarks>
		/// <param name="value">The non-null typed array to store.</param>
		/// <param name="storeType">True if the type should be stored; false otherwise</param>
		void WriteTypedArray(Array value, bool storeType)
		{
			var elementType = value.GetType().GetElementType();

			if (elementType == typeof(object))
			{
				storeType = false;
			}

			if (elementType == typeof(string))
			{
				WriteTypeCode(SerializedType.StringArrayType);
				WriteOptimized((object[]) value);
			}
			else if (elementType == typeof(Int32))
			{
				WriteTypeCode(SerializedType.Int32ArrayType);

				if (optimizeForSize)
				{
					WriteOptimized((Int32[]) value);
				}
				else
				{
					Write((Int32[]) value);
				}
			}
			else if (elementType == typeof(Int16))
			{
				WriteTypeCode(SerializedType.Int16ArrayType);

				if (optimizeForSize)
				{
					WriteOptimized((Int16[]) value);
				}
				else
				{
					Write((Int16[]) value);
				}
			}
			else if (elementType == typeof(Int64))
			{
				WriteTypeCode(SerializedType.Int64ArrayType);

				if (optimizeForSize)
				{
					WriteOptimized((Int64[]) value);
				}
				else 
				{
					Write((Int64[]) value);
				}
			}
			else if (elementType == typeof(UInt32))
			{
				WriteTypeCode(SerializedType.UInt32ArrayType);

				if (optimizeForSize)
				{
					WriteOptimized((UInt32[]) value);
				}
				else 
				{
					Write((UInt32[]) value);
				}
			}
			else if (elementType == typeof(UInt16))
			{
				WriteTypeCode(SerializedType.UInt16ArrayType);

				if (optimizeForSize)
				{
					WriteOptimized((UInt16[]) value); 
				}
				else 
				{
					Write((UInt16[]) value);
				}
			}
			else if (elementType == typeof(UInt64))
			{
				WriteTypeCode(SerializedType.UInt64ArrayType);

				if (optimizeForSize)
				{
					WriteOptimized((UInt64[]) value);
				}
				else 
				{
					Write((UInt64[]) value);
				}
			}
			else if (elementType == typeof(Single))
			{
				WriteTypeCode(SerializedType.SingleArrayType);
				WriteArray((Single[]) value);
			}
			else if (elementType == typeof(Double))
			{
				WriteTypeCode(SerializedType.DoubleArrayType);
				WriteArray((Double[]) value);
			}
			else if (elementType == typeof(Decimal))
			{
				WriteTypeCode(SerializedType.DecimalArrayType);
				WriteArray((Decimal[]) value);
			}
			else if (elementType == typeof(DateTime))
			{
				WriteTypeCode(SerializedType.DateTimeArrayType);

				if (optimizeForSize)
				{
					WriteOptimized((DateTime[]) value); 
				}
				else 
				{
					Write((DateTime[]) value);
				}
			}
			else if (elementType == typeof(TimeSpan))
			{
				WriteTypeCode(SerializedType.TimeSpanArrayType);

				if (optimizeForSize) 
				{
					WriteOptimized((TimeSpan[]) value); 
				}
				else 
				{
					Write((TimeSpan[]) value);
				}
			}
			else if (elementType == typeof(Guid))
			{
				WriteTypeCode(SerializedType.GuidArrayType);
				WriteArray((Guid[]) value);
			}
			else if (elementType == typeof(SByte))
			{
				WriteTypeCode(SerializedType.SByteArrayType);
				WriteArray((SByte[]) value);
			}
			else if (elementType == typeof(Boolean))
			{
				WriteTypeCode(SerializedType.BooleanArrayType);
				WriteArray((bool[]) value);
			}
			else if (elementType == typeof(Byte))
			{
				WriteTypeCode(SerializedType.ByteArrayType);
				WriteArray((Byte[]) value);
			}
			else if (elementType == typeof(Char))
			{
				WriteTypeCode(SerializedType.CharArrayType);
				WriteArray((Char[]) value);
			}
			else if (value.Length == 0)
			{
				WriteTypeCode(elementType == typeof(object) ? SerializedType.EmptyObjectArrayType : SerializedType.EmptyTypedArrayType);

				if (storeType) 
				{
					WriteOptimized(elementType);
				}
			}
			else if (elementType == typeof(object))
			{
				WriteTypeCode(SerializedType.ObjectArrayType);
				WriteObjectArray((object[]) value);
			}
			else
			{
				var optimizeFlags = IsTypeRecreatable(elementType) ? FullyOptimizableTypedArray : null;

				if (!elementType.IsValueType)
				{
					if ((optimizeFlags == null) || !ArrayElementsAreSameType((object[]) value, elementType))
					{
						if (!storeType)
						{
							WriteTypeCode(SerializedType.ObjectArrayType);
						}
						else
						{
							WriteTypeCode(SerializedType.OtherTypedArrayType);
							WriteOptimized(elementType);
						}

						WriteObjectArray((object[]) value);
						return;
					}
					
					for (var i = 0; i < value.Length; i++)
					{
						if (value.GetValue(i) == null)
						{
							if (optimizeFlags == FullyOptimizableTypedArray)
							{
								optimizeFlags = new BitArray(value.Length);
							}

							optimizeFlags[i] = true;
						}
					}
				}

				WriteTypedArrayTypeCode(optimizeFlags, value.Length);

				if (storeType) 
				{
					WriteOptimized(elementType);
				}

				for (var i = 0; i < value.Length; i++)
				{
					if (optimizeFlags == null)
					{
						WriteObject(value.GetValue(i));
					}
					else if (optimizeFlags == FullyOptimizableTypedArray || !optimizeFlags[i])
					{
						Write((IOwnedDataSerializable) value.GetValue(i), null);
					}
				}
			}
		}

		/// <summary>
		/// Checks whether instances of a Type can be created.
		/// </summary>
		/// <remarks>
		/// A Value Type only needs to implement IOwnedDataSerializable. 
		/// A Reference Type needs to implement IOwnedDataSerializableAndRecreatable and provide a default constructor.
		/// </remarks>
		/// <param name="type">The Type to check</param>
		/// <returns>true if the Type is recreatable; false otherwise.</returns>
		static bool IsTypeRecreatable(Type type)
		{
			if (type.IsValueType) return typeof(IOwnedDataSerializable).IsAssignableFrom(type);

			return typeof(IOwnedDataSerializableAndRecreatable).IsAssignableFrom(type) && HasEmptyConstructor(type);
		}

		/// <summary>
		/// Checks whether a type has a default/empty constructor.
		/// </summary>
		/// <param name="type">The Type to check</param>
		/// <returns>true if the Type has a default/empty constructor; false otherwise.</returns>
		static bool HasEmptyConstructor(Type type)
		{
			return type.GetConstructor(Type.EmptyTypes) != null;
		}

		/// <summary>
		/// Checks whether each element in an array is of the same type.
		/// </summary>
		/// <param name="values">The array to check</param>
		/// <param name="elementType">The expected element type.</param>
		/// <returns></returns>
		static bool ArrayElementsAreSameType(object[] values, Type elementType)
		{
			foreach(var value in values)
			{
				if ((value != null) && (value.GetType() != elementType)) return false;
			}

			return true;
		}

		/// <summary>
		/// Writes the TypeCode for the Typed Array followed by the number of elements.
		/// </summary>
		/// <param name="optimizeFlags"></param>
		/// <param name="length"></param>
		void WriteTypedArrayTypeCode(BitArray optimizeFlags, int length)
		{
			if (optimizeFlags == null)
			{
				WriteTypeCode(SerializedType.NonOptimizedTypedArrayType);
			}
			else if (optimizeFlags == FullyOptimizableTypedArray)
			{
				WriteTypeCode(SerializedType.FullyOptimizedTypedArrayType);
			}
			else
			{
				WriteTypeCode(SerializedType.PartiallyOptimizedTypedArrayType);
				WriteOptimized(optimizeFlags);
			}

			Write7BitEncodedSigned32BitValue(length);
		}

		/// <summary>
		/// gets the list of optional IFastSerializationTypeSurrogate instances which
		/// SerializationWriter and SerializationReader will use to serialize objects not directly supported.
		/// It is important to use the same list on both client and server ends to ensure that the same surrogated-types are supported.
		/// </summary>
		public static List<IFastSerializationTypeSurrogate> TypeSurrogates 
		{
			get { return typeSurrogates; }			
		}

		/// <summary>
		/// Returns the number of strings in the string token table.
		/// </summary>
		public int StringTokenTableSize
		{
			get { return stringLookup.Count; }
		}

		internal int GetTotalStringSize()
		{
			var strings = (string[]) typeof(UniqueStringList).GetField("stringList", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(stringLookup);
			var result = 0;

			for(var i = 0; i < stringLookup.Count; i++)
			{
				result += strings[i].Length;
			}

			return result;
		}

		/// <summary>
		/// Returns the number of objects in the object token table.
		/// </summary>
		public int ObjectTokenTableSize
		{
			get { return objectTokenLookup.Count; }
		}

		/// <summary>
		/// Gets or Sets a boolean flag to indicate whether to optimize for size (default)
		/// by storing data as packed bits or sections where possible.
		/// Setting this value to false will turn off this optimization and store
		/// data directly which increases the speed.
		/// Note: This only affects optimization of data passed to the WriteObject method
		/// and direct calls to the WriteOptimized methods will always pack data into
		/// the smallest space where possible.
		/// </summary>
		public bool OptimizeForSize
		{
			get { return optimizeForSize; }
			set { optimizeForSize = value; }
		}

		/// <summary>
		/// Gets or Sets a boolean flag to indicate whether to preserve the scale within
		/// a Decimal value when it would have no effect on the represented value.
		/// Note: a 2m value and a 2.00m value represent the same value but internally they 
		/// are stored differently - the former has a value of 2 and a scale of 0 and
		/// the latter has a value of 200 and a scale of 2. 
		/// The scaling factor also preserves any trailing zeroes in a Decimal number. 
		/// Trailing zeroes do not affect the value of a Decimal number in arithmetic or 
		/// comparison operations. However, trailing zeroes can be revealed by the ToString 
		/// method if an appropriate format string is applied.
		/// From a serialization point of view, the former will take 2 bytes whereas the 
		/// latter would take 4 bytes, therefore it is preferable to not save the scale where
		/// it doesn't affect the represented value.
		/// </summary>
		public bool PreserveDecimalScale
		{
			get { return preserveDecimalScale; }
			set { preserveDecimalScale = value; }
		} 

		#region Private Classes
		/// <summary>
		/// Private class used to wrap an object that is to be tokenized, and recreated at deserialization by its type.
		/// </summary>
		class SingletonTypeWrapper
		{
			readonly Type wrappedType;

			/// <summary>
			/// Initializes a new instance of the <see cref="SingletonTypeWrapper"/> class.
			/// </summary>
			/// <param name="value">The value.</param>
			public SingletonTypeWrapper(object value)
			{
				wrappedType = value.GetType();
			}

			/// <summary>
			/// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
			/// </summary>
			/// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
			/// <returns>
			/// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
			/// </returns>
			public override bool Equals(object obj)
			{
				return wrappedType.Equals(((SingletonTypeWrapper) obj).wrappedType);
			}

			/// <summary>
			/// Serves as a hash function for a particular type.
			/// </summary>
			/// <returns>
			/// A hash code for the current <see cref="T:System.Object"></see>.
			/// </returns>
			public override int GetHashCode()
			{
				return wrappedType.GetHashCode();
			}

			/// <summary>
			/// Gets the type of the wrapped.
			/// </summary>
			/// <value>The type of the wrapped.</value>
			public Type WrappedType
			{
				get { return wrappedType; }
			}
		}

		/// <summary>
		/// Provides a faster way to store string tokens both maintaining the order that they were added and
		/// providing a fast lookup.
		/// 
		/// Based on code developed by ewbi at http://ewbi.blogs.com/develops/2006/10/uniquestringlis.html
		/// </summary>
		sealed class UniqueStringList
		{
			const float LoadFactor = .72f;

			// Based on Golden Primes (as far as possible from nearest two powers of two)
			// at http://planetmath.org/encyclopedia/GoodHashTablePrimes.html
			static readonly int[] PrimeNumberList = new[]
				{
					// 193, 769, 3079, 12289, 49157 removed to allow quadrupling of bucket table size
					// for smaller size then reverting to doubling
					389, 1543, 6151, 24593, 98317, 196613, 393241, 786433, 1572869, 3145739, 6291469,
					12582917, 25165843, 50331653, 100663319, 201326611, 402653189, 805306457, 1610612741
				};

			string[] stringList;
			int[] buckets;
			int bucketListCapacity;
			int stringListIndex;
			int loadLimit;
			int primeNumberListIndex;

			/// <summary>
			/// Initializes a new instance of the <see cref="UniqueStringList"/> class.
			/// </summary>
			public UniqueStringList()
			{
				bucketListCapacity = PrimeNumberList[primeNumberListIndex++];
				stringList = new string[bucketListCapacity];
				buckets = new int[bucketListCapacity];
				loadLimit = (int)(bucketListCapacity * LoadFactor);
			}

			public bool Add(string value, out int index)
			{
				var bucketIndex = GetBucketIndex(value);
				index = buckets[bucketIndex] - 1;
				if (index == -1)
				{
					stringList[stringListIndex++] = value;
					buckets[bucketIndex] = stringListIndex;
					if (stringListIndex > loadLimit)
					{
						Expand();
					}
					index = stringListIndex - 1;
					return true;
				}
				return false;
			}

			/// <summary>
			/// Expands this instance.
			/// </summary>
			void Expand()
			{
				bucketListCapacity = PrimeNumberList[primeNumberListIndex++];
				buckets = new int[bucketListCapacity];
				var newStringlist = new string[bucketListCapacity];
				stringList.CopyTo(newStringlist, 0);
				stringList = newStringlist;
				Reindex();
			}

			/// <summary>
			/// Reindexes this instance.
			/// </summary>
			void Reindex()
			{
				loadLimit = (int)(bucketListCapacity * LoadFactor);
				for(var stringIndex = 0; stringIndex < stringListIndex; stringIndex++)
				{
					var index = GetBucketIndex(stringList[stringIndex]);
					buckets[index] = stringIndex + 1;
				}
			}

			/// <summary>
			/// Gets the index of the bucket.
			/// </summary>
			/// <param name="value">The value.</param>
			/// <returns></returns>
			int GetBucketIndex(string value)
			{
				var hashCode = value.GetHashCode() & 0x7fffffff;
				var bucketIndex = hashCode % bucketListCapacity;
				var increment = (bucketIndex > 1) ? bucketIndex : 1;
				var i = bucketListCapacity;

				while(0 < i--)
				{
					var stringIndex = buckets[bucketIndex];
					if (stringIndex == 0) return bucketIndex;

					if (value.Equals(stringList[stringIndex - 1]))
					{
						return bucketIndex;
					}

					bucketIndex = (bucketIndex + increment) % bucketListCapacity; // Probe.
				}

				throw new InvalidOperationException("Failed to locate a bucket.");
			}

			/// <summary>
			/// Gets the count.
			/// </summary>
			/// <value>The count.</value>
			public int Count
			{
				get { return stringListIndex; }
			}
		}
		#endregion

	}
}
