using AssetRipper.Tpk.Shared;

namespace AssetRipper.Tpk.TypeTrees.Json
{
	public static class NodeConversion
	{
		public static ushort Convert(UnityNode node, TpkStringBuffer stringBuffer, TpkUnityNodeBuffer nodeBuffer)
		{
			TpkUnityNode nodeData = new TpkUnityNode();
			nodeData.TypeName = stringBuffer.AddString(TypeNameFixer.GetFixedName(node.TypeName));
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
			for (int i = 0; i < subNodeCount; i++)
			{
				result.SubNodes.Add(Convert(nodeBuffer[nodeData.SubNodes[i]], stringBuffer, nodeBuffer, levelPlus, index + 1, out lastIndexUsed));
				index = lastIndexUsed;
			}

			return result;
		}
	}
}
