namespace AssetRipper.Tpk.Extensions;

/// <summary>
/// Unity version extension methods for <see cref="BinaryReader"/>
/// </summary>
internal static class BinaryReaderExtensions
{
	/// <summary>
	/// Reads a Unity version from the stream
	/// </summary>
	/// <param name="reader">A binary reader</param>
	/// <returns>The read Unity version</returns>
	public static UnityVersion ReadUnityVersion(this BinaryReader reader)
	{
		return UnityVersion.FromBits(reader.ReadUInt64());
	}
}
