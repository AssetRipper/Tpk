using K4os.Compression.LZ4;
using K4os.Compression.LZ4.Streams;

namespace AssetRipper.TpkCreation
{
	internal static class Lz4Codec2
	{
		public static byte[] Compress(byte[] data)
		{
			LZ4EncoderSettings settings = new LZ4EncoderSettings();
			settings.CompressionLevel = LZ4Level.L12_MAX;
			settings.ChainBlocks = false;
			settings.BlockSize = 65536 * 16;
			using MemoryStream inputStream = new MemoryStream(data);
			using MemoryStream outputStream = new MemoryStream();
			using LZ4EncoderStream encoderStream = LZ4Stream.Encode(outputStream, settings, true);
			inputStream.CopyTo(encoderStream);
			return outputStream.ToArray();
		}
	}
}
