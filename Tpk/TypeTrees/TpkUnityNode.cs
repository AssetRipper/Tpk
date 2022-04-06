namespace AssetRipper.Tpk.TypeTrees
{
	public sealed class TpkUnityNode : IEquatable<TpkUnityNode?>
	{
		public ushort TypeName { get; set; }
		public ushort Name { get; set; }
		//Level is redundant
		public int ByteSize { get; set; }
		//Index is redundant
		public short Version { get; set; }
		public byte TypeFlags { get; set; }
		public uint MetaFlag { get; set; }
		public ushort[] SubNodes { get; set; } = Array.Empty<ushort>();

		public void Read(BinaryReader reader)
		{
			TypeName = reader.ReadUInt16();
			Name = reader.ReadUInt16();
			ByteSize = reader.ReadInt32();
			Version = reader.ReadInt16();
			TypeFlags = reader.ReadByte();
			MetaFlag = reader.ReadUInt32();
			ushort count = reader.ReadUInt16();
			SubNodes = new ushort[count];
			for (int i = 0; i < count; i++)
			{
				SubNodes[i] = reader.ReadUInt16();
			}
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(TypeName);
			writer.Write(Name);
			writer.Write(ByteSize);
			writer.Write(Version);
			writer.Write(TypeFlags);
			writer.Write(MetaFlag);
			writer.Write((ushort)SubNodes.Length);
			for (int i = 0; i < SubNodes.Length; i++)
			{
				writer.Write(SubNodes[i]);
			}
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as TpkUnityNode);
		}

		public bool Equals(TpkUnityNode? other)
		{
			return other != null &&
				   TypeName == other.TypeName &&
				   Name == other.Name &&
				   ByteSize == other.ByteSize &&
				   Version == other.Version &&
				   TypeFlags == other.TypeFlags &&
				   MetaFlag == other.MetaFlag &&
				   EqualityComparer<ushort[]>.Default.Equals(SubNodes, other.SubNodes);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(TypeName, Name, ByteSize, Version, TypeFlags, MetaFlag, SubNodes);
		}

		public static bool operator ==(TpkUnityNode? left, TpkUnityNode? right)
		{
			return EqualityComparer<TpkUnityNode>.Default.Equals(left, right);
		}

		public static bool operator !=(TpkUnityNode? left, TpkUnityNode? right)
		{
			return !(left == right);
		}
	}
}
