using AssetRipper.Primitives;
using AssetRipper.Tpk.TypeTrees;
using NUnit.Framework;
using System.IO;

namespace AssetRipper.Tpk.Tests.TypeTrees
{
	internal static class CommonStringTests
	{
		private static UnityVersion Unity3 => new UnityVersion(3, 0, 0, UnityVersionType.Final, 1);
		private static UnityVersion Unity4 => new UnityVersion(4, 0, 0, UnityVersionType.Final, 1);
		private static UnityVersion Unity5 => new UnityVersion(5, 0, 0, UnityVersionType.Final, 1);
		private static UnityVersion Unity6 => new UnityVersion(6000, 0, 0, UnityVersionType.Final, 1);
		private static UnityVersion Unity7 => new UnityVersion(7000, 0, 0, UnityVersionType.Final, 1);

		[Test]
		public static void CountIsCorrectForEmptyCommonString()
		{
			TpkCommonString commonString = new();
			Assert.That(commonString.GetEntries(Unity5), Is.Empty);
		}

		[Test]
		public static void CountIsCorrectForNormalUse()
		{
			TpkCommonString commonString = MakeCommonString();
			using (Assert.EnterMultipleScope())
			{
				Assert.That(commonString.GetEntries(Unity3), Has.Length.EqualTo(5));
				Assert.That(commonString.GetEntries(Unity4), Has.Length.EqualTo(5));
				Assert.That(commonString.GetEntries(Unity5), Has.Length.EqualTo(5));
				Assert.That(commonString.GetEntries(Unity6), Has.Length.EqualTo(10));
				Assert.That(commonString.GetEntries(Unity7), Has.Length.EqualTo(10));
			}
		}

		[Test]
		public static void ReadingAndWritingUseSameBytes()
		{
			TpkCommonString commonString = MakeCommonString();
			MemoryStream stream = new();
			BinaryWriter writer = new(stream);
			commonString.Write(writer);
			writer.Flush();
			int bytesWritten = (int)stream.Length;

			stream.Position = 0;
			BinaryReader reader = new(stream);
			TpkCommonString readCommonString = new();
			readCommonString.Read(reader);
			int bytesRead = (int)stream.Position;

			Assert.That(bytesRead, Is.EqualTo(bytesWritten));
		}

		private static TpkCommonString MakeCommonString()
		{
			TpkCommonString commonString = new();
			commonString.Add(Unity4, new TpkCommonString.Entry[5]);
			commonString.Add(Unity6, new TpkCommonString.Entry[10]);
			return commonString;
		}
	}
}
