using NUnit.Framework;
using System.Collections.Generic;

namespace AssetRipper.TpkCreation.Tests
{
	public class FileSystemTests
	{
		[Test]
		public void ReadAndWriteAreTheSame()
		{
			TpkFileSystemBlob originalBlob = MakeRandomBlob();
			byte[] writtenData = originalBlob.ToBinary();
			TpkFileSystemBlob readBlob = TpkDataBlob.FromBinary<TpkFileSystemBlob>(writtenData);
			Assert.AreEqual(originalBlob.Files, readBlob.Files);
		}

		private static TpkFileSystemBlob MakeRandomBlob()
		{
			TpkFileSystemBlob blob = new TpkFileSystemBlob();
			AddRandomEntries(blob, 10);
			return blob;
		}

		private static void AddRandomEntries(TpkFileSystemBlob blob, int count)
		{
			for(int i = 0; i < count; i++)
			{
				string name = RandomUtils.RandomString();
				byte[] data = RandomUtils.RandomBytes();
				blob.Files.Add(new KeyValuePair<string, byte[]>(name, data));
			}
		}
	}
}