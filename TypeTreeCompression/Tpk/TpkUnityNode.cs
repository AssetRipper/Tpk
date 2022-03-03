using AssetRipper.TypeTreeCompression.UnitySerialization;

namespace AssetRipper.TypeTreeCompression.Tpk
{
	public sealed class TpkUnityNode
	{
		public ushort TypeName { get; set; }
		public ushort Name { get; set; }
		//Level is redundant
		public int ByteSize { get; set; }
		//Index is redundant
		public short Version { get; set; }
		public byte TypeFlags { get; set; }
		public uint MetaFlag { get; set; }
		public TpkUnityNode[] SubNodes { get; set; } = Array.Empty<TpkUnityNode>();

		public void Read(BinaryReader reader)
		{
			TypeName = reader.ReadUInt16();
			Name = reader.ReadUInt16();
			ByteSize = reader.ReadInt32();
			Version = reader.ReadInt16();
			TypeFlags = reader.ReadByte();
			MetaFlag = reader.ReadUInt32();
			int count = reader.ReadInt32();
			SubNodes = new TpkUnityNode[count];
			for (int i = 0; i < count; i++)
			{
				SubNodes[i] = new TpkUnityNode();
				SubNodes[i].Read(reader);
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
			int count = SubNodes.Length;
			writer.Write(count);
			for (int i = 0;i < count; i++)
			{
				SubNodes[i].Write(writer);
			}
		}

		public static TpkUnityNode Convert(UnityNode node, TpkStringBuffer buffer)
		{
			TpkUnityNode result = new TpkUnityNode();
			result.TypeName = buffer.AddString(node.TypeName);
			result.Name = buffer.AddString(node.Name);
			result.ByteSize = node.ByteSize;
			result.Version = node.Version;
			result.TypeFlags = node.TypeFlags;
			result.MetaFlag = node.MetaFlag;
			result.SubNodes = node.SubNodes.Select(n => Convert(n, buffer)).ToArray();
			return result;
		}

		public static UnityNode Convert(TpkUnityNode node, TpkStringBuffer buffer, byte level, int index, out int lastIndexUsed)
		{
			UnityNode result = new UnityNode();
			result.TypeName = buffer[node.TypeName];
			result.Name = buffer[node.Name];
			result.Level = level;
			result.ByteSize = node.ByteSize;
			result.Index = index;
			result.Version = node.Version;
			result.TypeFlags = node.TypeFlags;
			result.MetaFlag = node.MetaFlag;

			byte levelPlus = unchecked((byte)(level + 1U));
			lastIndexUsed = index;
			int subNodeCount = node.SubNodes.Length;
			result.SubNodes = new List<UnityNode>(subNodeCount);
			for(int i = 0; i < subNodeCount; i++)
			{
				result.SubNodes.Add(Convert(node.SubNodes[i], buffer, levelPlus, index + 1, out lastIndexUsed));
				index = lastIndexUsed;
			}

			return result;
		}
	}
}
