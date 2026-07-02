using AssetRipper.Tpk.Extensions;
using AssetRipper.Tpk.Shared;

namespace AssetRipper.Tpk.TypeTrees
{
	/// <summary>
	/// Handles storage of the common type tree strings and their offsets, ie AABB
	/// </summary>
	public sealed class TpkCommonString
	{
		/// <summary>
		/// An entry in <see cref="TpkCommonString"/>
		/// </summary>
		/// <param name="Offset">The byte offset of the string</param>
		/// <param name="String">The index of the string in <see cref="TpkStringBuffer"/></param>
		public readonly record struct Entry(ushort Offset, ushort String)
		{
			public Entry(ushort offset, string @string, TpkStringBuffer buffer) : this(offset, buffer.AddString(@string))
			{
			}

			public string ToString(TpkStringBuffer buffer)
			{
				return buffer[String];
			}

			public static Entry Read(BinaryReader reader)
			{
				ushort offset = reader.ReadUInt16();
				ushort stringIndex = reader.ReadUInt16();
				return new Entry(offset, stringIndex);
			}

			public void Write(BinaryWriter writer)
			{
				writer.Write(Offset);
				writer.Write(String);
			}
		}

		/// <summary>
		/// Unity version : string count<br/>
		/// Sequential by ascending Unity version
		/// </summary>
		public List<KeyValuePair<UnityVersion, Entry[]>> VersionInformation { get; } = new();

		public void Add(UnityVersion version, Entry[] entries)
		{
			VersionInformation.Add(new KeyValuePair<UnityVersion, Entry[]>(version, entries));
		}

		public Entry[] GetEntries(UnityVersion exactVersion)
		{
			if (VersionInformation.Count == 0)
			{
				return [];
			}
			for (int i = 1; i < VersionInformation.Count; i++)
			{
				if (exactVersion < VersionInformation[i].Key)
				{
					return VersionInformation[i - 1].Value;
				}
			}
			return VersionInformation[^1].Value;
		}

		public void Read(BinaryReader reader)
		{
			int versionCount = reader.ReadInt32();
			VersionInformation.Clear();
			VersionInformation.Capacity = versionCount;
			for (int i = 0; i < versionCount; i++)
			{
				UnityVersion version = reader.ReadUnityVersion();
				int entryCount = reader.ReadInt32();
				Entry[] entries = new Entry[entryCount];
				for (int j = 0; j < entryCount; j++)
				{
					entries[j] = Entry.Read(reader);
				}
				VersionInformation.Add(new KeyValuePair<UnityVersion, Entry[]>(version, entries));
			}
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(VersionInformation.Count);
			for (int i = 0; i < VersionInformation.Count; i++)
			{
				writer.Write(VersionInformation[i].Key);
				Entry[] entries = VersionInformation[i].Value;
				writer.Write(entries.Length);
				for (int j = 0; j < entries.Length; j++)
				{
					entries[j].Write(writer);
				}
			}
		}
	}
}
