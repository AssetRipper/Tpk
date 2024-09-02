using NUnit.Framework;

namespace AssetRipper.Tpk.Tests
{
	public class FileSystemTests
	{
		[Test]
		public void ReadAndWriteAreTheSame()
		{
			TpkFileSystemBlob originalBlob = BlobCreator.MakeRandomFileSystemBlob();
			byte[] writtenData = originalBlob.ToBinary();
			TpkFileSystemBlob readBlob = TpkDataBlob.FromBinary<TpkFileSystemBlob>(writtenData);
			Assert.That(readBlob.Files, Is.EqualTo(originalBlob.Files));
		}
	}
}