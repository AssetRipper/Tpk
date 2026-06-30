using System.Text.Json;
using System.Text.Json.Serialization;

namespace AssetRipper.Tpk.TypeTrees.Json
{
	[JsonSourceGenerationOptions(WriteIndented = false)]
	[JsonSerializable(typeof(UnityInfo))]
	internal sealed partial class UnityInfoSerializerContext : JsonSerializerContext
	{
		public static JsonSerializerOptions WriteIndentedOptions { get; }
		public static UnityInfoSerializerContext WriteIndentedContext { get; }

		static UnityInfoSerializerContext()
		{

			WriteIndentedOptions = new JsonSerializerOptions(s_defaultOptions)
			{
				WriteIndented = true
			};
			WriteIndentedContext = new UnityInfoSerializerContext(WriteIndentedOptions);
		}
	}
}
