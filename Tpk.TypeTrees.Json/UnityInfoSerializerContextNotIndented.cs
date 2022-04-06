using System.Text.Json.Serialization;

namespace AssetRipper.Tpk.TypeTrees.Json
{
	[JsonSourceGenerationOptions(WriteIndented = false)]
	[JsonSerializable(typeof(UnityInfo))]
	internal sealed partial class UnityInfoSerializerContextNotIndented : JsonSerializerContext
	{
	}
}
