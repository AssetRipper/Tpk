using AssetRipper.Primitives;
using AssetRipper.Tpk.Shared;
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
			return Create(pathsOrderedByUnityVersion.Select(path =>
			{
				string versionString = Path.GetFileNameWithoutExtension(path);
				UnityVersion version = UnityVersion.Parse(versionString);
				UnityInfo info = UnityInfo.ReadFromJsonFile(path);
				return (version, info);
			}));
		}

		private static TpkTypeTreeBlob Create(IEnumerable<(UnityVersion Version, UnityInfo Info)> infosOrderedByUnityVersion)
		{
			TpkTypeTreeBlob blob = new TpkTypeTreeBlob();

			List<TpkCommonString.Entry> commonStrings = [];
			Dictionary<int, string> latestUnityClassesDumped = [];
			Dictionary<int, TpkClassInformation> classDictionary = [];

			foreach ((UnityVersion version, UnityInfo? info) in infosOrderedByUnityVersion)
			{
				Console.WriteLine(version.ToString());
				blob.Versions.Add(version);

				if (!Same(info.Strings, commonStrings, blob.StringBuffer))
				{
					commonStrings.Clear();
					commonStrings.EnsureCapacity(commonStrings.Count);
					foreach (UnityString unityString in info.Strings)
					{
						commonStrings.Add(new TpkCommonString.Entry((ushort)unityString.Index, unityString.String, blob.StringBuffer));
					}
					blob.CommonString.Add(version, commonStrings.ToArray());
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

			//About 26k / 65k
			Console.WriteLine($"Node buffer has {blob.NodeBuffer.Count} entries, which is {GetUShortPercent(blob.NodeBuffer.Count)}% of its maximum {ushort.MaxValue} entries");
			//About 7k / 65k
			Console.WriteLine($"String buffer has {blob.StringBuffer.Count} entries, which is {GetUShortPercent(blob.StringBuffer.Count)}% of its maximum {ushort.MaxValue} entries");

			blob.CreationTime = DateTime.Now.ToUniversalTime();

			return blob;
		}

		private static bool Same(List<UnityString> jsonStrings, List<TpkCommonString.Entry> tpkEntries, TpkStringBuffer stringBuffer)
		{
			if (jsonStrings.Count != tpkEntries.Count)
			{
				return false;
			}
			for (int i = 0; i < jsonStrings.Count; i++)
			{
				if (jsonStrings[i].String != tpkEntries[i].ToString(stringBuffer))
				{
					return false;
				}
			}
			return true;
		}

		private static int GetUShortPercent(int value) => value * 100 / ushort.MaxValue;
	}
}
