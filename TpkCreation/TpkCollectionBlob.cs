namespace AssetRipper.TpkCreation
{
	/// <summary>
	/// A data blob for storing other data blobs
	/// </summary>
	public sealed class TpkCollectionBlob : TpkDataBlob
	{
		/// <summary>
		/// Name : Blob
		/// </summary>
		public List<KeyValuePair<string, TpkDataBlob>> Blobs { get; } = new List<KeyValuePair<string, TpkDataBlob>>();

		public override TpkDataType DataType => TpkDataType.Collection;

		public override void Read(BinaryReader reader)
		{
			int count = reader.ReadInt32();
			Blobs.Clear();
			Blobs.Capacity = count;
			for (int i = 0; i < count; i++)
			{
				string name = reader.ReadString();
				TpkDataType blobType = (TpkDataType)reader.ReadByte(); 
				TpkDataBlob blob = blobType.ToNewBlob();
				blob.Read(reader);
				Blobs.Add(new KeyValuePair<string, TpkDataBlob>(name, blob));
			}
		}

		public override void Write(BinaryWriter writer)
		{
			writer.Write(Blobs.Count);
			for (int i = 0; i < Blobs.Count; i++)
			{
				KeyValuePair<string, TpkDataBlob> pair = Blobs[i];
				writer.Write(pair.Key);
				writer.Write((byte)pair.Value.DataType);
				pair.Value.Write(writer);
			}
		}

		public void Add(string name, TpkDataBlob blob)
		{
			Blobs.Add(new KeyValuePair<string, TpkDataBlob>(name, blob));
		}
	}
}
