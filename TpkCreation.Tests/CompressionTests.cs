using AssetRipper.TpkCreation.Compression;
using NUnit.Framework;

namespace AssetRipper.TpkCreation.Tests
{
	public class CompressionTests
	{
		private readonly byte[] randomData = RandomUtils.RandomBytes(4096);

		[Test]
		public void Lz4CompressionReversesPerfectly()
		{
			byte[] compressedData = Lz4Handler.Compress(randomData);
			byte[] decompressedData = Lz4Handler.Decompress(compressedData, randomData.Length);
			Assert.AreEqual(randomData, decompressedData);
		}
	}
}
