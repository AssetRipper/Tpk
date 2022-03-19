using System.Text;

namespace AssetRipper.TpkCreation.Utilities
{
	internal sealed class SealedBinaryWriter : BinaryWriter
	{
		public SealedBinaryWriter()
		{
		}

		public SealedBinaryWriter(Stream output) : base(output)
		{
		}

		public SealedBinaryWriter(Stream output, Encoding encoding) : base(output, encoding)
		{
		}

		public SealedBinaryWriter(Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen)
		{
		}
	}
}
