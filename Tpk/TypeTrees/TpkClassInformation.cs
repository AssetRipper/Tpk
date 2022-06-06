using VersionClassPair = System.Collections.Generic.KeyValuePair<
	AssetRipper.VersionUtilities.UnityVersion,
	AssetRipper.Tpk.TypeTrees.TpkUnityClass?>;

namespace AssetRipper.Tpk.TypeTrees
{
	/// <summary>
	/// Tracks changes in a single class id
	/// </summary>
	public class TpkClassInformation
	{
		/// <summary>
		/// The class id number for this class set
		/// </summary>
		public int ID { get; private set; }
		/// <summary>
		/// Minimum version : Class<br/>
		/// These sequential versions form ranges. The class at index n is valid for version n to version n+1 exclusive.<br/>
		/// If the class is null, it is not present in that range.
		/// </summary>
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
				UnityVersion version = reader.ReadUnityVersion();
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
