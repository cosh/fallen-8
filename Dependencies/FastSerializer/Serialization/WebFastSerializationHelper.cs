using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Framework.Serialization
{
	public class WebFastSerializationHelper: IFastSerializationTypeSurrogate
	{
		#region Static
		public const double Epsilon = 2.2204460492503131e-016;

		internal static readonly int StateBagIsIgnoreCase = BitVector32.CreateMask();
		internal static readonly int StateBagHasDirtyEntries = BitVector32.CreateMask(StateBagIsIgnoreCase);
		internal static readonly int StateBagHasCleanEntries = BitVector32.CreateMask(StateBagHasDirtyEntries);

		internal static readonly BitVector32.Section UnitType = BitVector32.CreateSection(9); // 4 bits
		internal static readonly BitVector32.Section UnitIsZeroValue = BitVector32.CreateSection(1, UnitType);
		internal static readonly BitVector32.Section UnitIsNegativeValue = BitVector32.CreateSection(1, UnitIsZeroValue);
		internal static readonly BitVector32.Section UnitIsOptimizedValue = BitVector32.CreateSection(1, UnitIsNegativeValue);

		// Since Zero and Negative are mutually exclusive, we can use their combined masks and offsets 
		// as a Pseudo-BitVector32.Section rather than use an explicit UnitIsDouble BitVector32.Section which would 
		// require an extra bit and cause the BitVector32 to be written as 2 bytes depending on the value
		internal static readonly int UnitIsDoubleValue = UnitIsZeroValue.Mask << UnitIsZeroValue.Offset |
		                                                 UnitIsNegativeValue.Mask << UnitIsNegativeValue.Offset;

		internal static readonly FieldInfo StateBagIgnoreCaseField =
			typeof(StateBag).GetField("ignoreCase", BindingFlags.Instance | BindingFlags.NonPublic);
		#endregion Static

		#region IFastSerializationTypeSurrogate
		public bool SupportsType(Type type)
		{
			if (type == typeof(Pair)) return true;
			if (type == typeof(Triplet)) return true;
			if (type == typeof(StateBag)) return true;
			if (type == typeof(Unit)) return true;
			if (type == typeof(Hashtable)) return true;

			return false;
		}

		public void Serialize(SerializationWriter writer, object value)
		{
			var type = value.GetType();

			if (type == typeof(Pair))
			{
				Serialize(writer, (Pair) value);
			}
			else if (type == typeof(Triplet))
			{
				Serialize(writer, (Triplet) value);
			}
			else if (type == typeof(StateBag))
			{
				Serialize(writer, (StateBag) value);
			}
			else if (type == typeof(Unit))
			{
				Serialize(writer, (Unit) value);
			}
			else if (type == typeof(Hashtable))
			{
				Serialize(writer, (Hashtable) value);
			}
			else
			{
				throw new InvalidOperationException(string.Format("{0} does not support Type: {1}", GetType(), type));
			}
		}

		public object Deserialize(SerializationReader reader, Type type)
		{
			if (type == typeof(Pair)) return DeserializePair(reader);
			if (type == typeof(Triplet)) return DeserializeTriplet(reader);
			if (type == typeof(StateBag)) return DeserializeStateBag(reader);
			if (type == typeof(Unit)) return DeserializeUnit(reader);
			if (type == typeof(Hashtable)) return DeserializeHashtable(reader);

			throw new InvalidOperationException(string.Format("{0} does not support Type: {1}", GetType(), type));
		}
		#endregion IFastSerializationTypeSurrogate

		#region Unit
		public static void Serialize(SerializationWriter writer, Unit unit)
		{
			var state = new BitVector32();

			if (unit.IsEmpty)
			{
				writer.WriteOptimized(state);
				return;
			}

			state[UnitType] = (int) unit.Type;

			var integerValue = (int) Math.Round(unit.Value);
			var writeAsDouble = unit.Type != System.Web.UI.WebControls.UnitType.Pixel &&
			                    Math.Abs(unit.Value - integerValue) > Epsilon;

			if (writeAsDouble)
			{
				state[UnitIsDoubleValue] = true;
				writer.WriteOptimized(state);
				writer.Write(unit.Value);
			}
			else if (integerValue == 0)
			{
				state[UnitIsZeroValue] = 1;
				writer.WriteOptimized(state);
			}
			else
			{
				var complementedValue = integerValue > 0 ? integerValue : -(integerValue + 1);

				if (complementedValue > SerializationWriter.HighestOptimizable16BitValue)
				{
					writer.WriteOptimized(state);
					writer.Write((short) integerValue);
				}
				else
				{
					state[UnitIsOptimizedValue] = 1;
					state[UnitIsNegativeValue] = integerValue > 0 ? 0 : 1;
					writer.WriteOptimized(state);
					writer.WriteOptimized(complementedValue);
				}
			}
		}

		public static Unit DeserializeUnit(SerializationReader reader)
		{
			var state = reader.ReadOptimizedBitVector32();

			var unitType = state[UnitType];

			if (unitType == 0) return Unit.Empty;
			if (state[UnitIsDoubleValue]) return new Unit(reader.ReadDouble(), (UnitType) unitType);
			if (state[UnitIsZeroValue] == 1) return new Unit(0, (UnitType) unitType);
			
			var integerValue = state[UnitIsOptimizedValue] == 1 ? reader.ReadOptimizedInt32() : reader.ReadInt16();
			if (state[UnitIsNegativeValue] == 1) integerValue = -integerValue - 1;

			return new Unit(integerValue, (UnitType) unitType);
		}
		#endregion Unit

		#region StateBag
		public static void Serialize(SerializationWriter writer, StateBag stateBag)
		{
			var dirtyEntries = new List<DictionaryEntry>();
			var cleanEntries = new List<DictionaryEntry>();

			var flags = new BitVector32();
			flags[StateBagIsIgnoreCase] = (bool) StateBagIgnoreCaseField.GetValue(stateBag);

			foreach(DictionaryEntry entry in stateBag)
			{
				var stateItem = (StateItem) entry.Value;

				if (stateItem.IsDirty)
				{
					dirtyEntries.Add(entry);
				}
				else
				{
					cleanEntries.Add(entry);
				}
			}

			flags[StateBagHasDirtyEntries] = dirtyEntries.Count != 0;
			flags[StateBagHasCleanEntries] = cleanEntries.Count != 0;

			writer.WriteOptimized(flags);

			if (dirtyEntries.Count != 0)
			{
				writer.WriteOptimized(dirtyEntries.Count);

				foreach(var entry in dirtyEntries)
				{
					writer.WriteOptimized((string) entry.Key);
					writer.WriteObject(((StateItem) entry.Value).Value);
				}
			}

			if (cleanEntries.Count != 0)
			{
				writer.WriteOptimized(cleanEntries.Count);

				foreach(var entry in cleanEntries)
				{
					writer.WriteOptimized((string) entry.Key);
					writer.WriteObject(((StateItem) entry.Value).Value);
				}
			}
		}

		public static StateBag DeserializeStateBag(SerializationReader reader)
		{
			var flags = reader.ReadOptimizedBitVector32();
			var stateBag = new StateBag(flags[StateBagIsIgnoreCase]);

			if (flags[StateBagHasDirtyEntries])
			{
				var count = reader.ReadOptimizedInt32();

				for(var i = 0; i < count; i++)
				{
					var key = reader.ReadOptimizedString();
					var value = reader.ReadObject();

// ReSharper disable PossibleNullReferenceException
					stateBag.Add(key, value).IsDirty = true;
// ReSharper restore PossibleNullReferenceException
				}
			}

			if (flags[StateBagHasCleanEntries]) 
			{
				var count = reader.ReadOptimizedInt32();

				for(var i = 0; i < count; i++)
				{
					var key = reader.ReadOptimizedString();
					var value = reader.ReadObject();

					stateBag.Add(key, value);
				}
			}
			return stateBag;
		}

		#endregion StateBag

		#region Triplet
		public static void Serialize(SerializationWriter writer, Triplet triplet)
		{
			writer.WriteObject(triplet.First);
			writer.WriteObject(triplet.Second);
			writer.WriteObject(triplet.Third);
		}

		public static Triplet DeserializeTriplet(SerializationReader reader)
		{
			return new Triplet(reader.ReadObject(), reader.ReadObject(), reader.ReadObject());
		}
		#endregion Triplet

		#region Pair
		public static void Serialize(SerializationWriter writer, Pair pair)
		{
			writer.WriteObject(pair.First);
			writer.WriteObject(pair.Second);
		}

		public static Pair DeserializePair(SerializationReader reader)
		{
			return new Pair(reader.ReadObject(), reader.ReadObject());
		}
		#endregion Pair

		#region Hashtable
		// Note this is a simplistic version as it assumes defaults for comparer, hashcodeprovider, loadfactor etc.
		public static void Serialize(SerializationWriter writer, Hashtable hashtable) 
		{
			var keys = new object[hashtable.Count];
			var values = new object[hashtable.Count];

			hashtable.Keys.CopyTo(keys, 0);
			hashtable.Values.CopyTo(values, 0);

			writer.WriteOptimized(keys);
			writer.WriteOptimized(values);
		}

		// Note this is a simplistic version as it assumes defaults for comparer, hashcodeprovider, loadfactor etc.
		public static Hashtable DeserializeHashtable(SerializationReader reader)
		{
			var keys = reader.ReadOptimizedObjectArray();
			var values = reader.ReadOptimizedObjectArray();
			var result = new Hashtable(keys.Length);

			for(var i = 0; i < keys.Length; i++)
			{
				result[keys[i]] = values[i];
			}

			return result;
		}
		#endregion Hashtable

	}
}