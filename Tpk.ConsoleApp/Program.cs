using System.Diagnostics;

namespace AssetRipper.Tpk.ConsoleApp
{
	internal class Program
	{
		static void Main(string[] args)
		{
			try
			{
				Stopwatch sw = Stopwatch.StartNew();

				TpkDataBlob blob;
				if (args[0] is "--engine-assets")
				{
					blob = TpkEngineAssetsBlobCreator.CreateFromDirectory(args[1]);
				}
				else
				{
					blob = TpkTypeTreeBlobCreator.CreateFromPath(args[1], false);
				}

				MakeTpk(blob, "uncompressed.tpk", "lz4.tpk", "lzma.tpk", "brotli.tpk");

				sw.Stop();
				Console.WriteLine($"Done in {sw.Elapsed.TotalSeconds} seconds!");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			Console.ReadLine();
		}

		private static void SpeedTest()
		{
			TpkDataBlob blob = ReadTpk("test_lzma.tpk");
			WriteTestTpks(blob);
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

		private static void MakeTpk(TpkDataBlob blob, string uncompressedPath, string lz4Path, string lzmaPath, string brotliPath)
		{
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
	}
}
