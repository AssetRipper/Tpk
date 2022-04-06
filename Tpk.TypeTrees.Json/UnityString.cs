using System.Text.Json;

namespace AssetRipper.Tpk.TypeTrees.Json
{
	public sealed class UnityString
	{
		private string @string = "";

		public uint Index { get; set; }
		public string String { get => @string; set => @string = value ?? ""; }

		public string ToJsonString(bool indented = false)
		{
			return indented
				? JsonSerializer.Serialize(this, UnityInfoSerializerContextIndented.Default.UnityString)
				: JsonSerializer.Serialize(this, UnityInfoSerializerContextNotIndented.Default.UnityString);
		}

		public static UnityString? FromJsonString(string jsonString)
		{
			return JsonSerializer.Deserialize(jsonString, UnityInfoSerializerContextIndented.Default.UnityString);
		}
	}
}