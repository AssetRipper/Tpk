namespace AssetRipper.TpkCreation.TypeTrees
{
	public sealed class TpkUnityNodeData : IEquatable<TpkUnityNodeData>
	{
		public ushort TypeName { get; set; }
		public ushort Name { get; set; }
		//Level is redundant
		public int ByteSize { get; set; }
		//Index is redundant
		public short Version { get; set; }
		public byte TypeFlags { get; set; }
		public uint MetaFlag { get; set; }

		public void Read(BinaryReader reader)
		{
			TypeName = reader.ReadUInt16();
			Name = reader.ReadUInt16();
			ByteSize = reader.ReadInt32();
			Version = reader.ReadInt16();
			TypeFlags = reader.ReadByte();
			MetaFlag = reader.ReadUInt32();
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(TypeName);
			writer.Write(Name);
			writer.Write(ByteSize);
			writer.Write(Version);
			writer.Write(TypeFlags);
			writer.Write(MetaFlag);
		}

		public override bool Equals(object? obj)
		{
			return obj is not null && obj is TpkUnityNodeData data && Equals(data);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(TypeName, Name, ByteSize, Version, TypeFlags, MetaFlag);
		}

		public bool Equals(TpkUnityNodeData? other)
		{
			return other is not null && 
				TypeName == other.TypeName &&
                Name == other.Name &&
                ByteSize == other.ByteSize &&
                Version == other.Version &&
                TypeFlags == other.TypeFlags &&
                MetaFlag == other.MetaFlag;
		}
	}
}
