namespace AssetRipper.Tpk.TypeTrees
{
	public sealed class TpkUnityClass
	{
		/// <summary>
		/// string
		/// </summary>
		public ushort Name { get; set; }

		/// <summary>
		/// string
		/// </summary>
		public ushort Namespace { get; set; }

		/// <summary>
		/// string
		/// </summary>
		public ushort FullName { get; set; }

		/// <summary>
		/// string
		/// </summary>
		public ushort Module { get; set; }

		//TypeID excluded here because it's included elsewhere

		/// <summary>
		/// string
		/// </summary>
		public ushort Base { get; set; }

		/// <summary>
		/// strings
		/// </summary>
		public ushort[] Derived { get; set; } = Array.Empty<ushort>();

		/// <summary>
		/// The count of all classes that descend from this class<br/>
		/// It includes this class, so the count is always positive<br/>
		/// However, some older unity versions don't generate this, so sometimes we have to set it in SharedState initialization
		/// </summary>
		public uint DescendantCount { get; set; }

		//Size and TypeIndex are excluded from this format

		/// <summary>
		/// 8 boolean values packed into 1 byte
		/// </summary>
		public TpkUnityClassFlags Flags { get; set; }

		public ushort EditorRootNode { get; set; } = ushort.MaxValue;
		public ushort ReleaseRootNode { get; set; } = ushort.MaxValue;

		public void Read(BinaryReader reader)
		{
			Name = reader.ReadUInt16();
			Namespace = reader.ReadUInt16();
			FullName = reader.ReadUInt16();
			Module = reader.ReadUInt16();
			Base = reader.ReadUInt16();
			int derivedCount = reader.ReadInt32();
			Derived = new ushort[derivedCount];
			for (int i = 0; i < derivedCount; i++)
			{
				Derived[i] = reader.ReadUInt16();
			}
			DescendantCount = reader.ReadUInt32();
			Flags = (TpkUnityClassFlags)reader.ReadByte();
			EditorRootNode = Flags.HasEditorRootNode() ? reader.ReadUInt16() : ushort.MaxValue;
			ReleaseRootNode = Flags.HasReleaseRootNode() ? reader.ReadUInt16() : ushort.MaxValue;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(Name);
			writer.Write(Namespace);
			writer.Write(FullName);
			writer.Write(Module);
			writer.Write(Base);
			int derivedCount = Derived.Length;
			writer.Write(derivedCount);
			for (int i = 0;i < derivedCount; i++)
			{
				writer.Write(Derived[i]);
			}
			writer.Write(DescendantCount);
			writer.Write((byte)Flags);
			if (Flags.HasEditorRootNode())
			{
				writer.Write(EditorRootNode);
			}
			if (Flags.HasReleaseRootNode())
			{
				writer.Write(ReleaseRootNode);
			}
		}
	}
}
