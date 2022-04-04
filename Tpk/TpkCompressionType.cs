namespace AssetRipper.Tpk
{
	public enum TpkCompressionType : byte
	{
		None,
		Lz4,
		Lzma,
#if DEBUG
		Brotli,
#endif
	}
}
