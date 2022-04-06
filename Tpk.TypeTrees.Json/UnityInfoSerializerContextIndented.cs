using System.Text.Json.Serialization;

namespace AssetRipper.Tpk.TypeTrees.Json
{
	[JsonSourceGenerationOptions(WriteIndented = true)]
	[JsonSerializable(typeof(UnityInfo))]
	internal sealed partial class UnityInfoSerializerContextIndented : JsonSerializerContext
	{
	}
}
