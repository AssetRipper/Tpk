using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace AssetRipper.Tpk.TypeTrees.Json
{
	public sealed class UnityInfo
	{
		[AllowNull]
		public List<UnityString> Strings
		{
			get;
			set => field = value ?? new();
		} = new();

		[AllowNull]
		public List<UnityClass> Classes
		{
			get;
			set => field = value ?? new();
		} = new();

		public static UnityInfo ReadFromJsonFile(string jsonPath)
		{
			string text = File.ReadAllText(jsonPath);
			return FromJsonString(text) ?? throw new Exception($"Failed to deserialize {jsonPath}");
		}

		public string ToJsonString(bool indented = false)
		{
			return indented 
				? JsonSerializer.Serialize(this, UnityInfoSerializerContext.WriteIndentedContext.UnityInfo)
				: JsonSerializer.Serialize(this, UnityInfoSerializerContext.WriteNotIndentedContext.UnityInfo);
		}

		public static UnityInfo? FromJsonString(string jsonString)
		{
			return JsonSerializer.Deserialize(jsonString, UnityInfoSerializerContext.Default.UnityInfo);
		}

		public static UnityInfo? FromStream(Stream utf8JsonStream)
		{
			return JsonSerializer.Deserialize(utf8JsonStream, UnityInfoSerializerContext.Default.UnityInfo);
		}
	}
}