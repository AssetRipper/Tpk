using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AssetRipper.Tpk.TypeTrees.Json
{
	[JsonSourceGenerationOptions(IndentCharacter = '\t', IndentSize = 1)]
	[JsonSerializable(typeof(UnityInfo))]
	internal sealed partial class UnityInfoSerializerContext : JsonSerializerContext
	{
		public static UnityInfoSerializerContext WriteIndentedContext { get; }
		public static UnityInfoSerializerContext WriteNotIndentedContext { get; }

		static UnityInfoSerializerContext()
		{
			JsonSerializerOptions WriteIndentedOptions = new(s_defaultOptions)
			{
				WriteIndented = true,
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
			};
			WriteIndentedContext = new UnityInfoSerializerContext(WriteIndentedOptions);

			JsonSerializerOptions WriteNotIndentedOptions = new(s_defaultOptions)
			{
				WriteIndented = false,
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
			};
			WriteNotIndentedContext = new UnityInfoSerializerContext(WriteNotIndentedOptions);
		}
	}
}
