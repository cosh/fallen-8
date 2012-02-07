namespace Framework.Serialization
{
	/// <summary>
	/// Interface to implement on a compressor class which can be used to compress/decompress the resulting byte array of the Fast serializer. 
	/// </summary>
	public interface IByteCompressor
	{
		/// <summary>
		/// Compresses the specified serialized data.
		/// </summary>
		/// <param name="serializedData">The serialized data.</param>
		/// <returns>The  passed in serialized data in compressed form</returns>
		byte[] Compress(byte[] serializedData);

		/// <summary>
		/// Decompresses the specified compressed data.
		/// </summary>
		/// <param name="compressedData">The compressed data.</param>
		/// <returns>The  passed in de-serialized data in compressed form</returns>
		byte[] Decompress(byte[] compressedData);
	}
}