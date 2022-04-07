namespace AssetRipper.Tpk.TypeTrees.Json
{
	internal static class TypeNameFixer
	{
		public static string GetFixedName(string originalName)
		{
			return originalName switch
			{
				"short" => "SInt16",
				"int" => "SInt32",
				"long long" => "SInt64",
				"unsigned short" => "UInt16",
				"unsigned int" => "UInt32",
				"unsigned long long" => "UInt64",
				_ => originalName,
			};
		}
	}
}
