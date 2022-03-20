using SharpCompress.Compressors.LZMA;

namespace AssetRipper.TpkCreation.Compression
{
	internal static class LzmaHandler
	{
		public static byte[] Compress(byte[] uncompressedBytes)
		{
			LzmaEncoderProperties properties = new LzmaEncoderProperties(false, 1 << 25, 256);
			using MemoryStream inputStream = new MemoryStream(uncompressedBytes);
			using MemoryStream outputStream = new MemoryStream();
			using LzmaStream lzmaStream = new LzmaStream(properties, false, outputStream);
			outputStream.Write(lzmaStream.Properties);
			inputStream.CopyTo(lzmaStream);
			lzmaStream.Close();
			return outputStream.ToArray();
		}

		public static byte[] Decompress(byte[] compressedBytes, int decompressedSize)
		{
			if(compressedBytes.Length < 5)
				throw new ArgumentException($"Compressed size {compressedBytes.Length} cannot be less than 5", nameof(compressedBytes));

			byte[] properties = new Span<byte>(compressedBytes, 0, 5).ToArray();
			using MemoryStream inputStream = new MemoryStream(compressedBytes);
			inputStream.Position = 5;
			using MemoryStream outputStream = new MemoryStream();
			using LzmaStream lzmaStream = new LzmaStream(properties, inputStream, compressedBytes.Length - 5, decompressedSize);
			lzmaStream.CopyTo(outputStream);
			return outputStream.ToArray();
		}
	}
}
