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
		public static IEnumerable<UnityInfo> ReadUnityInfoFromZipFile(string zipFilePath)
		{
			List<UnityInfo> list = new();

			using FileStream fileStream = File.OpenRead(zipFilePath);
			using IWritableArchive<ZipWriterOptions> archive = ZipArchive.OpenArchive(fileStream);
			foreach (var entry in archive.Entries)
			{
				if (!entry.IsDirectory && (entry.Key ?? "").EndsWith(".json", StringComparison.Ordinal))
				{
					using MemoryStream unzippedFileStream = new MemoryStream();
					entry.WriteTo(unzippedFileStream);
					unzippedFileStream.Position = 0;
					UnityInfo? info = UnityInfo.FromStream(unzippedFileStream);
					if (info is not null)
					{
						list.Add(info);
					}
				}
			}

			list.Sort(CompareUnityInfo);

			return list;
		}

		/// <summary>
		/// Compare two UnityInfo by their versions
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns>
		/// Less than zero: a precedes b<br />
		/// Zero: equivalent position<br />
		/// Greater than zero: a follows b
		/// </returns>
		private static int CompareUnityInfo(UnityInfo a, UnityInfo b)
		{
			UnityVersion versionA = UnityVersion.Parse(a.Version);
			UnityVersion versionB = UnityVersion.Parse(b.Version);
			return versionA.CompareTo(versionB);
		}
	}
}
