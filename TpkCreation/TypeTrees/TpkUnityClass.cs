using AssetRipper.TpkCreation.Shared;

namespace AssetRipper.TpkCreation.TypeTrees
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

		public TpkUnityNode? EditorRootNode { get; set; }
		public TpkUnityNode? ReleaseRootNode { get; set; }

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
			if (Flags.HasEditorRootNode())
			{
				EditorRootNode = new TpkUnityNode();
				EditorRootNode.Read(reader);
			}
			if (Flags.HasReleaseRootNode())
			{
				ReleaseRootNode = new TpkUnityNode();
				ReleaseRootNode.Read(reader);
			}
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
				EditorRootNode!.Write(writer);
			}
			if (Flags.HasReleaseRootNode())
			{
				ReleaseRootNode!.Write(writer);
			}
		}

		private static TpkUnityClassFlags GetFlags(UnityClass unityClass)
		{
			TpkUnityClassFlags result = TpkUnityClassFlags.None;
			if (unityClass.IsAbstract)
				result |= TpkUnityClassFlags.IsAbstract;
			if (unityClass.IsSealed)
				result |= TpkUnityClassFlags.IsSealed;
			if (unityClass.IsEditorOnly)
				result |= TpkUnityClassFlags.IsEditorOnly;
			if (unityClass.IsReleaseOnly)
				result |= TpkUnityClassFlags.IsReleaseOnly;
			if (unityClass.IsStripped)
				result |= TpkUnityClassFlags.IsStripped;
			if (unityClass.EditorRootNode != null)
				result |= TpkUnityClassFlags.HasEditorRootNode;
			if (unityClass.ReleaseRootNode != null)
				result |= TpkUnityClassFlags.HasReleaseRootNode;
			return result;
		}

		public static TpkUnityClass Convert(UnityClass source, TpkStringBuffer buffer)
		{
			TpkUnityClass result = new TpkUnityClass();
			result.Name = buffer.AddString(source.Name);
			result.Namespace = buffer.AddString(source.Namespace);
			result.FullName = buffer.AddString(source.FullName);
			result.Module = buffer.AddString(source.Module);
			result.Base = buffer.AddString(source.Base);
			int derivedCount = source.Derived.Count;
			result.Derived = new ushort[derivedCount];
			for (int i = 0; i < derivedCount; i++)
			{
				result.Derived[i] = buffer.AddString(source.Derived[i]);
			}
			result.DescendantCount = source.DescendantCount;
			result.Flags = GetFlags(source);
			if (source.EditorRootNode != null)
			{
				result.EditorRootNode = TpkUnityNode.Convert(source.EditorRootNode, buffer);
			}
			if (source.ReleaseRootNode != null)
			{
				result.ReleaseRootNode = TpkUnityNode.Convert(source.ReleaseRootNode, buffer);
			}
			return result;
		}

		public static UnityClass Convert(TpkUnityClass source, TpkStringBuffer buffer)
		{
			UnityClass result = new UnityClass();
			result.Name = buffer[source.Name];
			result.Namespace = buffer[source.Namespace];
			result.FullName = buffer[source.FullName];
			result.Module = buffer[source.Module];
			//TypeID gets set elsewhere
			result.Base = buffer[source.Base];
			result.Derived = new List<string>(source.Derived.Length);
			for (int i = 0;i < source.Derived.Length; i++)
			{
				result.Derived.Add(buffer[source.Derived[i]]);
			}
			result.DescendantCount = source.DescendantCount;
			result.Size = -1;
			//Type index is ignored
			result.IsAbstract = source.Flags.IsAbstract();
			result.IsSealed = source.Flags.IsSealed();
			result.IsEditorOnly = source.Flags.IsEditorOnly();
			result.IsReleaseOnly = source.Flags.IsReleaseOnly();
			result.IsStripped = source.Flags.IsStripped();
			if (source.EditorRootNode != null)
			{
				result.EditorRootNode = TpkUnityNode.Convert(source.EditorRootNode, buffer, 0, 0, out var _);
			}
			if (source.ReleaseRootNode != null)
			{
				result.ReleaseRootNode = TpkUnityNode.Convert(source.ReleaseRootNode, buffer, 0, 0, out var _);
			}
			return result;
		}
	}
}
