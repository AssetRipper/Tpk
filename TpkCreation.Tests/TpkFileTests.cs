using NUnit.Framework;

namespace AssetRipper.TpkCreation.Tests
{
	internal class TpkFileTests
	{
		[Test]
		public void MagicBytesAreCorrect()
		{
			TpkFile file = CreateTpkFile();
			byte[] data = file.WriteToMemory();
			Assert.AreEqual((byte)'C', data[0]);
			Assert.AreEqual((byte)'L', data[1]);
			Assert.AreEqual((byte)'P', data[2]);
			Assert.AreEqual((byte)'K', data[3]);
		}

		private static TpkFile CreateTpkFile()
		{
			return new TpkFile(new TpkCollectionBlob(), TpkCompressionType.None);
		}
	}
}
