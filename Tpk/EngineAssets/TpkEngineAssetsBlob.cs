using AssetRipper.Tpk.Extensions;

namespace AssetRipper.Tpk.EngineAssets;

public sealed class TpkEngineAssetsBlob : TpkDataBlob
{
	public DateTime CreationTime { get; set; }
	public List<UnityVersion> Versions { get; } = new();
	public List<KeyValuePair<UnityVersion, string>> Data { get; } = new();

	public override TpkDataType DataType => TpkDataType.EngineAssets;

	public override void Read(BinaryReader reader)
	{
		long creationTimeBinary = reader.ReadInt64();
		CreationTime = DateTime.FromBinary(creationTimeBinary);

		int versionCount = reader.ReadInt32();
		Versions.Clear();
		Versions.Capacity = versionCount;
		for(int i = 0; i < versionCount; i++)
		{
			Versions.Add(reader.ReadUnityVersion());
		}

		int dataCount = reader.ReadInt32();
		Data.Clear();
		Data.Capacity = dataCount;
		for(int i = 0; i < dataCount; i++)
		{
			UnityVersion version = reader.ReadUnityVersion();
			string json = reader.ReadString();
			Data.Add(new KeyValuePair<UnityVersion, string>(version, json));
		}
	}

	public override void Write(BinaryWriter writer)
	{
		writer.Write(CreationTime.ToBinary());

		int versionCount = Versions.Count;
		writer.Write(versionCount);
		for(int i = 0; i < versionCount; i++)
		{
			writer.Write(Versions[i]);
		}

		int dataCount = Data.Count;
		writer.Write(dataCount);
		for(int i = 0; i < dataCount; i++)
		{
			Write(writer, Data[i]);
		}

		static void Write(BinaryWriter writer, KeyValuePair<UnityVersion, string> pair)
		{
			writer.Write(pair.Key);
			writer.Write(pair.Value);
		}
	}
}
