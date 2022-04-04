using AssetRipper.Tpk.TypeTrees;

namespace AssetRipper.Tpk.Tests
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
				blob.Add(name, data);
			}
		}

		internal static TpkCollectionBlob MakeRandomCollectionBlob()
		{
			TpkCollectionBlob collection = new TpkCollectionBlob();
			collection.Add(RandomUtils.RandomString(), new TpkTypeTreeBlob());
			collection.Add(RandomUtils.RandomString(), MakeRandomFileSystemBlob());
			collection.Add(RandomUtils.RandomString(), new TpkCollectionBlob());
			collection.Add(RandomUtils.RandomString(), MakeRandomFileSystemBlob());
			collection.Add(RandomUtils.RandomString(), MakeRandomFileSystemBlob());
			collection.Add(RandomUtils.RandomString(), new TpkTypeTreeBlob());

			TpkCollectionBlob subCollection = new TpkCollectionBlob();
			subCollection.Add(RandomUtils.RandomString(), new TpkTypeTreeBlob());
			subCollection.Add(RandomUtils.RandomString(), MakeRandomFileSystemBlob());
			collection.Add(RandomUtils.RandomString(), subCollection);

			collection.Add(RandomUtils.RandomString(), MakeRandomFileSystemBlob());

			return collection;
		}
	}
}
