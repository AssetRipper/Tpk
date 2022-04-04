using AssetRipper.Tpk.TypeTrees;
using System.Diagnostics;

namespace AssetRipper.Tpk.ConsoleApp
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
				//MakeTpk(args[0], "uncompressed.tpk", "lz4.tpk", "lzma.tpk");
				//MakeTpk(InfoJsonPath, "classes.tpk");
				TpkDataBlob blob = ReadTpk("test_lzma.tpk");
				WriteTestTpks(blob);
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
			Stopwatch sw = Stopwatch.StartNew();
			using FileStream stream = File.OpenRead(path);
			using BinaryReader reader = new BinaryReader(stream);
			TpkFile file = new TpkFile(reader);
			TpkDataBlob blob = file.GetDataBlob();
			sw.Stop();
			Console.WriteLine($"Read blob in {sw.Elapsed.TotalSeconds} seconds!");
			return blob;
		}

		private static void WriteTestTpks(TpkDataBlob blob)
		{
			Stopwatch sw = Stopwatch.StartNew();
			WriteBlobToFile(blob, "test_brotli.tpk", TpkCompressionType.Lz4);
			sw.Stop();
			Console.WriteLine($"Lz4 compressed in {sw.Elapsed.TotalSeconds} seconds!");
#if DEBUG
			sw.Restart();
			WriteBlobToFile(blob, "test_brotli.tpk", TpkCompressionType.Brotli);
			sw.Stop();
			Console.WriteLine($"Brotli compressed in {sw.Elapsed.TotalSeconds} seconds!");
#endif
			sw.Restart();
			WriteBlobToFile(blob, "test_lzma.tpk", TpkCompressionType.Lzma);
			sw.Stop();
			Console.WriteLine($"Lzma compressed in {sw.Elapsed.TotalSeconds} seconds!");
		}

		private static void MakeTpk(string inputDirectory, string uncompressedPath, string lz4Path, string lzmaPath)
		{
			TpkTypeTreeBlob blob = TpkTypeTreeBlob.Create(inputDirectory);

			WriteBlobToFile(blob, uncompressedPath, TpkCompressionType.None);
			Console.WriteLine($"Uncompressed file saved to {Path.GetFullPath(uncompressedPath)}");

			WriteBlobToFile(blob, lz4Path, TpkCompressionType.Lz4);
			Console.WriteLine($"Lz4 file saved to {Path.GetFullPath(lz4Path)}");

			WriteBlobToFile(blob, lzmaPath, TpkCompressionType.Lzma);
			Console.WriteLine($"Lzma file saved to {Path.GetFullPath(lzmaPath)}");
		}

		private static void WriteBlobToFile(TpkDataBlob blob, string outputPath, TpkCompressionType compressionType)
		{
			TpkFile file = new TpkFile(blob, compressionType);
			file.WriteToFile(outputPath);
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
				file.WriteToFile($"{fileNameNoExtension}.tpk");
			}
			else
			{
				throw new Exception($"Cannot handle {extension}");
			}
		}
	}
}
