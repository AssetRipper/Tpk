namespace AssetRipper.Tpk.TypeTrees
{
	[Flags]
	public enum TpkUnityClassFlags : byte
	{
		/// <summary>
		/// None of the flags apply to this class
		/// </summary>
		None = 0,
		/// <summary>
		/// Is the class abstract?
		/// </summary>
		IsAbstract = 1,
		/// <summary>
		/// Is the class sealed? Not necessarily accurate.
		/// </summary>
		IsSealed = 2,
		/// <summary>
		/// Does the class only appear in the editor?
		/// </summary>
		IsEditorOnly = 4,
		/// <summary>
		/// Does the class only appear in game files? Not currently used
		/// </summary>
		IsReleaseOnly = 8,
		/// <summary>
		/// Is the class stripped?
		/// </summary>
		IsStripped = 16,
		/// <summary>
		/// Not currently used
		/// </summary>
		Reserved = 32,
		/// <summary>
		/// Does the class have an editor root node?
		/// </summary>
		HasEditorRootNode = 64,
		/// <summary>
		/// Does the class have a release root node?
		/// </summary>
		HasReleaseRootNode = 128,
	}

	public static class TpkUnityClassFlagsExtensions
	{
		public static bool IsAbstract(this TpkUnityClassFlags flags)
		{
			return (flags & TpkUnityClassFlags.IsAbstract) != 0;
		}

		public static bool IsSealed(this TpkUnityClassFlags flags)
		{
			return (flags & TpkUnityClassFlags.IsSealed) != 0;
		}

		public static bool IsEditorOnly(this TpkUnityClassFlags flags)
		{
			return (flags & TpkUnityClassFlags.IsEditorOnly) != 0;
		}

		public static bool IsReleaseOnly(this TpkUnityClassFlags flags)
		{
			return (flags & TpkUnityClassFlags.IsReleaseOnly) != 0;
		}

		public static bool IsStripped(this TpkUnityClassFlags flags)
		{
			return (flags & TpkUnityClassFlags.IsStripped) != 0;
		}

		public static bool HasEditorRootNode(this TpkUnityClassFlags flags)
		{
			return (flags & TpkUnityClassFlags.HasEditorRootNode) != 0;
		}

		public static bool HasReleaseRootNode(this TpkUnityClassFlags flags)
		{
			return (flags & TpkUnityClassFlags.HasReleaseRootNode) != 0;
		}
	}
}
