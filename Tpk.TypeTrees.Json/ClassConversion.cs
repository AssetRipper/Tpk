using AssetRipper.Tpk.Shared;

namespace AssetRipper.Tpk.TypeTrees.Json
{
	public static class ClassConversion
	{
		public static TpkUnityClass Convert(UnityClass source, TpkStringBuffer stringBuffer, TpkUnityNodeBuffer nodeBuffer)
		{
			TpkUnityClass result = new TpkUnityClass();
			result.Name = stringBuffer.AddString(source.Name);
			result.Namespace = stringBuffer.AddString(source.Namespace);
			result.FullName = stringBuffer.AddString(source.FullName);
			result.Module = stringBuffer.AddString(source.Module);
			result.Base = stringBuffer.AddString(source.Base);
			int derivedCount = source.Derived.Count;
			result.Derived = new ushort[derivedCount];
			for (int i = 0; i < derivedCount; i++)
			{
				result.Derived[i] = stringBuffer.AddString(source.Derived[i]);
			}
			result.DescendantCount = source.DescendantCount;
			result.Flags = GetFlags(source);
			if (source.EditorRootNode != null)
			{
				result.EditorRootNode = NodeConversion.Convert(source.EditorRootNode, stringBuffer, nodeBuffer);
			}
			if (source.ReleaseRootNode != null)
			{
				result.ReleaseRootNode = NodeConversion.Convert(source.ReleaseRootNode, stringBuffer, nodeBuffer);
			}
			return result;
		}

		public static UnityClass Convert(TpkUnityClass source, TpkStringBuffer stringBuffer, TpkUnityNodeBuffer nodeBuffer)
		{
			UnityClass result = new UnityClass();
			result.Name = stringBuffer[source.Name];
			result.Namespace = stringBuffer[source.Namespace];
			result.FullName = stringBuffer[source.FullName];
			result.Module = stringBuffer[source.Module];
			//TypeID gets set elsewhere
			result.Base = stringBuffer[source.Base];
			result.Derived = new List<string>(source.Derived.Length);
			for (int i = 0; i < source.Derived.Length; i++)
			{
				result.Derived.Add(stringBuffer[source.Derived[i]]);
			}
			result.DescendantCount = source.DescendantCount;
			result.Size = -1;
			//Type index is ignored
			result.IsAbstract = source.Flags.IsAbstract();
			result.IsSealed = source.Flags.IsSealed();
			result.IsEditorOnly = source.Flags.IsEditorOnly();
			result.IsReleaseOnly = source.Flags.IsReleaseOnly();
			result.IsStripped = source.Flags.IsStripped();
			if (source.EditorRootNode != ushort.MaxValue)
			{
				result.EditorRootNode = NodeConversion.Convert(nodeBuffer[source.EditorRootNode], stringBuffer, nodeBuffer, 0, 0, out var _);
			}
			if (source.ReleaseRootNode != ushort.MaxValue)
			{
				result.ReleaseRootNode = NodeConversion.Convert(nodeBuffer[source.ReleaseRootNode], stringBuffer, nodeBuffer, 0, 0, out var _);
			}
			return result;
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
	}
}