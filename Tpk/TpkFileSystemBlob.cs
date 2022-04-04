namespace AssetRipper.Tpk
{
	/// <summary>
	/// A data blob for storing file data
	/// </summary>
	public sealed class TpkFileSystemBlob : TpkDataBlob
	{
		/// <summary>
		/// Relative path : File data
		/// </summary>
		public List<KeyValuePair<string, byte[]>> Files { get; } = new List<KeyValuePair<string, byte[]>>();

		public override TpkDataType DataType => TpkDataType.FileSystem;

		public override void Read(BinaryReader reader)
		{
			int count = reader.ReadInt32();
			Files.Clear();
			Files.Capacity = count;
			for (int i = 0; i < count; i++)
			{
				string relativePath = reader.ReadString();
				int byteCount = reader.ReadInt32();
				byte[] data = reader.ReadBytes(byteCount);
				Files.Add(new KeyValuePair<string, byte[]>(relativePath, data));
			}
		}

		public override void Write(BinaryWriter writer)
		{
			writer.Write(Files.Count);
			for (int i = 0; i < Files.Count; i++)
			{
				KeyValuePair<string, byte[]> file = Files[i];
				writer.Write(file.Key);
				writer.Write(file.Value.Length);
				writer.Write(file.Value);
			}
		}

		public void Add(string relativePath, byte[] data)
		{
			Files.Add(new KeyValuePair<string, byte[]>(relativePath, data));
		}
	}
}
