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
				MakeTpk(args[0], "uncompressed.tpk", "lz4.tpk", "lzma.tpk", "brotli.tpk", false);
				//MakeTpk(InfoJsonPath, "classes.tpk");
				//TpkDataBlob blob = ReadTpk("test_lzma.tpk");
				//WriteTestTpks(blob);
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
			TpkFile file = TpkFile.FromFile(path);
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

			sw.Restart();
			WriteBlobToFile(blob, "test_brotli.tpk", TpkCompressionType.Brotli);
			sw.Stop();
			Console.WriteLine($"Brotli compressed in {sw.Elapsed.TotalSeconds} seconds!");

			sw.Restart();
			WriteBlobToFile(blob, "test_lzma.tpk", TpkCompressionType.Lzma);
			sw.Stop();
			Console.WriteLine($"Lzma compressed in {sw.Elapsed.TotalSeconds} seconds!");
		}

		private static void MakeTpk(string inputDirectory, string uncompressedPath, string lz4Path, string lzmaPath, string brotliPath, bool isZipFile = false)
		{
			TpkTypeTreeBlob blob = isZipFile 
				? TpkTypeTreeBlobCreator.CreateFromZipFile(inputDirectory)
				: TpkTypeTreeBlobCreator.CreateFromDirectory(inputDirectory);

			WriteBlobToFile(blob, uncompressedPath, TpkCompressionType.None);
			Console.WriteLine($"Uncompressed file saved to {Path.GetFullPath(uncompressedPath)}");

			WriteBlobToFile(blob, lz4Path, TpkCompressionType.Lz4);
			Console.WriteLine($"Lz4 file saved to {Path.GetFullPath(lz4Path)}");

			WriteBlobToFile(blob, lzmaPath, TpkCompressionType.Lzma);
			Console.WriteLine($"Lzma file saved to {Path.GetFullPath(lzmaPath)}");

			WriteBlobToFile(blob, brotliPath, TpkCompressionType.Brotli);
			Console.WriteLine($"Brotli file saved to {Path.GetFullPath(brotliPath)}");
		}

		private static void WriteBlobToFile(TpkDataBlob blob, string outputPath, TpkCompressionType compressionType)
		{
			TpkFile file = TpkFile.FromBlob(blob, compressionType);
			file.WriteToFile(outputPath);
		}

		private static void Convert(string path)
		{
			string extension = Path.GetExtension(path);
			string fileNameNoExtension = Path.GetFileNameWithoutExtension(path);
			if (extension == ".tpk")
			{
				TpkFile file = TpkFile.FromFile(path);
				File.WriteAllBytes($"{fileNameNoExtension}.json", file.GetDecompressedData());
			}
			else if (extension == ".json")
			{
				byte[] fileData = File.ReadAllBytes(path);
				TpkFile file = TpkFile.FromData(fileData, TpkCompressionType.Lz4);
				file.WriteToFile($"{fileNameNoExtension}.tpk");
			}
			else
			{
				throw new Exception($"Cannot handle {extension}");
			}
		}
	}
}
