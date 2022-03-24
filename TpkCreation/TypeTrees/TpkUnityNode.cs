using AssetRipper.TpkCreation.Shared;

namespace AssetRipper.TpkCreation.TypeTrees
{
	public sealed class TpkUnityNode
	{
		public ushort NodeData { get; set; }

		public void Read(BinaryReader reader)
		{
			NodeData = reader.ReadUInt16();
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(NodeData);
		}

		public static TpkUnityNode Convert(UnityNode node, TpkStringBuffer stringBuffer, TpkUnityNodeDataBuffer nodeBuffer)
		{
			TpkUnityNodeData nodeData = new TpkUnityNodeData();
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
				nodeData.SubNodes[i] = Convert(node.SubNodes[i], stringBuffer, nodeBuffer).NodeData;
			}

			TpkUnityNode result = new TpkUnityNode();
			result.NodeData = nodeBuffer.AddNode(nodeData);
			return result;
		}

		public static UnityNode Convert(TpkUnityNode node, TpkStringBuffer stringBuffer, TpkUnityNodeDataBuffer nodeBuffer, byte level, int index, out int lastIndexUsed)
		{
			TpkUnityNodeData nodeData = nodeBuffer[node.NodeData];
			return Convert(nodeData, stringBuffer, nodeBuffer, level, index, out lastIndexUsed);
		}

		public static UnityNode Convert(TpkUnityNodeData nodeData, TpkStringBuffer stringBuffer, TpkUnityNodeDataBuffer nodeBuffer, byte level, int index, out int lastIndexUsed)
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
