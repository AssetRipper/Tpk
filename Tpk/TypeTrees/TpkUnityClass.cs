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

		//Derived and Descendent count are excluded from this format

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
