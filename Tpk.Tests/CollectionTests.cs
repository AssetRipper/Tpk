using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AssetRipper.Tpk.Tests
{
	internal class CollectionTests
	{
		[Test]
		public void ReadAndWriteAreTheSame()
		{
			TpkCollectionBlob originalBlob = BlobCreator.MakeRandomCollectionBlob();
			byte[] writtenData = originalBlob.ToBinary();
			TpkCollectionBlob readBlob = TpkDataBlob.FromBinary<TpkCollectionBlob>(writtenData);
			AssertEquality(originalBlob.Blobs, readBlob.Blobs);
		}

		private static void AssertEquality(List<KeyValuePair<string, TpkDataBlob>> expected, List<KeyValuePair<string, TpkDataBlob>> actual)
		{
			if(expected.Count != actual.Count)
			{
				Assert.Fail($"Expected length {expected.Count} does not match actual length {actual.Count}");
			}

			for (int i = 0; i < expected.Count; i++)
			{
				if(actual[i].Key != expected[i].Key)
				{
					Assert.Fail($"Names inequal at index {i}\nExpected: {expected[i].Key}\nActual: {actual[i].Key}");
				}
				if (expected[i].Value.GetType() != actual[i].Value.GetType())
				{
					Assert.Fail($"Blob types inequal at index {i}\nExpected type: {expected[i].Value.GetType()}\nActual type: {actual[i].Value.GetType()}");
				}
				byte[] expectedData = expected[i].Value.ToBinary();
				byte[] actualData = actual[i].Value.ToBinary();
				if(!Enumerable.SequenceEqual(expectedData, actualData))
				{
					Assert.Fail($"Blob with type {expected[i].Value.GetType()} inequal at index {i}");
				}
			}
		}
	}
}
