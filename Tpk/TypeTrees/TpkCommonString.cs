using AssetRipper.Tpk.Shared;

namespace AssetRipper.Tpk.TypeTrees
{
    /// <summary>
    /// Handles storage of the common type tree strings and their offsets, ie AABB
    /// </summary>
    public sealed class TpkCommonString
    {
        /// <summary>
        /// Unity version : string count<br/>
        /// Sequential by ascending Unity version
        /// </summary>
        public List<KeyValuePair<UnityVersion, byte>> VersionInformation { get; } = new();
        public List<ushort> StringBufferIndices { get; } = new();

        public void Add(UnityVersion version, byte count)
        {
            VersionInformation.Add(new KeyValuePair<UnityVersion, byte>(version, count));
        }

        public void SetIndices(TpkStringBuffer buffer, List<string> strings)
        {
            int count = strings.Count;
            StringBufferIndices.Clear();
            StringBufferIndices.Capacity = count;
            for (int i = 0; i < count; i++)
            {
                StringBufferIndices.Add(buffer.AddString(strings[i]));
            }
        }

        public byte GetCount(UnityVersion exactVersion)
        {
            if (VersionInformation.Count == 0)
            {
                return 0;
            }
            for (int i = 1; i < VersionInformation.Count; i++)
            {
                if (exactVersion < VersionInformation[i].Key)
                {
                    return VersionInformation[i - 1].Value;
                }
            }
            return VersionInformation[VersionInformation.Count - 1].Value;
        }

        public string[] GetStrings(TpkStringBuffer buffer)
        {
            int length = StringBufferIndices.Count;
            string[] strings = new string[length];
            for (int i = 0; i < length; i++)
            {
                strings[i] = buffer[StringBufferIndices[i]];
            }
            return strings;
        }

        public void Read(BinaryReader reader)
        {
            int versionCount = reader.ReadInt32();
            VersionInformation.Clear();
            VersionInformation.Capacity = versionCount;
            for (int i = 0; i < versionCount; i++)
            {
                UnityVersion version = reader.ReadUnityVersion();
                byte stringCount = reader.ReadByte();
                VersionInformation.Add(new KeyValuePair<UnityVersion, byte>(version, stringCount));
            }
            int indicesCount = reader.ReadInt32();
            StringBufferIndices.Clear();
            StringBufferIndices.Capacity = indicesCount;
            for (int j = 0; j < indicesCount; j++)
            {
                StringBufferIndices.Add(reader.ReadUInt16());
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(VersionInformation.Count);
            for (int i = 0; i < VersionInformation.Count; i++)
            {
                writer.Write(VersionInformation[i].Key);
                writer.Write(VersionInformation[i].Value);
            }
            writer.Write(StringBufferIndices.Count);
            for (int i = 0; i < StringBufferIndices.Count; i++)
            {
                writer.Write(StringBufferIndices[i]);
            }
        }
    }
}
