namespace AssetRipper.TpkCreation.TypeTrees
{
	public sealed class TpkUnityNodeBuffer
	{
		private List<TpkUnityNode> Nodes { get; } = new();

		/// <summary>
		/// Ensures a node is in the buffer
		/// </summary>
		/// <param name="node">The node to be added</param>
		/// <returns>The index at which that node appears</returns>
		public ushort AddNode(TpkUnityNode node)
		{
			int index = Nodes.IndexOf(node);
			if (index == -1)
			{
				index = Nodes.Count;
				Nodes.Add(node);
			}
			return (ushort)index;
		}

		public TpkUnityNode this[int index]
		{
			get => Nodes[index];
		}

		public int Count => Nodes.Count;

		public void Read(BinaryReader reader)
		{
			Nodes.Clear();
			int count = reader.ReadInt32();
			Nodes.Capacity = count;
			for (int i = 0; i < count; i++)
			{
				TpkUnityNode data = new();
				data.Read(reader);
				Nodes.Add(data);
			}
		}

		public void Write(BinaryWriter writer)
		{
			int count = Nodes.Count;
			writer.Write(count);
			for (int i = 0; i < count; i++)
			{
				Nodes[i].Write(writer);
			}
		}
	}
}
