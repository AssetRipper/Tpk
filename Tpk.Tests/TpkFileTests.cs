using NUnit.Framework;

namespace AssetRipper.Tpk.Tests
{
	internal class TpkFileTests
	{
		[Test]
		public void MagicBytesAreCorrect()
		{
			TpkFile file = CreateTpkFile();
			byte[] data = file.WriteToMemory();
			Assert.Multiple(() =>
			{
				Assert.That(data[0], Is.EqualTo((byte)'T'));
				Assert.That(data[1], Is.EqualTo((byte)'P'));
				Assert.That(data[2], Is.EqualTo((byte)'K'));
				Assert.That(data[3], Is.EqualTo((byte)'*'));
			});
		}

		private static TpkFile CreateTpkFile()
		{
			return TpkFile.FromBlob(new TpkCollectionBlob(), TpkCompressionType.None);
		}
	}
}
