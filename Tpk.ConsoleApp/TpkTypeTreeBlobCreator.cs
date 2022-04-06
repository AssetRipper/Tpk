using AssetRipper.Tpk.TypeTrees.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
			Dictionary<int, UnityClass> latestUnityClassesDumped = new Dictionary<int, UnityClass>();
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
					if (!latestUnityClassesDumped.TryGetValue(unityClass.TypeID, out UnityClass? cachedClass) || !AreEqual(cachedClass, unityClass))
					{
						latestUnityClassesDumped[unityClass.TypeID] = unityClass;
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

			blob.ClassInformation.AddRange(classDictionary.Values);

			blob.CommonString.SetIndices(blob.StringBuffer, commonStrings);
			//About 20k
			Console.WriteLine($"Node buffer has {blob.NodeBuffer.Count} entries");
			//About 7k
			Console.WriteLine($"String buffer has {blob.StringBuffer.Count} entries");

			blob.CreationTime = DateTime.Now.ToUniversalTime();

			return blob;
		}

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

		private static bool AreEqual(UnityClass left, UnityClass right)
		{
			return left.Name == right.Name &&
				   left.Namespace == right.Namespace &&
				   left.FullName == right.FullName &&
				   left.Module == right.Module &&
				   left.TypeID == right.TypeID &&
				   left.Base == right.Base &&
				   left.IsAbstract == right.IsAbstract &&
				   left.IsSealed == right.IsSealed &&
				   left.IsEditorOnly == right.IsEditorOnly &&
				   left.IsReleaseOnly == right.IsReleaseOnly &&
				   left.IsStripped == right.IsStripped &&
				   AreEqual(left.EditorRootNode, right.EditorRootNode) &&
				   AreEqual(left.ReleaseRootNode, right.ReleaseRootNode);
		}

		private static bool AreEqual(UnityNode? left, UnityNode? right)
		{
			if (left is null || right is null)
				return left is null && right is null;

			return left.TypeName == right.TypeName &&
				   left.Name == right.Name &&
				   left.Version == right.Version &&
				   left.TypeFlags == right.TypeFlags &&
				   left.MetaFlag == right.MetaFlag &&
				   AreEqual(left.SubNodes, right.SubNodes);
		}

		private static bool AreEqual(List<UnityNode> left, List<UnityNode> right)
		{
			if(left.Count != right.Count)
				return false;
			for(int i = 0; i < left.Count; i++)
			{
				if(!AreEqual(left[i], right[i]))
					return false;
			}
			return true;
		}

		private const string JsonExtension = ".json";
	}
}
