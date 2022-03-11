namespace AssetRipper.TpkCreation
{
	public abstract class TpkDataBlob
	{
		public abstract TpkDataType DataType { get; }
		public abstract void Read(BinaryReader reader);
		public abstract void Write(BinaryWriter writer);
	}
}
