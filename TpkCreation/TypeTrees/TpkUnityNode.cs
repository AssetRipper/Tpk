using AssetRipper.TpkCreation.Shared;

namespace AssetRipper.TpkCreation.TypeTrees
{
	public sealed class TpkUnityNode
	{
		public int NodeData { get; set; }
		public TpkUnityNode[] SubNodes { get; set; } = Array.Empty<TpkUnityNode>();

		public void Read(BinaryReader reader)
		{
			NodeData = reader.ReadInt32();
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
			writer.Write(NodeData);
			int count = SubNodes.Length;
			writer.Write(count);
			for (int i = 0;i < count; i++)
			{
				SubNodes[i].Write(writer);
			}
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
			TpkUnityNode result = new TpkUnityNode();
			result.NodeData = nodeBuffer.AddNode(nodeData);
			result.SubNodes = node.SubNodes.Select(n => Convert(n, stringBuffer, nodeBuffer)).ToArray();
			return result;
		}

		public static UnityNode Convert(TpkUnityNode node, TpkStringBuffer stringBuffer, TpkUnityNodeDataBuffer nodeBuffer, byte level, int index, out int lastIndexUsed)
		{
			TpkUnityNodeData nodeData = nodeBuffer[node.NodeData];
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
			int subNodeCount = node.SubNodes.Length;
			result.SubNodes = new List<UnityNode>(subNodeCount);
			for(int i = 0; i < subNodeCount; i++)
			{
				result.SubNodes.Add(Convert(node.SubNodes[i], stringBuffer, nodeBuffer, levelPlus, index + 1, out lastIndexUsed));
				index = lastIndexUsed;
			}

			return result;
		}
	}
}
