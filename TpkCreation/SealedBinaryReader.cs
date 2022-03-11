using System.Text;

namespace AssetRipper.TpkCreation
{
	internal sealed class SealedBinaryReader : BinaryReader
	{
		public SealedBinaryReader(Stream input) : base(input)
		{
		}

		public SealedBinaryReader(Stream input, Encoding encoding) : base(input, encoding)
		{
		}

		public SealedBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
		{
		}
	}
}
