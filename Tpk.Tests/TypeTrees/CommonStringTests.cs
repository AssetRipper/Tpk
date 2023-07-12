using AssetRipper.Tpk.TypeTrees;
using AssetRipper.Primitives;
using NUnit.Framework;

namespace AssetRipper.Tpk.Tests.TypeTrees
{
	internal static class CommonStringTests
	{
		private static UnityVersion Unity3 => new UnityVersion(3, 0, 0, UnityVersionType.Final, 1);
		private static UnityVersion Unity4 => new UnityVersion(4, 0, 0, UnityVersionType.Final, 1);
		private static UnityVersion Unity5 => new UnityVersion(5, 0, 0, UnityVersionType.Final, 1);
		private static UnityVersion Unity6 => new UnityVersion(6, 0, 0, UnityVersionType.Final, 1);
		private static UnityVersion Unity7 => new UnityVersion(7, 0, 0, UnityVersionType.Final, 1);

		[Test]
		public static void CountIsCorrectForEmptyCommonString()
		{
			TpkCommonString commonString = new();
			Assert.AreEqual(0, commonString.GetCount(Unity5));
		}

		[Test]
		public static void CountIsCorrectForNormalUse()
		{
			TpkCommonString commonString = MakeCommonString();
			Assert.AreEqual(5, commonString.GetCount(Unity3));
			Assert.AreEqual(5, commonString.GetCount(Unity4));
			Assert.AreEqual(5, commonString.GetCount(Unity5));
			Assert.AreEqual(10, commonString.GetCount(Unity6));
			Assert.AreEqual(10, commonString.GetCount(Unity7));
		}

		private static TpkCommonString MakeCommonString()
		{
			TpkCommonString commonString = new();
			commonString.Add(Unity4, 5);
			commonString.Add(Unity6, 10);
			return commonString;
		}
	}
}
