namespace AssetRipper.TypeTreeCompression.Tpk
{
	public sealed class TpkStringBuffer
	{
		private List<string> Strings { get; } = new();

		/// <summary>
		/// Ensures a string is in the buffer
		/// </summary>
		/// <param name="str">The string to be added</param>
		/// <returns>The index at which that string appears</returns>
		public ushort AddString(string str)
		{
			int index = Strings.IndexOf(str);
			if (index == -1)
			{
				index = Strings.Count;
				Strings.Add(str);
			}
			return (ushort)index;
		}

		public string this[int index]
		{
			get => Strings[index];
		}

		public int Count => Strings.Count;

		public void Read(BinaryReader reader)
		{
			Strings.Clear();
			int stringCount = reader.ReadInt32();
			for (int i = 0; i < stringCount; i++)
			{
				Strings.Add(reader.ReadString());
			}
		}

		public void Write(BinaryWriter writer)
		{
			int count = Strings.Count;
			writer.Write(count);
			for(int i = 0; i < count; i++)
			{
				writer.Write(Strings[i]);
			}
		}
	}
}
