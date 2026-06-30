using AssetRipper.Primitives;
using AssetRipper.Tpk.TypeTrees.Json;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Writers.Zip;
using System.Collections.Generic;

namespace AssetRipper.Tpk.ConsoleApp
{
	internal static class ZipFileReader
	{
		/// <summary>
		/// Read UnityInfo objects from a zip file
		/// </summary>
		/// <param name="zipFilePath"></param>
		/// <returns>A list ordered by Unity version</returns>
		public static IEnumerable<(UnityVersion Version, UnityInfo Info)> ReadUnityInfoFromZipFile(string zipFilePath)
		{
			List<(UnityVersion Version, UnityInfo Info)> list = new();

			using FileStream fileStream = File.OpenRead(zipFilePath);
			using IWritableArchive<ZipWriterOptions> archive = ZipArchive.OpenArchive(fileStream);
			foreach (var entry in archive.Entries)
			{
				if (!entry.IsDirectory && (entry.Key ?? "").EndsWith(".json", StringComparison.Ordinal))
				{
					string versionString = Path.GetFileNameWithoutExtension(entry.Key ?? "");
					UnityVersion version = UnityVersion.Parse(versionString);
					using MemoryStream unzippedFileStream = new MemoryStream();
					entry.WriteTo(unzippedFileStream);
					unzippedFileStream.Position = 0;
					UnityInfo? info = UnityInfo.FromStream(unzippedFileStream);
					if (info is not null)
					{
						list.Add((version, info));
					}
				}
			}

			list.Sort((a, b) => a.Version.CompareTo(b.Version));

			return list;
		}
	}
}
