using AssetRipper.Tpk.TypeTrees.Json;
using System.Collections.Generic;
using System.Linq;
using VersionClassPair = System.Collections.Generic.KeyValuePair<
	AssetRipper.VersionUtilities.UnityVersion,
	AssetRipper.Tpk.TypeTrees.TpkUnityClass?>;
using VersionBytePair = System.Collections.Generic.KeyValuePair<
	AssetRipper.VersionUtilities.UnityVersion,
	byte>;
using AssetRipper.VersionUtilities;
using AssetRipper.Tpk.TypeTrees;

namespace AssetRipper.Tpk.ConsoleApp
{
	internal static class TpkTypeTreeBlobCreator
	{

		public static TpkTypeTreeBlob Create(string directoryPath)
		{
			return Create(GetOrderedFilePaths(directoryPath));
		}

		private static TpkTypeTreeBlob Create(IEnumerable<string> pathsOrderedByUnityVersion)
		{
			TpkTypeTreeBlob blob = new TpkTypeTreeBlob();
			blob.CommonString.VersionInformation.Add(new VersionBytePair(UnityVersion.MinVersion, 0));

			byte latestCommonStringCount = 0;
			List<string> commonStrings = new List<string>();
			Dictionary<int, string> latestUnityClassesDumped = new Dictionary<int, string>();
			Dictionary<int, TpkClassInformation> classDictionary = new Dictionary<int, TpkClassInformation>();

			foreach (string path in pathsOrderedByUnityVersion)
			{
				Console.WriteLine(path);
				UnityInfo info = UnityInfo.ReadFromJsonFile(path);
				UnityVersion version = UnityVersion.Parse(info.Version);
				blob.Versions.Add(version);

				if (info.Strings.Count != latestCommonStringCount)
				{
					latestCommonStringCount = (byte)info.Strings.Count;
					blob.CommonString.VersionInformation.Add(new VersionBytePair(version, latestCommonStringCount));
				}

				for (int i = 0; i < info.Strings.Count; i++)
				{
					if (i < commonStrings.Count)
					{
						if (info.Strings[i].String != commonStrings[i])
						{
							throw new Exception($"String inequality at index {i} for version {version}");
						}
					}
					else
					{
						commonStrings.Add(info.Strings[i].String);
					}
				}

				foreach (UnityClass unityClass in info.Classes)
				{
					string dump = unityClass.ToJsonString();
					if (!latestUnityClassesDumped.TryGetValue(unityClass.TypeID, out string? cachedDump) || cachedDump != dump)
					{
						latestUnityClassesDumped[unityClass.TypeID] = dump;
						if (!classDictionary.TryGetValue(unityClass.TypeID, out TpkClassInformation? tpkClassInformation))
						{
							tpkClassInformation = new TpkClassInformation(unityClass.TypeID);
							classDictionary.Add(unityClass.TypeID, tpkClassInformation);
						}
						TpkUnityClass tpkUnityClass = ClassConversion.Convert(unityClass, blob.StringBuffer, blob.NodeBuffer);
						tpkClassInformation.Classes.Add(new VersionClassPair(version, tpkUnityClass));
					}
				}
			}

			foreach(TpkClassInformation tpkClassInfo in classDictionary.Values)
			{
				VersionClassPair[] pairs = tpkClassInfo.Classes.ToArray();
				TpkUnityClass? previousClass = pairs[0].Value;
				for(int i = 1; i < pairs.Length; i++)
				{
					VersionClassPair pair = pairs[i];
					if(pair.Value == previousClass)
					{
						tpkClassInfo.Classes.Remove(pair);
					}
					else
					{
						previousClass = pair.Value;
					}
				}
			}

			blob.ClassInformation.AddRange(classDictionary.Values);

			blob.CommonString.SetIndices(blob.StringBuffer, commonStrings);
			//About 19k / 65k
			Console.WriteLine($"Node buffer has {blob.NodeBuffer.Count} entries, which is {GetUShortPercent(blob.NodeBuffer.Count)}% of its maximum {ushort.MaxValue} entries");
			//About 7k / 65k
			Console.WriteLine($"String buffer has {blob.StringBuffer.Count} entries, which is {GetUShortPercent(blob.StringBuffer.Count)}% of its maximum {ushort.MaxValue} entries");

			blob.CreationTime = DateTime.Now.ToUniversalTime();

			return blob;
		}

		private static int GetUShortPercent(int value) => value * 100 / ushort.MaxValue;

		/// <summary>
		/// Get the paths to the jsons in order
		/// </summary>
		/// <param name="directoryPath">The directory containing the struct dumps</param>
		/// <returns>An array of absolute file paths ordered by Unity version</returns>
		/// <exception cref="ArgumentException"></exception>
		private static IEnumerable<string> GetOrderedFilePaths(string directoryPath)
		{
			DirectoryInfo directory = new DirectoryInfo(directoryPath);
			if (!directory.Exists)
			{
				throw new ArgumentException(nameof(directoryPath));
			}

			Dictionary<UnityVersion, string> files = new Dictionary<UnityVersion, string>();
			foreach (FileInfo file in directory.GetFiles())
			{
				if (file.Extension == JsonExtension)
				{
					string fileNameWithoutExtension = file.Name.Substring(0, file.Name.Length - JsonExtension.Length);
					UnityVersion version = UnityVersion.Parse(fileNameWithoutExtension);
					files.Add(version, file.FullName);
				}
			}
			List<UnityVersion> orderedVersions = files.Select(pair => pair.Key).ToList();
			orderedVersions.Sort();
			return orderedVersions.Select(version => files[version]);
		}

		private const string JsonExtension = ".json";
	}
}
