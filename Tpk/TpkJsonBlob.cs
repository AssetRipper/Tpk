namespace AssetRipper.Tpk
{
	public sealed class TpkJsonBlob : TpkDataBlob
	{
		public string Text { get; set; } = string.Empty;

		public override TpkDataType DataType => TpkDataType.Json;

		public override void Read(BinaryReader reader)
		{
			Text = reader.ReadString();
		}

		public override void Write(BinaryWriter writer)
		{
			writer.Write(Text);
		}
	}
}
