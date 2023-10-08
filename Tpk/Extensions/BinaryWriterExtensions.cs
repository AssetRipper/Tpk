namespace AssetRipper.Tpk.Extensions;

/// <summary>
/// Unity version extension methods for <see cref="BinaryWriter"/>
/// </summary>
internal static class BinaryWriterExtensions
{
	/// <summary>
	/// Write a Unity version to the stream
	/// </summary>
	/// <param name="writer">A binary writer</param>
	/// <param name="version">A Unity version</param>
	public static void Write(this BinaryWriter writer, UnityVersion version)
	{
		writer.Write(version.GetBits());
	}
}
