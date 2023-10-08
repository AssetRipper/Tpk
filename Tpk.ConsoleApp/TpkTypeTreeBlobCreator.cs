using AssetRipper.Primitives;
using AssetRipper.Tpk.TypeTrees;
using AssetRipper.Tpk.TypeTrees.Json;
using System.Collections.Generic;
using System.Linq;
using VersionClassPair = System.Collections.Generic.KeyValuePair<
	AssetRipper.Primitives.UnityVersion,
	AssetRipper.Tpk.TypeTrees.TpkUnityClass?>;

namespace AssetRipper.Tpk.ConsoleApp
{
	internal static class TpkTypeTreeBlobCreator
	{
		public static TpkTypeTreeBlob CreateFromPath(string path, bool isZipFile)
		{
			return isZipFile ? CreateFromZipFile(path) : CreateFromDirectory(path);
		}

		public static TpkTypeTreeBlob CreateFromDirectory(string directoryPath)
		{
			return Create(JsonFileSorter.GetOrderedFilePaths(directoryPath));
		}

		public static TpkTypeTreeBlob CreateFromZipFile(string zipFilePath)
		{
			return Create(ZipFileReader.ReadUnityInfoFromZipFile(zipFilePath));
		}

		private static TpkTypeTreeBlob Create(IEnumerable<string> pathsOrderedByUnityVersion)
		{
			return Create(pathsOrderedByUnityVersion.Select(path => UnityInfo.ReadFromJsonFile(path)));
		}

		private static TpkTypeTreeBlob Create(IEnumerable<UnityInfo> infosOrderedByUnityVersion)
		{
			TpkTypeTreeBlob blob = new TpkTypeTreeBlob();
			blob.CommonString.Add(UnityVersion.MinVersion, 0);

			byte latestCommonStringCount = 0;
			List<string> commonStrings = new List<string>();
			Dictionary<int, string> latestUnityClassesDumped = new Dictionary<int, string>();
			Dictionary<int, TpkClassInformation> classDictionary = new Dictionary<int, TpkClassInformation>();

			foreach (UnityInfo info in infosOrderedByUnityVersion)
			{
				Console.WriteLine(info.Version);
				UnityVersion version = UnityVersion.Parse(info.Version);
				blob.Versions.Add(version);

				if (info.Strings.Count != latestCommonStringCount)
				{
					latestCommonStringCount = (byte)info.Strings.Count;
					blob.CommonString.Add(version, latestCommonStringCount);
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

				List<int> typeIds = info.Classes.Select(c => c.TypeID).ToList();
				foreach (int unusedId in classDictionary.Keys.Where(id => !typeIds.Contains(id)))
				{
					if (!string.IsNullOrEmpty(latestUnityClassesDumped[unusedId]))
					{
						latestUnityClassesDumped[unusedId] = "";
						classDictionary[unusedId].Classes.Add(new VersionClassPair(version, null));
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
			//About 21k / 65k
			Console.WriteLine($"Node buffer has {blob.NodeBuffer.Count} entries, which is {GetUShortPercent(blob.NodeBuffer.Count)}% of its maximum {ushort.MaxValue} entries");
			//About 7k / 65k
			Console.WriteLine($"String buffer has {blob.StringBuffer.Count} entries, which is {GetUShortPercent(blob.StringBuffer.Count)}% of its maximum {ushort.MaxValue} entries");

			blob.CreationTime = DateTime.Now.ToUniversalTime();

			return blob;
		}

		private static int GetUShortPercent(int value) => value * 100 / ushort.MaxValue;
	}
}
