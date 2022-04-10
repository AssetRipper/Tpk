namespace AssetRipper.Tpk.TypeTrees
{
	public sealed class TpkUnityClass : IEquatable<TpkUnityClass?>
	{
		/// <summary>
		/// string
		/// </summary>
		public ushort Name { get; set; }

		//Namespace, FullName, and Module are excluded from this format

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
			Base = reader.ReadUInt16();
			Flags = (TpkUnityClassFlags)reader.ReadByte();
			EditorRootNode = Flags.HasEditorRootNode() ? reader.ReadUInt16() : ushort.MaxValue;
			ReleaseRootNode = Flags.HasReleaseRootNode() ? reader.ReadUInt16() : ushort.MaxValue;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(Name);
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

		public override bool Equals(object? obj)
		{
			return Equals(obj as TpkUnityClass);
		}

		public bool Equals(TpkUnityClass? other)
		{
			return other != null &&
				   Name == other.Name &&
				   Base == other.Base &&
				   Flags == other.Flags &&
				   EditorRootNode == other.EditorRootNode &&
				   ReleaseRootNode == other.ReleaseRootNode;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Name, Base, Flags, EditorRootNode, ReleaseRootNode);
		}

		public static bool operator ==(TpkUnityClass? left, TpkUnityClass? right)
		{
			return EqualityComparer<TpkUnityClass>.Default.Equals(left, right);
		}

		public static bool operator !=(TpkUnityClass? left, TpkUnityClass? right)
		{
			return !(left == right);
		}
	}
}
