﻿using K4os.Compression.LZ4;

namespace AssetRipper.Tpk.Compression
{
	internal static class Lz4Handler
	{
		public static byte[] Decompress(byte[] compressedBytes, int decompressedSize)
		{
			byte[] decompressedBytes = new byte[decompressedSize];
			LZ4Codec.Decode(compressedBytes, decompressedBytes);
			return decompressedBytes;
		}

		public static byte[] Compress(byte[] uncompressedBytes)
		{
			byte[] buffer = new byte[LZ4Codec.MaximumOutputSize(uncompressedBytes.Length)];
			int compressedSize = LZ4Codec.Encode(uncompressedBytes, buffer, LZ4Level.L12_MAX);

			if (compressedSize < 0)
				throw new Exception("Could not compress data");

			byte[] compressedBytes = new byte[compressedSize];
			Array.Copy(buffer, compressedBytes, compressedSize);
			return compressedBytes;
		}
	}
}
