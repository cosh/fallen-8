using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace Framework.Serialization
{
	/// <summary>
	/// A SerializationReader instance is used to read stored values and objects from a byte array.
	///
	/// Once an instance is created, use the various methods to read the required data.
	/// The data read MUST be exactly the same type and in the same order as it was written.
	/// </summary>
	public sealed class SerializationReader: BinaryReader
	{
		// Marker to denote that all elements in a value array are optimizable
		static readonly BitArray FullyOptimizableTypedArray = new BitArray(0);

		readonly int startPosition;
		readonly int endPosition;
		List<string> stringTokenList;
		List<object> objectTokenList;

		#region Debug Related
		/// <summary>
		/// Dumps the string tables.
		/// </summary>
		/// <param name="list">The list.</param>
		[Conditional("DEBUG")]
		public void DumpStringTables(ArrayList list)
		{
			list.AddRange(stringTokenList);
		}
		#endregion

		/// <summary>
		/// Creates a SerializationReader using a byte[] previous created by SerializationWriter
		/// 
		/// A MemoryStream is used to access the data without making a copy of it.
		/// </summary>
		/// <param name="data">The byte[] containining serialized data.</param>
		public SerializationReader(byte[] data): this(new MemoryStream(data)) 
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SerializationReader"/> class.
		/// </summary>
		/// <param name="stream">The stream.</param>
		public SerializationReader(Stream stream): base(stream)
		{
			// Store the start position of the stream if seekable
			startPosition = stream.CanSeek ? (int) stream.Position : 0;

			// Always read the first 4 bytes
			endPosition = startPosition + ReadInt32();

			// If the first four bytes are zero
			if (startPosition == endPosition)
			{
				// then there is no token table presize info
				InitializeTokenTables(0, 0);
			}
			else
			{
				// Use the correct token table sizes
				InitializeTokenTables(ReadInt32(), ReadInt32());
			}
		}

		/// <summary>
		/// Creates a SerializationReader based around the passed Stream.
		/// Allows the string and object token tables to be presized using
		/// the specified values.
		/// </summary>
		/// <param name="stream">The stream containing the serialized data</param>
		/// <param name="stringTokenTablePresize">Number of string tokens to presize</param>
		/// <param name="objectTokenTablePresize">Number of object tokens to presize</param>
		public SerializationReader(Stream stream, int stringTokenTablePresize, int objectTokenTablePresize): base(stream)
		{
			if (stringTokenTablePresize < 0) throw new ArgumentOutOfRangeException("stringTokenTablePresize", "Cannot be negative");
			if (objectTokenTablePresize < 0) throw new ArgumentOutOfRangeException("objectTokenTablePresize", "Cannot be negative");

			// Store the start position of the stream if seekable
			startPosition = stream.CanSeek ? (int) stream.Position : 0;

			// Always read the first 4 bytes
			endPosition = startPosition + ReadInt32();

			// Need to ignore token table size if present (unlikely)
			if (startPosition != endPosition)
			{
				ReadInt32();
				ReadInt32();
			}

			InitializeTokenTables(stringTokenTablePresize, objectTokenTablePresize);
		}

		/// <summary>
		/// Returns an ArrayList or null from the stream.
		/// </summary>
		/// <returns>An ArrayList instance.</returns>
		public ArrayList ReadArrayList()
		{
			if (ReadTypeCode() == SerializedType.NullType) return null;

			return new ArrayList(ReadOptimizedObjectArray());
		}

		/// <summary>
		/// Returns a BitArray or null from the stream.
		/// </summary>
		/// <returns>A BitArray instance.</returns>
		public BitArray ReadBitArray()
		{
			if (ReadTypeCode() == SerializedType.NullType) return null;

			return ReadOptimizedBitArray();
		}

		/// <summary>
		/// Returns a BitVector32 value from the stream.
		/// </summary>
		/// <returns>A BitVector32 value.</returns>
		public BitVector32 ReadBitVector32()
		{
			return new BitVector32(ReadInt32());
		}
		
		/// <summary>
		/// Reads the specified number of bytes directly from the stream.
		/// </summary>
		/// <param name="count">The number of bytes to read</param>
		/// <returns>A byte[] containing the read bytes</returns>
		public byte[] ReadBytesDirect(int count)
		{
			return ReadBytes(count);
		}

		/// <summary>
		/// Returns a DateTime value from the stream.
		/// </summary>
		/// <returns>A DateTime value.</returns>
		public DateTime ReadDateTime()
		{
			return DateTime.FromBinary(ReadInt64());
		}

		/// <summary>
		/// Returns a Guid value from the stream.
		/// </summary>
		/// <returns>A DateTime value.</returns>
		public Guid ReadGuid()
		{
			return new Guid(ReadBytes(16));
		}

		/// <summary>
		/// Returns an object based on the SerializedType read next from the stream.
		/// </summary>
		/// <returns>An object instance.</returns>
		public object ReadObject()
		{
			return ProcessObject((SerializedType) ReadByte());
		}
		/// <summary>
		/// Called ReadOptimizedString().
		/// This override to hide base BinaryReader.ReadString().
		/// </summary>
		/// <returns>A string value.</returns>
		public override string ReadString()
		{
			return ReadOptimizedString();
		}

		/// <summary>
		/// Returns a string value from the stream.
		/// </summary>
		/// <returns>A string value.</returns>
		public string ReadStringDirect()
		{
			return base.ReadString();
		}

		/// <summary>
		/// Returns a TimeSpan value from the stream.
		/// </summary>
		/// <returns>A TimeSpan value.</returns>
		public TimeSpan ReadTimeSpan()
		{
			return new TimeSpan(ReadInt64());
		}

		/// <summary>
		/// Returns a Type or null from the stream.
		/// 
		/// Throws an exception if the Type cannot be found.
		/// </summary>
		/// <returns>A Type instance.</returns>
		public Type ReadType()
		{
			return ReadType(true);
		}

		/// <summary>
		/// Returns a Type or null from the stream.
		/// 
		/// Throws an exception if the Type cannot be found and throwOnError is true.
		/// </summary>
		/// <returns>A Type instance.</returns>
		public Type ReadType(bool throwOnError)
		{
			if (ReadTypeCode() == SerializedType.NullType) return null;

			return Type.GetType(ReadOptimizedString(), throwOnError);
		}

		/// <summary>
		/// Returns an ArrayList from the stream that was stored optimized.
		/// </summary>
		/// <returns>An ArrayList instance.</returns>
		public ArrayList ReadOptimizedArrayList()
		{
			return new ArrayList(ReadOptimizedObjectArray());
		}

		/// <summary>
		/// Returns a BitArray from the stream that was stored optimized.
		/// </summary>
		/// <returns>A BitArray instance.</returns>
		public BitArray ReadOptimizedBitArray()
		{
			var length = ReadOptimizedInt32();
			if (length == 0) return FullyOptimizableTypedArray;

			return new BitArray(ReadBytes((length + 7) / 8)) { Length = length };
		}

		/// <summary>
		/// Returns a BitVector32 value from the stream that was stored optimized.
		/// </summary>
		/// <returns>A BitVector32 value.</returns>
		public BitVector32 ReadOptimizedBitVector32()
		{
			return new BitVector32(Read7BitEncodedInt());
		}

		/// <summary>
		/// Returns a DateTime value from the stream that was stored optimized.
		/// </summary>
		/// <returns>A DateTime value.</returns>
		public DateTime ReadOptimizedDateTime()
		{
			// Read date information from first three bytes
			var dateMask = new BitVector32(ReadByte() | (ReadByte() << 8) | (ReadByte() << 16));
			var result = new DateTime(
					dateMask[SerializationWriter.DateYearMask],
					dateMask[SerializationWriter.DateMonthMask],
					dateMask[SerializationWriter.DateDayMask]
			);

			if (dateMask[SerializationWriter.DateHasTimeOrKindMask] == 1)
			{
				var initialByte = ReadByte();
				var dateTimeKind = (DateTimeKind) (initialByte & 0x03);

				// Remove the IsNegative and HasDays flags which are never true for a DateTime
				initialByte &= 0xfc; 
				if (dateTimeKind != DateTimeKind.Unspecified)
				{
					result = DateTime.SpecifyKind(result, dateTimeKind);
				}

				if (initialByte == 0)
				{
					// No need to call decodeTimeSpan if there is no time information
					ReadByte(); 
				}
				else
				{
					result = result.Add(DecodeTimeSpan(initialByte));
				}
			}

			return result;
		}

		/// <summary>
		/// Returns a Decimal value from the stream that was stored optimized.
		/// </summary>
		/// <returns>A Decimal value.</returns>
		public Decimal ReadOptimizedDecimal()
		{
			var flags = ReadByte();
			var lo = 0;
			var mid = 0;
			var hi = 0;
			byte scale = 0;

			if ((flags & 0x02) != 0)
			{
				scale = ReadByte();
			}

			if ((flags & 4) == 0)
			{
				lo = (flags & 32) != 0 ? ReadOptimizedInt32() : ReadInt32();
			}

			if ((flags & 8) == 0)
			{
				mid = (flags & 64) != 0 ? ReadOptimizedInt32() : ReadInt32();
			}

			if ((flags & 16) == 0)
			{
				hi = (flags & 128) != 0 ? ReadOptimizedInt32() : ReadInt32();
			}

			return new decimal(lo, mid, hi, (flags & 0x01) != 0, scale);
		}

		/// <summary>
		/// Returns an Int32 value from the stream that was stored optimized.
		/// </summary>
		/// <returns>An Int32 value.</returns>
		public int ReadOptimizedInt32()
		{
			var result = 0;
			var bitShift = 0;

			while(true)
			{
				var nextByte = ReadByte();

				result |= (nextByte & 0x7f) << bitShift;
				bitShift += 7;

				if ((nextByte & 0x80) == 0) return result;
			}
		}

		/// <summary>
		/// Returns an Int16 value from the stream that was stored optimized.
		/// </summary>
		/// <returns>An Int16 value.</returns>
		public short ReadOptimizedInt16()
		{
			return (short) ReadOptimizedInt32();
		}

		/// <summary>
		/// Returns an Int64 value from the stream that was stored optimized.
		/// </summary>
		/// <returns>An Int64 value.</returns>
		public long ReadOptimizedInt64()
		{
			long result = 0;
			var bitShift = 0;

			while(true)
			{
				var nextByte = ReadByte();

				result |= ((long) nextByte & 0x7f) << bitShift;
				bitShift += 7;

				if ((nextByte & 0x80) == 0) return result;
			}
		}

		/// <summary>
		/// Returns an object[] from the stream that was stored optimized.
		/// </summary>
		/// <returns>An object[] instance.</returns>
		public object[] ReadOptimizedObjectArray()
		{
			return ReadOptimizedObjectArray(null);
		}

		/// <summary>
		/// Returns an object[] from the stream that was stored optimized.
		/// The returned array will be typed according to the specified element type
		/// and the resulting array can be cast to the expected type.
		/// e.g.
		/// string[] myStrings = (string[]) reader.ReadOptimizedObjectArray(typeof(string));
		/// 
		/// An exception will be thrown if any of the deserialized values cannot be
		/// cast to the specified elementType.
		/// 
		/// </summary>
		/// <param name="elementType">The Type of the expected array elements. null will return a plain object[].</param>
		/// <returns>An object[] instance.</returns>
		public object[] ReadOptimizedObjectArray(Type elementType)
		{
			var length = ReadOptimizedInt32();
			var result = (object[]) (elementType == null ? new object[length] : Array.CreateInstance(elementType, length));

			for (var i = 0; i < result.Length; i++)
			{
				var serializedType = (SerializedType) ReadByte();

				switch (serializedType)
				{
					case SerializedType.NullSequenceType:
						i += ReadOptimizedInt32();

						break;

					case SerializedType.DuplicateValueSequenceType:
						var target = result[i] = ReadObject();
						var duplicateValueCount = ReadOptimizedInt32();

						while (duplicateValueCount-- > 0)
						{
							result[++i] = target;
						}

						break;

					case SerializedType.DBNullSequenceType:
						result[i] = DBNull.Value;
						var duplicateDBNullCount = ReadOptimizedInt32();

						while (duplicateDBNullCount-- > 0)
						{
							result[++i] = DBNull.Value;
						}

						break;

					default:
						if (serializedType != SerializedType.NullType)
						{
							result[i] = ProcessObject(serializedType);
						}

						break;
				}
			}
			return result;
		}

		/// <summary>
		/// Returns a pair of object[] arrays from the stream that were stored optimized.
		/// </summary>
		/// <returns>A pair of object[] arrays.</returns>
		public void ReadOptimizedObjectArrayPair(out object[] values1, out object[] values2)
		{
			values1 = ReadOptimizedObjectArray(null);
			values2 = new object[values1.Length];

			for(var i = 0; i < values2.Length; i++)
			{
				var serializedType = (SerializedType) ReadByte();

				switch (serializedType)
				{
					case SerializedType.DuplicateValueSequenceType:
						values2[i] = values1[i];
						var duplicateValueCount = ReadOptimizedInt32();

						while (duplicateValueCount-- > 0)
						{
							values2[++i] = values1[i];
						}

						break;

					case SerializedType.DuplicateValueType:
						values2[i] = values1[i];

						break;

					case SerializedType.NullSequenceType:
						i += ReadOptimizedInt32();

						break;

					case SerializedType.DBNullSequenceType:
						values2[i] = DBNull.Value;
						var duplicates = ReadOptimizedInt32();

						while (duplicates-- > 0)
						{
							values2[++i] = DBNull.Value;
						}

						break;

					default:
						if (serializedType != SerializedType.NullType)
						{
							values2[i] = ProcessObject(serializedType);
						}

						break;
				}
			}
		}

		/// <summary>
		/// Returns a string value from the stream that was stored optimized.
		/// </summary>
		/// <returns>A string value.</returns>
		public string ReadOptimizedString()
		{
			var typeCode = ReadTypeCode();

			if (typeCode < SerializedType.NullType)
			{
				return ReadTokenizedString((int) typeCode);
			}

			switch (typeCode)
			{
				case SerializedType.NullType: return null;
				case SerializedType.YStringType: return "Y";
				case SerializedType.NStringType: return "N";
				case SerializedType.SingleCharStringType: return Char.ToString(ReadChar());
				case SerializedType.SingleSpaceType: return " ";
				case SerializedType.EmptyStringType: return string.Empty;

				default: throw new InvalidOperationException("Unrecognized TypeCode");
			}
		}

		/// <summary>
		/// Returns a TimeSpan value from the stream that was stored optimized.
		/// </summary>
		/// <returns>A TimeSpan value.</returns>
		public TimeSpan ReadOptimizedTimeSpan()
		{
			return DecodeTimeSpan(ReadByte());
		}

		/// <summary>
		/// Returns a Type from the stream.
		/// 
		/// Throws an exception if the Type cannot be found.
		/// </summary>
		/// <returns>A Type instance.</returns>
		public Type ReadOptimizedType()
		{
			return ReadOptimizedType(true);
		}
		/// <summary>
		/// Returns a Type from the stream.
		/// 
		/// Throws an exception if the Type cannot be found and throwOnError is true.
		/// </summary>
		/// <returns>A Type instance.</returns>
		public Type ReadOptimizedType(bool throwOnError)
		{
			return Type.GetType(ReadOptimizedString(), throwOnError);
		}

		/// <summary>
		/// Returns a UInt16 value from the stream that was stored optimized.
		/// </summary>
		/// <returns>A UInt16 value.</returns>
		[CLSCompliant(false)]
		public ushort ReadOptimizedUInt16()
		{
			return (ushort) ReadOptimizedUInt32();	
		}

		/// <summary>
		/// Returns a UInt32 value from the stream that was stored optimized.
		/// </summary>
		/// <returns>A UInt32 value.</returns>
		[CLSCompliant(false)]
		public uint ReadOptimizedUInt32()
		{
			uint result = 0;
			var bitShift = 0;

			while(true)
			{
				var nextByte = ReadByte();

				result |= ((uint) nextByte & 0x7f) << bitShift;
				bitShift += 7;

				if ((nextByte & 0x80) == 0) return result;
			}
		}

		/// <summary>
		/// Returns a UInt64 value from the stream that was stored optimized.
		/// </summary>
		/// <returns>A UInt64 value.</returns>
		[CLSCompliant(false)]
		public ulong ReadOptimizedUInt64()
		{
			ulong result = 0;
			var bitShift = 0;

			while(true)
			{
				var nextByte = ReadByte();

				result |= ((ulong) nextByte & 0x7f) << bitShift;
				bitShift += 7;

				if ((nextByte & 0x80) == 0) return result;
			}
		}

		/// <summary>
		/// Returns a typed array from the stream.
		/// </summary>
		/// <returns>A typed array.</returns>
		public Array ReadTypedArray()
		{
			return (Array) ProcessArrayTypes(ReadTypeCode(), null);
		}

		/// <summary>
		/// Returns a new, simple generic dictionary populated with keys and values from the stream.
		/// </summary>
		/// <typeparam name="K">The key Type.</typeparam>
		/// <typeparam name="V">The value Type.</typeparam>
		/// <returns>A new, simple, populated generic Dictionary.</returns>
		public Dictionary<K, V> ReadDictionary<K, V>()
		{
			var result = new Dictionary<K, V>();

			ReadDictionary(result);

			return result;
		}

		
		/// <summary>
		/// Populates a pre-existing generic dictionary with keys and values from the stream.
		/// This allows a generic dictionary to be created without using the default constructor.
		/// </summary>
		/// <typeparam name="K">The key Type.</typeparam>
		/// <typeparam name="V">The value Type.</typeparam>
		public void ReadDictionary<K, V>(Dictionary<K, V> dictionary)
		{
			var keys = (K[]) ProcessArrayTypes(ReadTypeCode(), typeof(K));
			var values = (V[]) ProcessArrayTypes(ReadTypeCode(), typeof(V));

			if (dictionary == null) 
			{
				dictionary = new Dictionary<K, V>(keys.Length);
			}

			for (var i = 0; i < keys.Length; i++)
			{
				dictionary.Add(keys[i], values[i]);
			}
		}

		/// <summary>
		/// Returns a generic List populated with values from the stream.
		/// </summary>
		/// <typeparam name="T">The list Type.</typeparam>
		/// <returns>A new generic List.</returns>
		public List<T> ReadList<T>()
		{
			return new List<T>((T[]) ProcessArrayTypes(ReadTypeCode(), typeof(T)));
		}
		
		/// <summary>
		/// Returns a Nullable struct from the stream.
		/// The value returned must be cast to the correct Nullable type.
		/// Synonym for ReadObject();
		/// </summary>
		/// <returns>A struct value or null</returns>
		public ValueType ReadNullable()
		{
			return (ValueType) ReadObject();
		}
		
		/// <summary>
		/// Returns a Nullable Boolean from the stream.
		/// </summary>
		/// <returns>A Nullable Boolean.</returns>
		public Boolean? ReadNullableBoolean()
		{
			return (bool?) ReadObject();
		}

		/// <summary>
		/// Returns a Nullable Byte from the stream.
		/// </summary>
		/// <returns>A Nullable Byte.</returns>
		public Byte? ReadNullableByte()
		{
			return (byte?) ReadObject();
		}

		/// <summary>
		/// Returns a Nullable Char from the stream.
		/// </summary>
		/// <returns>A Nullable Char.</returns>
		public Char? ReadNullableChar()
		{
			return (char?) ReadObject();
		}

		/// <summary>
		/// Returns a Nullable DateTime from the stream.
		/// </summary>
		/// <returns>A Nullable DateTime.</returns>
		public DateTime? ReadNullableDateTime()
		{
			return (DateTime?) ReadObject();
		}

		/// <summary>
		/// Returns a Nullable Decimal from the stream.
		/// </summary>
		/// <returns>A Nullable Decimal.</returns>
		public Decimal? ReadNullableDecimal()
		{
			return (decimal?) ReadObject();
		}

		/// <summary>
		/// Returns a Nullable Double from the stream.
		/// </summary>
		/// <returns>A Nullable Double.</returns>
		public Double? ReadNullableDouble()
		{
			return (double?) ReadObject();
		}

		/// <summary>
		/// Returns a Nullable Guid from the stream.
		/// </summary>
		/// <returns>A Nullable Guid.</returns>
		public Guid? ReadNullableGuid()
		{
			return (Guid?) ReadObject();
		}

		/// <summary>
		/// Returns a Nullable Int16 from the stream.
		/// </summary>
		/// <returns>A Nullable Int16.</returns>
		public Int16? ReadNullableInt16()
		{
			return (short?) ReadObject();
		}
		/// <summary>
		/// Returns a Nullable Int32 from the stream.
		/// </summary>
		/// <returns>A Nullable Int32.</returns>
		public Int32? ReadNullableInt32()
		{
			return (int?) ReadObject();
		}

		/// <summary>
		/// Returns a Nullable Int64 from the stream.
		/// </summary>
		/// <returns>A Nullable Int64.</returns>
		public Int64? ReadNullableInt64()
		{
			return (long?) ReadObject();
		}

		/// <summary>
		/// Returns a Nullable SByte from the stream.
		/// </summary>
		/// <returns>A Nullable SByte.</returns>
		[CLSCompliant(false)]
		public SByte? ReadNullableSByte()
		{
			return (sbyte?) ReadObject();
		}

		/// <summary>
		/// Returns a Nullable Single from the stream.
		/// </summary>
		/// <returns>A Nullable Single.</returns>
		public Single? ReadNullableSingle()
		{
			return (float?) ReadObject();
		}

		/// <summary>
		/// Returns a Nullable TimeSpan from the stream.
		/// </summary>
		/// <returns>A Nullable TimeSpan.</returns>
		public TimeSpan? ReadNullableTimeSpan()
		{
			return (TimeSpan?) ReadObject();
		}

		/// <summary>
		/// Returns a Nullable UInt16 from the stream.
		/// </summary>
		/// <returns>A Nullable UInt16.</returns>
		[CLSCompliant(false)]
		public UInt16? ReadNullableUInt16()
		{
			return (ushort?) ReadObject();
		}

		/// <summary>
		/// Returns a Nullable UInt32 from the stream.
		/// </summary>
		/// <returns>A Nullable UInt32.</returns>
		[CLSCompliant(false)]
		public UInt32? ReadNullableUInt32()
		{
			return (uint?) ReadObject();
		}

		/// <summary>
		/// Returns a Nullable UInt64 from the stream.
		/// </summary>
		/// <returns>A Nullable UInt64.</returns>
		[CLSCompliant(false)]
		public UInt64? ReadNullableUInt64()
		{
			return (ulong?) ReadObject();
		}

		/// <summary>
		/// Returns a Byte[] from the stream.
		/// </summary>
		/// <returns>A Byte instance; or null.</returns>
		public byte[] ReadByteArray()
		{
			switch (ReadTypeCode())
			{
				case SerializedType.NullType: return null;
				case SerializedType.EmptyTypedArrayType: return new byte[0];

				default: return ReadByteArrayInternal();
			}
		}

		/// <summary>
		/// Returns a Char[] from the stream.
		/// </summary>
		/// <returns>A Char[] value; or null.</returns>
		public char[] ReadCharArray()
		{
			switch (ReadTypeCode())
			{
				case SerializedType.NullType: return null;
				case SerializedType.EmptyTypedArrayType: return new char[0];

				default: return ReadCharArrayInternal();
			}
		}

		/// <summary>
		/// Returns a Double[] from the stream.
		/// </summary>
		/// <returns>A Double[] instance; or null.</returns>
		public double[] ReadDoubleArray()
		{
			switch (ReadTypeCode())
			{
				case SerializedType.NullType: return null;
				case SerializedType.EmptyTypedArrayType: return new double[0];
				
				default: return ReadDoubleArrayInternal();
			}
		}

		/// <summary>
		/// Returns a Guid[] from the stream.
		/// </summary>
		/// <returns>A Guid[] instance; or null.</returns>
		public Guid[] ReadGuidArray()
		{
			switch (ReadTypeCode())
			{
				case SerializedType.NullType: return null;
				case SerializedType.EmptyTypedArrayType: return new Guid[0];

				default: return ReadGuidArrayInternal();
			}
		}

		/// <summary>
		/// Returns an Int16[] from the stream.
		/// </summary>
		/// <returns>An Int16[] instance; or null.</returns>
		public short[] ReadInt16Array()
		{
			var t = ReadTypeCode();

			switch (t)
			{
				case SerializedType.NullType: return null;
				case SerializedType.EmptyTypedArrayType: return new short[0];

				default: 
					var optimizeFlags = ReadTypedArrayOptimizeFlags(t);
					var result = new short[ReadOptimizedInt32()];

					for (var i = 0; i < result.Length; i++)
					{
						if ((optimizeFlags == null) || ((optimizeFlags != FullyOptimizableTypedArray) && !optimizeFlags[i]))
						{
							result[i] = ReadInt16();
						}
						else
						{
							result[i] = ReadOptimizedInt16();
						}
					}

					return result;
			}
		}

		/// <summary>
		/// Returns an object[] or null from the stream.
		/// </summary>
		/// <returns>A DateTime value.</returns>
		public object[] ReadObjectArray()
		{
			return ReadObjectArray(null);
		}

		/// <summary>
		/// Returns an object[] or null from the stream.
		/// The returned array will be typed according to the specified element type
		/// and the resulting array can be cast to the expected type.
		/// e.g.
		/// string[] myStrings = (string[]) reader.ReadObjectArray(typeof(string));
		/// 
		/// An exception will be thrown if any of the deserialized values cannot be
		/// cast to the specified elementType.
		/// 
		/// </summary>
		/// <param name="elementType">The Type of the expected array elements. null will return a plain object[].</param>
		/// <returns>An object[] instance.</returns>
		public object[] ReadObjectArray(Type elementType)
		{
			switch (ReadTypeCode())
			{
				case SerializedType.NullType: return null;
				case SerializedType.EmptyObjectArrayType: return elementType == null ? new object[0] : (object[]) Array.CreateInstance(elementType, 0);
				case SerializedType.EmptyTypedArrayType: throw new Exception(string.Format("None of the deserialized values can be casted to the specified elementType: '{0}'", elementType.FullName));

				default: return ReadOptimizedObjectArray(elementType);
			}
		}

		/// <summary>
		/// Returns a Single[] from the stream.
		/// </summary>
		/// <returns>A Single[] instance; or null.</returns>
		public float[] ReadSingleArray()
		{
			switch (ReadTypeCode())
			{
				case SerializedType.NullType: return null;
				case SerializedType.EmptyTypedArrayType: return new float[0];

				default: return ReadSingleArrayInternal();
			}
		}

		/// <summary>
		/// Returns an SByte[] from the stream.
		/// </summary>
		/// <returns>An SByte[] instance; or null.</returns>
		[CLSCompliant(false)]
		public sbyte[] ReadSByteArray()
		{
			switch (ReadTypeCode())
			{
				case SerializedType.NullType: return null;
				case SerializedType.EmptyTypedArrayType: return new sbyte[0];

				default: return ReadSByteArrayInternal();
			}
		}

		/// <summary>
		/// Returns a string[] or null from the stream.
		/// </summary>
		/// <returns>An string[] instance.</returns>
		public string[] ReadStringArray()
		{
			return (string[]) ReadObjectArray(typeof(string));
		}

		/// <summary>
		/// Returns a UInt16[] from the stream.
		/// </summary>
		/// <returns>A UInt16[] instance; or null.</returns>
		[CLSCompliant(false)]
		public ushort[] ReadUInt16Array()
		{
			var typeCode = ReadTypeCode();

			switch (typeCode)
			{
				case SerializedType.NullType: return null;
				case SerializedType.EmptyTypedArrayType: return new ushort[0];

				default:
					var optimizeFlags = ReadTypedArrayOptimizeFlags(typeCode);
					var result = new ushort[ReadOptimizedUInt32()];

					for (var i = 0; i < result.Length; i++)
					{
						if ((optimizeFlags == null) || ((optimizeFlags != FullyOptimizableTypedArray) && !optimizeFlags[i]))
						{
							result[i] = ReadUInt16();
						}
						else
						{
							result[i] = ReadOptimizedUInt16();
						}
					}

					return result;
			}
		}

		/// <summary>
		/// Returns a Boolean[] from the stream.
		/// </summary>
		/// <returns>A Boolean[] instance; or null.</returns>
		public bool[] ReadBooleanArray()
		{
			switch (ReadTypeCode())
			{
				case SerializedType.NullType: return null;
				case SerializedType.EmptyTypedArrayType: return new bool[0];

				default: return ReadBooleanArrayInternal();
			}
		}

		/// <summary>
		/// Returns a DateTime[] from the stream.
		/// </summary>
		/// <returns>A DateTime[] instance; or null.</returns>
		public DateTime[] ReadDateTimeArray()
		{
			var typeCode = ReadTypeCode();
			switch (typeCode)
			{
				case SerializedType.NullType: return null;
				case SerializedType.EmptyTypedArrayType: return new DateTime[0];

				default:
					var optimizeFlags = ReadTypedArrayOptimizeFlags(typeCode);
					var result = new DateTime[ReadOptimizedInt32()];

					for (var i = 0; i < result.Length; i++)
					{
						if ((optimizeFlags == null) || ((optimizeFlags != FullyOptimizableTypedArray) && !optimizeFlags[i]))
						{
							result[i] = ReadDateTime();
						}
						else
						{
							result[i] = ReadOptimizedDateTime();
						}
					}

					return result;
			}
		}

		/// <summary>
		/// Returns a Decimal[] from the stream.
		/// </summary>
		/// <returns>A Decimal[] instance; or null.</returns>
		public decimal[] ReadDecimalArray()
		{
			switch (ReadTypeCode())
			{
				case SerializedType.NullType: return null;
				case SerializedType.EmptyTypedArrayType: return new decimal[0];

				default: return ReadDecimalArrayInternal();
			}
		}

		/// <summary>
		/// Returns an Int32[] from the stream.
		/// </summary>
		/// <returns>An Int32[] instance; or null.</returns>
		public int[] ReadInt32Array()
		{
			var typeCode = ReadTypeCode();

			switch (typeCode)
			{
				case SerializedType.NullType: return null;
				case SerializedType.EmptyTypedArrayType: return new int[0];

				default:
					var optimizeFlags = ReadTypedArrayOptimizeFlags(typeCode);
					var result = new int[ReadOptimizedInt32()];

					for (var i = 0; i < result.Length; i++)
					{
						if ((optimizeFlags == null) || ((optimizeFlags != FullyOptimizableTypedArray) && !optimizeFlags[i]))
						{
							result[i] = ReadInt32();
						}
						else
						{
							result[i] = ReadOptimizedInt32();
						}
					}

					return result;
			}
		}

		/// <summary>
		/// Returns an Int64[] from the stream.
		/// </summary>
		/// <returns>An Int64[] instance; or null.</returns>
		public long[] ReadInt64Array()
		{
			var typeCode = ReadTypeCode();

			switch (typeCode)
			{
				case SerializedType.NullType: return null;
				case SerializedType.EmptyTypedArrayType: return new long[0];

				default:
					var optimizeFlags = ReadTypedArrayOptimizeFlags(typeCode);
					var result = new long[ReadOptimizedInt64()];

					for (var i = 0; i < result.Length; i++)
					{
						if ((optimizeFlags == null) || ((optimizeFlags != FullyOptimizableTypedArray) && !optimizeFlags[i]))
						{
							result[i] = ReadInt64();
						}
						else
						{
							result[i] = ReadOptimizedInt64();
						}
					}

					return result;
			}
		}

		/// <summary>
		/// Returns a string[] from the stream that was stored optimized.
		/// </summary>
		/// <returns>An string[] instance.</returns>
		public string[] ReadOptimizedStringArray()
		{
			return (string[]) ReadOptimizedObjectArray(typeof(string));
		}

		/// <summary>
		/// Returns a TimeSpan[] from the stream.
		/// </summary>
		/// <returns>A TimeSpan[] instance; or null.</returns>
		public TimeSpan[] ReadTimeSpanArray()
		{
			var typeCode = ReadTypeCode();

			switch (typeCode)
			{
				case SerializedType.NullType: return null;
				case SerializedType.EmptyTypedArrayType: return new TimeSpan[0];

				default:
					var optimizeFlags = ReadTypedArrayOptimizeFlags(typeCode);
					var result = new TimeSpan[ReadOptimizedInt32()];

					for (var i = 0; i < result.Length; i++)
					{
						if ((optimizeFlags == null) || ((optimizeFlags != FullyOptimizableTypedArray) && !optimizeFlags[i]))
						{
							result[i] = ReadTimeSpan();
						}
						else
						{
							result[i] = ReadOptimizedTimeSpan();
						}
					}

					return result;
			}
		}

		/// <summary>
		/// Returns a UInt[] from the stream.
		/// </summary>
		/// <returns>A UInt[] instance; or null.</returns>
		[CLSCompliant(false)]
		public uint[] ReadUInt32Array()
		{
			var typeCode = ReadTypeCode();

			switch (typeCode)
			{
				case SerializedType.NullType: return null;
				case SerializedType.EmptyTypedArrayType: return new uint[0];

				default:
					var optimizeFlags = ReadTypedArrayOptimizeFlags(typeCode);
					var result = new uint[ReadOptimizedUInt32()];

					for (var i = 0; i < result.Length; i++)
					{
						if ((optimizeFlags == null) || ((optimizeFlags != FullyOptimizableTypedArray) && !optimizeFlags[i]))
						{
							result[i] = ReadUInt32();
						}
						else
						{
							result[i] = ReadOptimizedUInt32();
						}
					}

					return result;
			}
		}

		/// <summary>
		/// Returns a UInt64[] from the stream.
		/// </summary>
		/// <returns>A UInt64[] instance; or null.</returns>
		[CLSCompliant((false))]
		public ulong[] ReadUInt64Array()
		{
			var typeCode = ReadTypeCode();

			switch (typeCode)
			{
				case SerializedType.NullType: return null;
				case SerializedType.EmptyTypedArrayType: return new ulong[0];

				default:
					var optimizeFlags = ReadTypedArrayOptimizeFlags(typeCode);
					var result = new ulong[ReadOptimizedInt64()];

					for (var i = 0; i < result.Length; i++)
					{
						if ((optimizeFlags == null) || ((optimizeFlags != FullyOptimizableTypedArray) && !optimizeFlags[i]))
						{
							result[i] = ReadUInt64();
						}
						else
						{
							result[i] = ReadOptimizedUInt64();
						}
					}

					return result;
			}
		}

		/// <summary>
		/// Returns a Boolean[] from the stream.
		/// </summary>
		/// <returns>A Boolean[] instance; or null.</returns>
		public bool[] ReadOptimizedBooleanArray()
		{
			return ReadBooleanArray();
		}

		/// <summary>
		/// Returns a DateTime[] from the stream.
		/// </summary>
		/// <returns>A DateTime[] instance; or null.</returns>
		public DateTime[] ReadOptimizedDateTimeArray()
		{
			return ReadDateTimeArray();
		}

		/// <summary>
		/// Returns a Decimal[] from the stream.
		/// </summary>
		/// <returns>A Decimal[] instance; or null.</returns>
		public decimal[] ReadOptimizedDecimalArray()
		{
			return ReadDecimalArray();
		}

		/// <summary>
		/// Returns a Int16[] from the stream.
		/// </summary>
		/// <returns>An Int16[] instance; or null.</returns>
		public short[] ReadOptimizedInt16Array()
		{
			return ReadInt16Array();
		}

		/// <summary>
		/// Returns a Int32[] from the stream.
		/// </summary>
		/// <returns>An Int32[] instance; or null.</returns>
		public int[] ReadOptimizedInt32Array()
		{
			return ReadInt32Array();
		}

		/// <summary>
		/// Returns a Int64[] from the stream.
		/// </summary>
		/// <returns>A Int64[] instance; or null.</returns>
		public long[] ReadOptimizedInt64Array()
		{
			return ReadInt64Array();
		}

		/// <summary>
		/// Returns a TimeSpan[] from the stream.
		/// </summary>
		/// <returns>A TimeSpan[] instance; or null.</returns>
		public TimeSpan[] ReadOptimizedTimeSpanArray()
		{
			return ReadTimeSpanArray();
		}

		/// <summary>
		/// Returns a UInt16[] from the stream.
		/// </summary>
		/// <returns>A UInt16[] instance; or null.</returns>
		[CLSCompliant(false)]
		public ushort[] ReadOptimizedUInt16Array()
		{
			return ReadUInt16Array();
		}

		/// <summary>
		/// Returns a UInt32[] from the stream.
		/// </summary>
		/// <returns>A UInt32[] instance; or null.</returns>
		[CLSCompliant(false)]
		public uint[] ReadOptimizedUInt32Array()
		{
			return ReadUInt32Array();
		}

		/// <summary>
		/// Returns a UInt64[] from the stream.
		/// </summary>
		/// <returns>A UInt64[] instance; or null.</returns>
		[CLSCompliant(false)]
		public ulong[] ReadOptimizedUInt64Array()
		{
			return ReadUInt64Array();
		}

		/// <summary>
		/// Allows an existing object, implementing IOwnedDataSerializable, to 
		/// retrieve its owned data from the stream.
		/// </summary>
		/// <param name="target">Any IOwnedDataSerializable object.</param>
		/// <param name="context">An optional, arbitrary object to allow context to be provided.</param>
		public void ReadOwnedData(IOwnedDataSerializable target, object context)
		{
			target.DeserializeOwnedData(this, context);
		}

		/// <summary>
		/// Returns the object associated with the object token read next from the stream.
		/// </summary>
		/// <returns>An object.</returns>
		public object ReadTokenizedObject()
		{
			var token = ReadOptimizedInt32();

			if (token >= objectTokenList.Count)
			{
				var tokenizedObject = ReadObject();

				objectTokenList.Add(tokenizedObject);

				return tokenizedObject;
			}

			return objectTokenList[token];
		}

		/// <summary>
		/// Initializes the token tables.
		/// </summary>
		/// <param name="stringTokenTablePresize">The string token table presize.</param>
		/// <param name="objectTokenTablePresize">The object token table presize.</param>
		void InitializeTokenTables(int stringTokenTablePresize, int objectTokenTablePresize)
		{
			stringTokenList = new List<string>(stringTokenTablePresize);
			objectTokenList = new List<object>(objectTokenTablePresize);
		}

		/// <summary>
		/// Returns a TimeSpan decoded from packed data.
		/// This routine is called from ReadOptimizedDateTime() and ReadOptimizedTimeSpan().
		/// <remarks>
		/// This routine uses a parameter to allow ReadOptimizedDateTime() to 'peek' at the
		/// next byte and extract the DateTimeKind from bits one and two (IsNegative and HasDays)
		/// which are never set for a Time portion of a DateTime.
		/// </remarks>
		/// </summary>
		/// <param name="initialByte">The first of two always-present bytes.</param>
		/// <returns>A decoded TimeSpan</returns>
		TimeSpan DecodeTimeSpan(byte initialByte)
		{
			var packedData = new BitVector32(initialByte | (ReadByte() << 8)); // Read first two bytes
			var hasTime = packedData[SerializationWriter.HasTimeSection] == 1;
			var hasSeconds = packedData[SerializationWriter.HasSecondsSection] == 1;
			var hasMilliseconds = packedData[SerializationWriter.HasMillisecondsSection] == 1;
			long ticks = 0;

			if (hasMilliseconds)
			{
				packedData = new BitVector32(packedData.Data | (ReadByte() << 16) | (ReadByte() << 24));
			}
			else if (hasTime && hasSeconds)
			{
				packedData = new BitVector32(packedData.Data | (ReadByte() << 16));
			}

			if (hasTime)
			{
				ticks += packedData[SerializationWriter.HoursSection] * TimeSpan.TicksPerHour;
				ticks += packedData[SerializationWriter.MinutesSection] * TimeSpan.TicksPerMinute;
			}

			if (hasSeconds)
			{
				ticks += packedData[(!hasTime && !hasMilliseconds)
				                    	? SerializationWriter.MinutesSection
				                    	: SerializationWriter.SecondsSection] * TimeSpan.TicksPerSecond;
			}

			if (hasMilliseconds)
			{
				ticks += packedData[SerializationWriter.MillisecondsSection] * TimeSpan.TicksPerMillisecond;
			}

			if (packedData[SerializationWriter.HasDaysSection] == 1)
			{
				ticks += ReadOptimizedInt32() * TimeSpan.TicksPerDay;
			}

			if (packedData[SerializationWriter.IsNegativeSection] == 1)
			{
				ticks = -ticks;
			}

			return new TimeSpan(ticks);
		}

		/// <summary>
		/// Creates a BitArray representing which elements of a typed array
		/// are serializable.
		/// </summary>
		/// <param name="serializedType">The type of typed array.</param>
		/// <returns>A BitArray denoting which elements are serializable.</returns>
		BitArray ReadTypedArrayOptimizeFlags(SerializedType serializedType)
		{
			switch (serializedType)
			{
				case SerializedType.FullyOptimizedTypedArrayType: return FullyOptimizableTypedArray;
				case SerializedType.PartiallyOptimizedTypedArrayType: return ReadOptimizedBitArray();

				default:
					return null;
			}
		}

		/// <summary>
		/// Returns an object based on supplied SerializedType.
		/// </summary>
		/// <returns>An object instance.</returns>
		object ProcessObject(SerializedType typeCode)
		{
			if (typeCode < SerializedType.NullType) return ReadTokenizedString((int) typeCode);

			switch (typeCode)
			{
				case SerializedType.NullType: return null;
				case SerializedType.Int32Type: return ReadInt32();
				case SerializedType.EmptyStringType: return string.Empty;
				case SerializedType.BooleanFalseType: return false;
				case SerializedType.ZeroInt32Type: return 0;
				case SerializedType.OptimizedInt32Type: return ReadOptimizedInt32();
				case SerializedType.OptimizedInt32NegativeType: return -ReadOptimizedInt32() - 1;
				case SerializedType.DecimalType: return ReadOptimizedDecimal();
				case SerializedType.ZeroDecimalType: return (Decimal) 0;
				case SerializedType.YStringType: return "Y";
				case SerializedType.DateTimeType: return ReadDateTime();
				case SerializedType.OptimizedDateTimeType: return ReadOptimizedDateTime();
				case SerializedType.SingleCharStringType: return Char.ToString(ReadChar());
				case SerializedType.SingleSpaceType: return " ";
				case SerializedType.OneInt32Type: return 1;
				case SerializedType.OptimizedInt16Type: return ReadOptimizedInt16();
				case SerializedType.OptimizedInt16NegativeType: return (short)(-ReadOptimizedInt16() - 1);
				case SerializedType.OneDecimalType: return (Decimal) 1;
				case SerializedType.BooleanTrueType: return true;
				case SerializedType.NStringType: return "N";
				case SerializedType.DBNullType: return DBNull.Value;
				case SerializedType.ObjectArrayType: return ReadOptimizedObjectArray();
				case SerializedType.EmptyObjectArrayType: return new object[0];
				case SerializedType.MinusOneInt32Type: return -1;
				case SerializedType.MinusOneInt64Type: return (Int64) (-1);
				case SerializedType.MinusOneInt16Type: return (Int16) (-1);
				case SerializedType.MinDateTimeType: return DateTime.MinValue;
				case SerializedType.GuidType: return ReadGuid();
				case SerializedType.EmptyGuidType: return Guid.Empty;
				case SerializedType.TimeSpanType: return ReadTimeSpan();
				case SerializedType.MaxDateTimeType: return DateTime.MaxValue;
				case SerializedType.ZeroTimeSpanType: return TimeSpan.Zero;
				case SerializedType.OptimizedTimeSpanType: return ReadOptimizedTimeSpan();
				case SerializedType.DoubleType: return ReadDouble();
				case SerializedType.ZeroDoubleType: return (Double) 0;
				case SerializedType.Int64Type: return ReadInt64();
				case SerializedType.ZeroInt64Type: return (Int64) 0;
				case SerializedType.OptimizedInt64Type: return ReadOptimizedInt64();
				case SerializedType.OptimizedInt64NegativeType: return -ReadOptimizedInt64() - 1;
				case SerializedType.Int16Type: return ReadInt16();
				case SerializedType.ZeroInt16Type: return (Int16) 0;
				case SerializedType.SingleType: return ReadSingle();
				case SerializedType.ZeroSingleType: return (Single) 0;
				case SerializedType.ByteType: return ReadByte();
				case SerializedType.ZeroByteType: return (Byte) 0;
				case SerializedType.OtherType: return new BinaryFormatter().Deserialize(BaseStream);
				case SerializedType.UInt16Type: return ReadUInt16();
				case SerializedType.ZeroUInt16Type: return (UInt16) 0;
				case SerializedType.UInt32Type: return ReadUInt32();
				case SerializedType.ZeroUInt32Type: return (UInt32) 0;
				case SerializedType.OptimizedUInt32Type: return ReadOptimizedUInt32();
				case SerializedType.UInt64Type: return ReadUInt64();
				case SerializedType.ZeroUInt64Type: return (UInt64) 0;
				case SerializedType.OptimizedUInt64Type: return ReadOptimizedUInt64();
				case SerializedType.BitVector32Type: return ReadBitVector32();
				case SerializedType.CharType: return ReadChar();
				case SerializedType.ZeroCharType: return (Char) 0;
				case SerializedType.SByteType: return ReadSByte();
				case SerializedType.ZeroSByteType: return (SByte) 0;
				case SerializedType.OneByteType: return (Byte) 1;
				case SerializedType.OneDoubleType: return (Double) 1;
				case SerializedType.OneCharType: return (Char) 1;
				case SerializedType.OneInt16Type: return (Int16) 1;
				case SerializedType.OneInt64Type: return (Int64) 1;
				case SerializedType.OneUInt16Type: return (UInt16) 1;
				case SerializedType.OptimizedUInt16Type: return ReadOptimizedUInt16();
				case SerializedType.OneUInt32Type: return (UInt32) 1;
				case SerializedType.OneUInt64Type: return (UInt64) 1;
				case SerializedType.OneSByteType: return (SByte) 1;
				case SerializedType.OneSingleType: return (Single) 1;
				case SerializedType.BitArrayType: return ReadOptimizedBitArray();
				case SerializedType.TypeType: return Type.GetType(ReadOptimizedString(), false);
				case SerializedType.ArrayListType: return ReadOptimizedArrayList();
				case SerializedType.SingleInstanceType:
					try
					{
						return Activator.CreateInstance(Type.GetType(ReadStringDirect()), true);
					}
					catch
					{
						// cannot recover from this, swallow.
						return null;
					}

				case SerializedType.OwnedDataSerializableAndRecreatableType:
					{
						var result = Activator.CreateInstance(ReadOptimizedType());

						ReadOwnedData((IOwnedDataSerializable) result, null);

						return result;
					}

				case SerializedType.OptimizedEnumType:
					{
						var enumType = ReadOptimizedType();
						var underlyingType = Enum.GetUnderlyingType(enumType);

						if ((underlyingType == typeof(int)) || (underlyingType == typeof(uint)) ||
						    (underlyingType == typeof(long)) || (underlyingType == typeof(ulong)))
						{
							return Enum.ToObject(enumType, ReadOptimizedUInt64());
						}
						
						return Enum.ToObject(enumType, ReadUInt64());
					}

				case SerializedType.EnumType:
					{
						var enumType = ReadOptimizedType();
						var underlyingType = Enum.GetUnderlyingType(enumType);

						if (underlyingType == typeof(Int32)) return Enum.ToObject(enumType, ReadInt32());
						if (underlyingType == typeof(Byte)) return Enum.ToObject(enumType, ReadByte());
						if (underlyingType == typeof(Int16)) return Enum.ToObject(enumType, ReadInt16());
						if (underlyingType == typeof(UInt32)) return Enum.ToObject(enumType, ReadUInt32());
						if (underlyingType == typeof(Int64)) return Enum.ToObject(enumType, ReadInt64());
						if (underlyingType == typeof(SByte)) return Enum.ToObject(enumType, ReadSByte());
						if (underlyingType == typeof(UInt16)) return Enum.ToObject(enumType, ReadUInt16());
					
						return Enum.ToObject(enumType, ReadUInt64()); 
					}

				case SerializedType.SurrogateHandledType:
					{
						var serializedType = ReadOptimizedType();
						var typeSurrogate = SerializationWriter.FindSurrogateForType(serializedType);

						return typeSurrogate.Deserialize(this, serializedType);
					}

				default:
					{
						var result = ProcessArrayTypes(typeCode, null);
						if (result != null) return result;

						throw new InvalidOperationException("Unrecognized TypeCode: " + typeCode);
					}
			}
		}

		/// <summary>
		/// Determine whether the passed-in type code refers to an array type
		/// and deserializes the array if it is.
		/// Returns null if not an array type.
		/// </summary>
		/// <param name="typeCode">The SerializedType to check.</param>
		/// <param name="defaultElementType">The Type of array element; null if to be read from stream.</param>
		/// <returns></returns>
		object ProcessArrayTypes(SerializedType typeCode, Type defaultElementType)
		{
			switch (typeCode)
			{
				case SerializedType.StringArrayType: return ReadOptimizedStringArray();
				case SerializedType.Int32ArrayType: return ReadInt32Array();
				case SerializedType.Int64ArrayType: return ReadInt64Array();
				case SerializedType.DecimalArrayType: return ReadDecimalArrayInternal();
				case SerializedType.TimeSpanArrayType: return ReadTimeSpanArray();
				case SerializedType.UInt32ArrayType: return ReadUInt32Array();
				case SerializedType.UInt64ArrayType: return ReadUInt64Array();
				case SerializedType.DateTimeArrayType: return ReadDateTimeArray();
				case SerializedType.BooleanArrayType: return ReadBooleanArrayInternal();
				case SerializedType.ByteArrayType: return ReadByteArrayInternal();
				case SerializedType.CharArrayType: return ReadCharArrayInternal();
				case SerializedType.DoubleArrayType: return ReadDoubleArrayInternal();
				case SerializedType.SingleArrayType: return ReadSingleArrayInternal();
				case SerializedType.GuidArrayType: return ReadGuidArrayInternal();
				case SerializedType.SByteArrayType: return ReadSByteArrayInternal();
				case SerializedType.Int16ArrayType: return ReadInt16Array();
				case SerializedType.UInt16ArrayType: return ReadUInt16Array();
				case SerializedType.EmptyTypedArrayType: return Array.CreateInstance(defaultElementType ?? ReadOptimizedType(), 0);
				case SerializedType.OtherTypedArrayType: return ReadOptimizedObjectArray(ReadOptimizedType());
				case SerializedType.ObjectArrayType: return ReadOptimizedObjectArray(defaultElementType);
				case SerializedType.NonOptimizedTypedArrayType:
				case SerializedType.PartiallyOptimizedTypedArrayType:
				case SerializedType.FullyOptimizedTypedArrayType:
					var optimizeFlags = ReadTypedArrayOptimizeFlags(typeCode);
					var length = ReadOptimizedInt32();

					if (defaultElementType == null)
					{
						defaultElementType = ReadOptimizedType();
					}

					var result = Array.CreateInstance(defaultElementType, length);

					for (var i = 0; i < length; i++)
					{
						if (optimizeFlags == null)
						{
							result.SetValue(ReadObject(), i);
						}
						else if ((optimizeFlags == FullyOptimizableTypedArray) || !optimizeFlags[i])
						{
							var value = (IOwnedDataSerializable) Activator.CreateInstance(defaultElementType);

							ReadOwnedData(value, null);
							result.SetValue(value, i);
						}
					}

					return result;
			}

			return null;
		}

		/// <summary>
		/// Returns the string value associated with the string token read next from the stream.
		/// </summary>
		/// <returns>A DateTime value.</returns>
		string ReadTokenizedString(int bucket)
		{
			var stringTokenIndex = (ReadOptimizedInt32() << 7) + bucket;

			if (stringTokenIndex >= stringTokenList.Count)
			{
				stringTokenList.Add(base.ReadString());
			}

			return stringTokenList[stringTokenIndex];
		}
		/// <summary>
		/// Returns the SerializedType read next from the stream.
		/// </summary>
		/// <returns>A SerializedType value.</returns>
		SerializedType ReadTypeCode()
		{
			return (SerializedType) ReadByte();
		}

		/// <summary>
		/// Internal implementation returning a Bool[].
		/// </summary>
		/// <returns>A Bool[].</returns>
		bool[] ReadBooleanArrayInternal() 
		{
			var bitArray = ReadOptimizedBitArray();
			var result = new bool[bitArray.Count];

			for(var i = 0; i < result.Length; i++)
			{
				result[i] = bitArray[i];
			}

			return result;
		}

		/// <summary>
		/// Internal implementation returning a Byte[].
		/// </summary>
		/// <returns>A Byte[].</returns>
		byte[] ReadByteArrayInternal()
		{
			return ReadBytes(ReadOptimizedInt32());
		}

		/// <summary>
		/// Internal implementation returning a Char[].
		/// </summary>
		/// <returns>A Char[].</returns>
		char[] ReadCharArrayInternal()
		{
			return ReadChars(ReadOptimizedInt32());
		}

		/// <summary>
		/// Internal implementation returning a Decimal[].
		/// </summary>
		/// <returns>A Decimal[].</returns>
		decimal[] ReadDecimalArrayInternal()
		{
			var result = new decimal[ReadOptimizedInt32()];

			for(var i = 0; i < result.Length; i++)
			{
				result[i] = ReadOptimizedDecimal();
			}

			return result;
		}

		/// <summary>
		/// Internal implementation returning a Double[].
		/// </summary>
		/// <returns>A Double[].</returns>
		double[] ReadDoubleArrayInternal()
		{
			var result = new double[ReadOptimizedInt32()];

			for(var i = 0; i < result.Length; i++)
			{
				result[i] = ReadDouble();
			}

			return result;
		}

		/// <summary>
		/// Internal implementation returning a Guid[].
		/// </summary>
		/// <returns>A Guid[].</returns>
		Guid[] ReadGuidArrayInternal()
		{
			var result = new Guid[ReadOptimizedInt32()];

			for(var i = 0; i < result.Length; i++)
			{
				result[i] = ReadGuid();
			}

			return result;
		}

		/// <summary>
		/// Internal implementation returning an SByte[].
		/// </summary>
		/// <returns>An SByte[].</returns>
		sbyte[] ReadSByteArrayInternal()
		{
			var result = new sbyte[ReadOptimizedInt32()];

			for(var i = 0; i < result.Length; i++)
			{
				result[i] = ReadSByte();
			}

			return result;
		}

		/// <summary>
		/// Internal implementation returning a Single[].
		/// </summary>
		/// <returns>A Single[].</returns>
		float[] ReadSingleArrayInternal()
		{
			var result = new float[ReadOptimizedInt32()];

			for(var i = 0; i < result.Length; i++)
			{
				result[i] = ReadSingle();
			}

			return result;
		}

		#region Class Property declarations
		/// <summary>
		/// Returns the number of bytes or serialized remaining to be processed.
		/// Useful for checking that deserialization is complete.
		/// 
		/// Warning: Retrieving the Position in certain stream types can be expensive,
		/// e.g. a FileStream, so use sparingly unless known to be a MemoryStream.
		/// </summary>
		public int BytesRemaining
		{
			get { return endPosition == 0 ? -1 : endPosition - (int) BaseStream.Position; }
		}
		#endregion
	}
}
