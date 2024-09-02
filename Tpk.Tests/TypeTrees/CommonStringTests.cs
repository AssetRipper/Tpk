using AssetRipper.Primitives;
using AssetRipper.Tpk.TypeTrees;
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
			Assert.That(commonString.GetCount(Unity5), Is.EqualTo(0));
		}

		[Test]
		public static void CountIsCorrectForNormalUse()
		{
			TpkCommonString commonString = MakeCommonString();
			Assert.Multiple(() =>
			{
				Assert.That(commonString.GetCount(Unity3), Is.EqualTo(5));
				Assert.That(commonString.GetCount(Unity4), Is.EqualTo(5));
				Assert.That(commonString.GetCount(Unity5), Is.EqualTo(5));
				Assert.That(commonString.GetCount(Unity6), Is.EqualTo(10));
				Assert.That(commonString.GetCount(Unity7), Is.EqualTo(10));
			});
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
