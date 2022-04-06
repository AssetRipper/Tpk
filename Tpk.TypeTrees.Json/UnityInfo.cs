using System.Text.Json;

namespace AssetRipper.Tpk.TypeTrees.Json
{
	public sealed class UnityInfo
	{
		private string version = "";
		private List<UnityString> strings = new();
		private List<UnityClass> classes = new();

		public string Version
		{
			get => version;
			set => version = value ?? "";
		}

		public List<UnityString> Strings
		{
			get => strings;
			set => strings = value ?? new();
		}

		public List<UnityClass> Classes
		{
			get => classes;
			set => classes = value ?? new();
		}

		public static UnityInfo ReadFromJsonFile(string jsonPath)
		{
			string text = File.ReadAllText(jsonPath);
			return FromJsonString(text) ?? throw new Exception($"Failed to deserialize {jsonPath}");
		}

		public string ToJsonString(bool indented = false)
		{
			return indented 
				? JsonSerializer.Serialize(this, UnityInfoSerializerContextIndented.Default.UnityInfo)
				: JsonSerializer.Serialize(this, UnityInfoSerializerContextNotIndented.Default.UnityInfo);
		}

		public static UnityInfo? FromJsonString(string jsonString)
		{
			return JsonSerializer.Deserialize(jsonString, UnityInfoSerializerContextIndented.Default.UnityInfo);
		}
	}
}