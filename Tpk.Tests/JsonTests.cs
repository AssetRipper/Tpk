using NUnit.Framework;

namespace AssetRipper.Tpk.Tests
{
	public class JsonTests
	{
		[Test]
		public void ReadAndWriteAreTheSame()
		{
			TpkJsonBlob originalBlob = new TpkJsonBlob();
			originalBlob.Text = RandomUtils.RandomString(557);
			byte[] writtenData = originalBlob.ToBinary();
			TpkJsonBlob readBlob = TpkDataBlob.FromBinary<TpkJsonBlob>(writtenData);
			Assert.AreEqual(originalBlob.Text, readBlob.Text);
		}

		[Test]
		public void EmptyReadAndWriteAreTheSame()
		{
			TpkJsonBlob originalBlob = new TpkJsonBlob();
			byte[] writtenData = originalBlob.ToBinary();
			TpkJsonBlob readBlob = TpkDataBlob.FromBinary<TpkJsonBlob>(writtenData);
			Assert.AreEqual(originalBlob.Text, readBlob.Text);
		}
	}
}
