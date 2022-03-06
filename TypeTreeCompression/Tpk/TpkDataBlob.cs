using AssetRipper.TypeTreeCompression.UnitySerialization;
using System.Text.Json;
using VersionClassPair = System.Collections.Generic.KeyValuePair<
	AssetRipper.VersionUtilities.UnityVersion,
	AssetRipper.TypeTreeCompression.Tpk.TpkUnityClass?>;
using VersionBytePair = System.Collections.Generic.KeyValuePair<
	AssetRipper.VersionUtilities.UnityVersion,
	byte>;

namespace AssetRipper.TypeTreeCompression.Tpk
{
	public sealed class TpkDataBlob
	{
		public List<TpkClassInformation> ClassInfo { get; } = new();
		public TpkCommonString CommonString { get; } = new();
		public TpkStringBuffer StringBuffer { get; } = new();
		public List<UnityVersion> Versions { get; } = new();

		public void Read(BinaryReader reader)
		{
			int versionCount = reader.ReadInt32();
			Versions.Clear();
			Versions.Capacity = versionCount;
			for(int i = 0; i < versionCount; i++)
			{
				Versions.Add(reader.ReadUnityVersion());
			}
			int classCount = reader.ReadInt32();
			ClassInfo.Clear();
			ClassInfo.Capacity = classCount;
			for(int i = 0; i < classCount; i++)
			{
				TpkClassInformation classInformation = new();
				classInformation.Read(reader);
				ClassInfo.Add(classInformation);
			}
			CommonString.Read(reader);
			StringBuffer.Read(reader);
		}

		public void Write(BinaryWriter writer)
		{
			int versionCount = Versions.Count;
			writer.Write(versionCount);
			for(int i = 0; i < versionCount; i++)
			{
				writer.Write(Versions[i]);
			}
			int classCount = ClassInfo.Count;
			writer.Write(classCount);
			for(int i = 0; i < classCount; i++)
			{
				ClassInfo[i].Write(writer);
			}
			CommonString.Write(writer);
			StringBuffer.Write(writer);
		}

		public static TpkDataBlob Create(string directoryPath)
		{
			return Create(GetOrderedFilePaths(directoryPath));
		}

		private static TpkDataBlob Create(IEnumerable<string> pathsOrderedByUnityVersion)
		{
			TpkDataBlob blob = new TpkDataBlob();
			blob.CommonString.VersionInformation.Add(new VersionBytePair(UnityVersion.MinVersion, 0));

			byte latestCommonStringCount = 0;
			List<string> commonStrings = new List<string>();
			Dictionary<int, string> latestUnityClassesDumped = new Dictionary<int, string>();
			Dictionary<int, TpkClassInformation> classDictionary = new Dictionary<int, TpkClassInformation>();

			foreach(string path in pathsOrderedByUnityVersion)
			{
				//System.Threading.Thread.Sleep(500);
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
					string dump = Dump(unityClass);
					if (!latestUnityClassesDumped.TryGetValue(unityClass.TypeID, out string? cachedDump) || cachedDump != dump)
					{
						latestUnityClassesDumped[unityClass.TypeID] = dump;
						if(!classDictionary.TryGetValue(unityClass.TypeID, out TpkClassInformation? tpkClassInformation))
						{
							tpkClassInformation = new TpkClassInformation(unityClass.TypeID);
							classDictionary.Add(unityClass.TypeID, tpkClassInformation);
						}
						TpkUnityClass tpkUnityClass = TpkUnityClass.Convert(unityClass, blob.StringBuffer);
						tpkClassInformation.Classes.Add(new VersionClassPair(version, tpkUnityClass));
					}
				}
			}

			blob.ClassInfo.AddRange(classDictionary.Values);

			blob.CommonString.SetIndices(blob.StringBuffer, commonStrings);
			Console.WriteLine($"String buffer has {blob.StringBuffer.Count} entries");
			return blob;
		}

		private static string Dump(UnityClass unityClass) => JsonSerializer.Serialize(unityClass);

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
