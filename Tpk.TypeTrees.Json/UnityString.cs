using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace AssetRipper.Tpk.TypeTrees.Json
{
	public sealed record class UnityString
	{
		public uint Index { get; set; }

		[AllowNull]
		public string String { get; set => field = value ?? ""; } = "";

		public string ToJsonString(bool indented = false)
		{
			return indented
				? JsonSerializer.Serialize(this, UnityInfoSerializerContext.WriteIndentedContext.UnityString)
				: JsonSerializer.Serialize(this, UnityInfoSerializerContext.WriteNotIndentedContext.UnityString);
		}

		public static UnityString? FromJsonString(string jsonString)
		{
			return JsonSerializer.Deserialize(jsonString, UnityInfoSerializerContext.Default.UnityString);
		}
	}
}