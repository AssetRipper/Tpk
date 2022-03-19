using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace AssetRipper.TpkCreation.Tests
{
	public class FileSystemTests
	{
		[Test]
		public void ReadAndWriteAreTheSame()
		{
			TpkFileSystemBlob originalBlob = MakeRandomBlob();
			byte[] writtenData = ConvertToData(originalBlob);
			TpkFileSystemBlob readBlob = ConvertFromData(writtenData);
			Assert.AreEqual(originalBlob.Files, readBlob.Files);
		}

		private static byte[] ConvertToData(TpkFileSystemBlob blob)
		{
			using MemoryStream memoryStream = new MemoryStream();
			using BinaryWriter writer = new BinaryWriter(memoryStream);
			blob.Write(writer);
			return memoryStream.ToArray();
		}

		private static TpkFileSystemBlob ConvertFromData(byte[] data)
		{
			using MemoryStream memoryStream = new MemoryStream(data);
			using BinaryReader reader = new BinaryReader(memoryStream);
			TpkFileSystemBlob blob = new TpkFileSystemBlob();
			blob.Read(reader);
			return blob;
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