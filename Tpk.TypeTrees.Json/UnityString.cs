namespace AssetRipper.Tpk.TypeTrees.Json
{
	public sealed class UnityString
	{
		private string @string = "";

		public uint Index { get; set; }
		public string String { get => @string; set => @string = value ?? ""; }
	}
}