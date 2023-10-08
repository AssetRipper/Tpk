using AssetRipper.Primitives;
using AssetRipper.Tpk.EngineAssets;
using System.Collections.Generic;
using System.Linq;

namespace AssetRipper.Tpk.ConsoleApp;
internal static class TpkEngineAssetsBlobCreator
{
	public static TpkEngineAssetsBlob CreateFromDirectory(string directoryPath)
	{
		return Create(JsonFileSorter.GetOrderedFilePaths(directoryPath));
	}

	private static TpkEngineAssetsBlob Create(IEnumerable<string> pathsOrderedByUnityVersion)
	{
		TpkEngineAssetsBlob blob = new()
		{
			CreationTime = DateTime.UtcNow
		};
		string? latestJson = null;
		foreach ((string path, string json) in pathsOrderedByUnityVersion.Select(path => (path, File.ReadAllText(path))))
		{
			string versionString = Path.GetFileNameWithoutExtension(path);
			Console.WriteLine(versionString);
			UnityVersion version = UnityVersion.Parse(versionString);
			blob.Versions.Add(version);
			if (json != latestJson)
			{
				blob.Data.Add(new KeyValuePair<UnityVersion, string>(version, json));
				latestJson = json;
			}
		}
		return blob;
	}
}
