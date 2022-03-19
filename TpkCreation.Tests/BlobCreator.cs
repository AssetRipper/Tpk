using System.Collections.Generic;

namespace AssetRipper.TpkCreation.Tests
{
	internal static class BlobCreator
	{
		public static TpkFileSystemBlob MakeRandomFileSystemBlob()
		{
			TpkFileSystemBlob blob = new TpkFileSystemBlob();
			blob.AddRandomEntries(10);
			return blob;
		}

		private static void AddRandomEntries(this TpkFileSystemBlob blob, int count)
		{
			for (int i = 0; i < count; i++)
			{
				string name = RandomUtils.RandomString();
				byte[] data = RandomUtils.RandomBytes();
				blob.Files.Add(new KeyValuePair<string, byte[]>(name, data));
			}
		}
	}
}
