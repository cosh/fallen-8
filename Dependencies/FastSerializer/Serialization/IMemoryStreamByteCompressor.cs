using System.IO;

namespace Framework.Serialization
{
	/// <summary>
	/// Interface to implement on specialized compressor classes to compress a passed-in memory stream
	/// </summary>
	public interface IMemoryStreamByteCompressor: IByteCompressor
	{
		/// <summary>
		/// Compresses the specified memory stream.
		/// </summary>
		/// <param name="memoryStream">The memory stream.</param>
		/// <returns>the data in the memory stream in compressed format.</returns>
		byte[] Compress(MemoryStream memoryStream);
	}
}