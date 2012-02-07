using System;
using System.Collections.Specialized;
using System.Drawing;

namespace Framework.Serialization
{
	public class DrawingFastSerializationHelper: IFastSerializationTypeSurrogate
	{
		static readonly int ColorIsKnown = BitVector32.CreateMask();
		static readonly int ColorHasName = BitVector32.CreateMask(ColorIsKnown);
		static readonly int ColorHasValue = BitVector32.CreateMask(ColorHasName);
		static readonly int ColorHasRed = BitVector32.CreateMask(ColorHasValue);
		static readonly int ColorHasGreen = BitVector32.CreateMask(ColorHasRed);
		static readonly int ColorHasBlue = BitVector32.CreateMask(ColorHasGreen);
		static readonly int ColorHasAlpha = BitVector32.CreateMask(ColorHasBlue);

		#region IFastSerialization
		public bool SupportsType(Type type)
		{
			return type == typeof(Color);
		}

		public void Serialize(SerializationWriter writer, object value)
		{
			var type = value.GetType();

			if (type == typeof(Color))
			{
				Serialize(writer, (Color) value);
			}
			else
			{
				throw new InvalidOperationException(string.Format("{0} does not support Type: {1}", GetType(), type));
			}
		}

		public object Deserialize(SerializationReader reader, Type type)
		{
			if (type == typeof(Color)) return DeserializeColor(reader);

			throw new InvalidOperationException(string.Format("{0} does not support Type: {1}", GetType(), type));
		}
		#endregion IFastSerialization

		#region Color
		public static void Serialize(SerializationWriter writer, Color color)
		{
			var flags = new BitVector32();

			if (color.IsKnownColor)
			{
				flags[ColorIsKnown] = true;
			}
			else if (color.IsNamedColor)
			{
				flags[ColorHasName] = true;
			}
			else if (!color.IsEmpty)
			{
				flags[ColorHasValue] = true;
				flags[ColorHasRed] = color.R != 0;
				flags[ColorHasGreen] = color.G != 0;
				flags[ColorHasBlue] = color.B != 0;
				flags[ColorHasAlpha] = color.A != 0;
			}
			writer.WriteOptimized(flags);

			if (color.IsKnownColor)
			{
				writer.WriteOptimized((int) color.ToKnownColor());
			}
			else if (color.IsNamedColor)
			{
				writer.WriteOptimized(color.Name);
			}
			else if (!color.IsEmpty)
			{
				byte component;
				if ( (component = color.R) != 0) writer.Write(component);	
				if ( (component = color.G) != 0) writer.Write(component);	
				if ( (component = color.B) != 0) writer.Write(component);	
				if ( (component = color.A) != 0) writer.Write(component);	
			}
		}

		public static Color DeserializeColor(SerializationReader reader)
		{
			var flags = reader.ReadOptimizedBitVector32();

			if (flags[ColorIsKnown]) return Color.FromKnownColor((KnownColor) reader.ReadOptimizedInt32());
			if (flags[ColorHasName]) return Color.FromName(reader.ReadOptimizedString());
			if (!flags[ColorHasValue]) return Color.Empty;

			var red = flags[ColorHasRed] ? reader.ReadByte() : (byte) 0;
			var green = flags[ColorHasGreen] ? reader.ReadByte() : (byte) 0;
			var blue = flags[ColorHasBlue] ? reader.ReadByte() : (byte) 0;
			var alpha = flags[ColorHasAlpha] ? reader.ReadByte() : (byte) 0;

			return Color.FromArgb(alpha, red, green, blue);
		}
		#endregion Color
	}
}