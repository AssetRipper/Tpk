namespace AssetRipper.TpkCreation.Exceptions
{
	public sealed class InvalidByteCountException : Exception
	{
		public int BytesRead { get; }
		public int BytesExpected { get; }

		public InvalidByteCountException(int bytesRead, int bytesExpected) : base($"Read {bytesRead} but expected {bytesExpected}")
		{
			BytesRead = bytesRead;
			BytesExpected = bytesExpected;
		}
	}
}
