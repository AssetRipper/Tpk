using AssetRipper.Primitives;
using AssetRipper.Tpk.EngineAssets;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace AssetRipper.Tpk.Tests;

public class EngineAssetsTests
{
	[Test]
	public void ReadAndWriteAreTheSame()
	{
		TpkEngineAssetsBlob originalBlob = new();
		originalBlob.CreationTime = DateTime.FromFileTimeUtc(100000);
		originalBlob.Versions.Add(new UnityVersion(3));
		originalBlob.Versions.Add(new UnityVersion(4));
		originalBlob.Data.Add(new KeyValuePair<UnityVersion, string>(new UnityVersion(3), RandomUtils.RandomString()));
		originalBlob.Data.Add(new KeyValuePair<UnityVersion, string>(new UnityVersion(4), RandomUtils.RandomString()));
		byte[] writtenData = originalBlob.ToBinary();
		TpkEngineAssetsBlob readBlob = TpkDataBlob.FromBinary<TpkEngineAssetsBlob>(writtenData);
		Assert.Multiple(() =>
		{
			Assert.That(readBlob.CreationTime, Is.EqualTo(originalBlob.CreationTime));
			Assert.That(readBlob.Versions, Is.EqualTo(originalBlob.Versions));
			Assert.That(readBlob.Data, Is.EqualTo(originalBlob.Data));
		});
	}

	[Test]
	public void EmptyReadAndWriteAreTheSame()
	{
		TpkEngineAssetsBlob originalBlob = new();
		byte[] writtenData = originalBlob.ToBinary();
		TpkEngineAssetsBlob readBlob = TpkDataBlob.FromBinary<TpkEngineAssetsBlob>(writtenData);
		Assert.Multiple(() =>
		{
			Assert.That(readBlob.CreationTime, Is.EqualTo(originalBlob.CreationTime));
			Assert.That(readBlob.Versions, Is.EqualTo(originalBlob.Versions));
			Assert.That(readBlob.Data, Is.EqualTo(originalBlob.Data));
		});
	}
}
