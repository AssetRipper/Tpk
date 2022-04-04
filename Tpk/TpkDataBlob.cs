namespace AssetRipper.Tpk
{
	public abstract class TpkDataBlob
	{
		public abstract TpkDataType DataType { get; }
		public abstract void Read(BinaryReader reader);
		public void Read(byte[] data)
		{
			using MemoryStream memoryStream = new MemoryStream(data);
			using BinaryReader reader = new BinaryReader(memoryStream);
			Read(reader);
		}
		public void Read(Stream stream)
		{
			using BinaryReader reader = new BinaryReader(stream);
			Read(reader);
		}
		public abstract void Write(BinaryWriter writer);
		public byte[] ToBinary()
		{
			using MemoryStream memoryStream = new MemoryStream();
			using BinaryWriter writer = new BinaryWriter(memoryStream);
			Write(writer);
			return memoryStream.ToArray();
		}
		public static T FromBinary<T>(byte[] data) where T : TpkDataBlob, new()
		{
			T blob = new T();
			blob.Read(data);
			return blob;
		}
	}
}
