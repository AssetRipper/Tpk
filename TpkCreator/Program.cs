using AssetRipper.TypeTreeCompression.Tpk;
using System.Diagnostics;

namespace AssetRipper.TpkCreator
{
	internal class Program
	{
		//private const string InfoJsonPath = @"F:\TypeTreeDumps\InfoJson";
		//private const string InfoJsonPath = @"TestFolder";

		static void Main(string[] args)
		{
			try
			{
				Stopwatch sw = Stopwatch.StartNew();
				MakeTpk(args[0], "uncompressed.tpk", "compressed.tpk");
				//MakeTpk(InfoJsonPath, "classes.tpk");
				//ReadTpk("classes.tpk");
				sw.Stop();
				Console.WriteLine($"Done in {sw.Elapsed.TotalSeconds} seconds!");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			Console.ReadLine();
		}

		private static TpkDataBlob ReadTpk(string path)
		{
			using FileStream stream = File.OpenRead(path);
			using BinaryReader reader = new BinaryReader(stream);
			TpkFile file = new TpkFile(reader);
			return file.GetDataBlob();
		}

		private static void MakeTpk(string inputDirectory, string uncompressedPath, string lz4Path)
		{
			TpkDataBlob blob = TpkDataBlob.Create(inputDirectory);
			WriteBlobToFile(blob, uncompressedPath, TpkCompressionType.None);
			WriteBlobToFile(blob, lz4Path, TpkCompressionType.Lz4);
		}

		private static void WriteBlobToFile(TpkDataBlob blob, string outputPath, TpkCompressionType compressionType)
		{
			TpkFile file = new TpkFile(blob, compressionType);
			using FileStream stream = File.Create(outputPath);
			using BinaryWriter writer = new BinaryWriter(stream);
			file.Write(writer);
		}

		private static void Convert(string path)
		{
			string extension = Path.GetExtension(path);
			string fileNameNoExtension = Path.GetFileNameWithoutExtension(path);
			if (extension == ".tpk")
			{
				using FileStream stream = File.OpenRead(path);
				using BinaryReader reader = new BinaryReader(stream);
				TpkFile file = new TpkFile(reader);
				File.WriteAllBytes($"{fileNameNoExtension}.json", file.GetDecompressedData());
			}
			else if (extension == ".json")
			{
				byte[] fileData = File.ReadAllBytes(path);
				TpkFile file = new TpkFile(fileData, TpkCompressionType.Lz4);
				using FileStream stream = File.Create($"{fileNameNoExtension}.tpk");
				using BinaryWriter writer = new BinaryWriter(stream);
				file.Write(writer);
			}
			else
			{
				throw new Exception($"Cannot handle {extension}");
			}
		}
	}
}
