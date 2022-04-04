using AssetRipper.Tpk.Shared;

namespace AssetRipper.Tpk.TypeTrees
{
	public sealed class TpkTypeTreeBlob : TpkDataBlob
	{
		public DateTime CreationTime { get; set; }
		public List<UnityVersion> Versions { get; } = new();
		public List<TpkClassInformation> ClassInfo { get; } = new();
		public TpkCommonString CommonString { get; } = new();
		public TpkUnityNodeBuffer NodeBuffer { get; } = new();
		public TpkStringBuffer StringBuffer { get; } = new();

		public override TpkDataType DataType => TpkDataType.TypeTreeInformation;

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

			int classCount = reader.ReadInt32();
			ClassInfo.Clear();
			ClassInfo.Capacity = classCount;
			for(int i = 0; i < classCount; i++)
			{
				TpkClassInformation classInformation = new();
				classInformation.Read(reader);
				ClassInfo.Add(classInformation);
			}

			CommonString.Read(reader);

			NodeBuffer.Read(reader);

			StringBuffer.Read(reader);
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

			int classCount = ClassInfo.Count;
			writer.Write(classCount);
			for(int i = 0; i < classCount; i++)
			{
				ClassInfo[i].Write(writer);
			}

			CommonString.Write(writer);

			NodeBuffer.Write(writer);

			StringBuffer.Write(writer);
		}
	}
}
