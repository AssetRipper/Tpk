using NUnit.Framework;

namespace AssetRipper.Tpk.TypeTrees.Json.Tests;
public class UnityNodeTests
{
	[Test]
	public void ComputeHashReturns16Bytes()
	{
		UnityNode node = new();
		Assert.That(node.ComputeHash(), Has.Length.EqualTo(16));
	}
}