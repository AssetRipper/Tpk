using VersionClassPair = System.Collections.Generic.KeyValuePair<
	AssetRipper.VersionUtilities.CompactUnityVersion32,
	AssetRipper.TypeTreeCompression.Tpk.TpkUnityClass?>;

namespace AssetRipper.TypeTreeCompression.Tpk
{
	public class TpkClassInformation
	{
		public int ID { get; private set; }
		public List<VersionClassPair> Classes { get; } = new();

		public TpkClassInformation() { }

		public TpkClassInformation(int id)
		{
			ID = id;
		}

		public void Read(BinaryReader reader)
		{
			ID = reader.ReadInt32();
			int count = reader.ReadInt32();
			Classes.Clear();
			Classes.Capacity = count;
			for(int i = 0; i < count; i++)
			{
				CompactUnityVersion32 version = reader.ReadCompactUnityVersion32();
				bool hasClassData = reader.ReadBoolean();
				TpkUnityClass? classData = null;
				if (hasClassData)
				{
					classData = new();
					classData.Read(reader);
				}
				Classes.Add(new VersionClassPair(version, classData));
			}
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(ID);
			int count = Classes.Count;
			writer.Write(count);
			for(int i = 0; i < count; i++)
			{
				writer.Write(Classes[i].Key);
				TpkUnityClass? tpkUnityClass = Classes[i].Value;
				writer.Write(tpkUnityClass != null);
				if (tpkUnityClass != null)
				{
					tpkUnityClass.Write(writer);
				}
			}
		}
	}
}
