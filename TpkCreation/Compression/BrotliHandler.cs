using System.IO.Compression;

namespace AssetRipper.TpkCreation.Compression
{
	internal static class BrotliHandler
	{
		public static byte[] Compress(byte[] uncompressedBytes)
		{
			using MemoryStream inputStream = new MemoryStream(uncompressedBytes);
			using MemoryStream outputStream = new MemoryStream();
			using BrotliStream compressionStream = new BrotliStream(outputStream, CompressionLevel.SmallestSize);
			inputStream.CopyTo(compressionStream);
			compressionStream.Close();
			return outputStream.ToArray();
		}

		public static byte[] Decompress(byte[] compressedBytes)
		{
			using MemoryStream inputStream = new MemoryStream(compressedBytes);
			using BrotliStream decompressionStream = new BrotliStream(inputStream, CompressionMode.Decompress);
			using MemoryStream outputStream = new MemoryStream();
			decompressionStream.CopyTo(outputStream);
			return outputStream.ToArray();
		}
	}
}
