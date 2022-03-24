using AssetRipper.TpkCreation.Shared;

namespace AssetRipper.TpkCreation.TypeTrees
{
	public sealed class TpkUnityNode : IEquatable<TpkUnityNode>
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
			return obj is not null && obj is TpkUnityNode data && Equals(data);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(TypeName, Name, ByteSize, Version, TypeFlags, MetaFlag, SubNodes);
		}

		public bool Equals(TpkUnityNode? other)
		{
			return other is not null &&
				TypeName == other.TypeName &&
				Name == other.Name &&
				ByteSize == other.ByteSize &&
				Version == other.Version &&
				TypeFlags == other.TypeFlags &&
				MetaFlag == other.MetaFlag &&
				ArrayEqual(SubNodes, other.SubNodes);
		}

		private static bool ArrayEqual(ushort[] array1, ushort[] array2)
		{
			if (array1.Length != array2.Length)
				return false;
			for (int i = 0; i < array1.Length; i++)
			{
				if (array1[i] != array2[i])
				{
					return false;
				}
			}
			return true;
		}

		public static ushort Convert(UnityNode node, TpkStringBuffer stringBuffer, TpkUnityNodeBuffer nodeBuffer)
		{
			TpkUnityNode nodeData = new TpkUnityNode();
			nodeData.TypeName = stringBuffer.AddString(node.TypeName);
			nodeData.Name = stringBuffer.AddString(node.Name);
			nodeData.ByteSize = node.ByteSize;
			nodeData.Version = node.Version;
			nodeData.TypeFlags = node.TypeFlags;
			nodeData.MetaFlag = node.MetaFlag;

			int subNodeCount = node.SubNodes.Count;
			nodeData.SubNodes = subNodeCount == 0 ? Array.Empty<ushort>() : new ushort[subNodeCount];
			for (int i = 0; i < subNodeCount; i++)
			{
				nodeData.SubNodes[i] = Convert(node.SubNodes[i], stringBuffer, nodeBuffer);
			}

			return nodeBuffer.AddNode(nodeData);
		}

		public static UnityNode Convert(TpkUnityNode nodeData, TpkStringBuffer stringBuffer, TpkUnityNodeBuffer nodeBuffer, byte level, int index, out int lastIndexUsed)
		{
			UnityNode result = new UnityNode();
			result.TypeName = stringBuffer[nodeData.TypeName];
			result.Name = stringBuffer[nodeData.Name];
			result.Level = level;
			result.ByteSize = nodeData.ByteSize;
			result.Index = index;
			result.Version = nodeData.Version;
			result.TypeFlags = nodeData.TypeFlags;
			result.MetaFlag = nodeData.MetaFlag;

			byte levelPlus = unchecked((byte)(level + 1U));
			lastIndexUsed = index;
			int subNodeCount = nodeData.SubNodes.Length;
			result.SubNodes = new List<UnityNode>(subNodeCount);
			for(int i = 0; i < subNodeCount; i++)
			{
				result.SubNodes.Add(Convert(nodeBuffer[nodeData.SubNodes[i]], stringBuffer, nodeBuffer, levelPlus, index + 1, out lastIndexUsed));
				index = lastIndexUsed;
			}

			return result;
		}
	}
}
