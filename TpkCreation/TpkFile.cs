using AssetRipper.TpkCreation.Exceptions;
using AssetRipper.TpkCreation.TypeTrees;
using AssetRipper.TpkCreation.Utilities;
using K4os.Compression.LZ4;

namespace AssetRipper.TpkCreation
{
	public sealed class TpkFile
	{
		public const uint TpkMagicBytes = 0x4B504C43; // CLPK in binary
		public const byte TpkVersionNumber = 2;

		public TpkCompressionType CompressionType { get; private set; }
		public TpkDataType DataType { get; private set; }
		public int CompressedSize { get; private set; }
		public int DecompressedSize { get; private set; }
		public byte[] CompressedBytes { get; private set; } = Array.Empty<byte>();

		public TpkFile(BinaryReader reader)
		{
			Read(reader);
		}

		public TpkFile(byte[] uncompressedData, TpkCompressionType compressionType)
		{
			StoreData(uncompressedData, compressionType);
		}

		public TpkFile(TpkDataBlob blob, TpkCompressionType compressionType)
		{
			StoreDataBlob(blob, compressionType);
		}

		public void Read(BinaryReader reader)
		{
			uint magic = reader.ReadUInt32();
			if (magic != TpkMagicBytes)
			{
				throw new InvalidDataException($"Magic bytes do not match: {magic:X}");
			}

			byte versionNumber = reader.ReadByte();
			if (versionNumber != TpkVersionNumber)
			{
				throw new NotSupportedException($"Version number not supported: {versionNumber}");
			}

			CompressionType = (TpkCompressionType)reader.ReadByte();
			DataType = (TpkDataType)reader.ReadByte();
			reader.ReadByte();//Reserved byte
			reader.ReadUInt32();//Reserved bytes
			CompressedSize = reader.ReadInt32();
			DecompressedSize = reader.ReadInt32();
			CompressedBytes = reader.ReadBytes(CompressedSize);
			if (CompressedBytes.Length != CompressedSize)
			{
				throw new InvalidByteCountException(CompressedBytes.Length, CompressedSize);
			}
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(TpkMagicBytes);
			writer.Write(TpkVersionNumber);
			writer.Write((byte)CompressionType);
			writer.Write((byte)DataType);
			writer.Write((byte)0);//Reserved byte
			writer.Write((uint)0);//Reserved bytes
			writer.Write(CompressedSize);
			writer.Write(DecompressedSize);
			writer.Write(CompressedBytes);
		}

		public TpkDataBlob GetDataBlob()
		{
			byte[] data = GetDecompressedData();
			using MemoryStream memoryStream = new MemoryStream(data);
			using SealedBinaryReader reader = new SealedBinaryReader(memoryStream);
			TpkDataBlob result = DataType switch
			{
				TpkDataType.TypeTreeInformation => new TpkTypeTreeBlob(),
				_ => throw new NotSupportedException($"Data type {DataType} not supported"),
			};
			result.Read(reader);
			return result;
		}

		public void StoreDataBlob(TpkDataBlob blob, TpkCompressionType compressionType)
		{
			using MemoryStream memoryStream = new MemoryStream();
			using SealedBinaryWriter writer = new SealedBinaryWriter(memoryStream);
			blob.Write(writer);
			StoreData(memoryStream.ToArray(), compressionType);
			DataType = blob.DataType;
		}

		public byte[] GetDecompressedData()
		{
			return CompressionType switch
			{
				TpkCompressionType.None => CompressedBytes,
				TpkCompressionType.Lz4 => DecompressWithLz4(),
				_ => throw new NotSupportedException($"Compression type {CompressionType} is not supported"),
			};
		}

		private byte[] DecompressWithLz4()
		{
			byte[] decompressedBytes = new byte[DecompressedSize];
			LZ4Codec.Decode(CompressedBytes, decompressedBytes);
			return decompressedBytes;
		}

		private void StoreData(byte[] uncompressedData, TpkCompressionType compressionType)
		{
			CompressionType = compressionType;
			switch (compressionType)
			{
				case TpkCompressionType.None:
					StoreWithNoCompression(uncompressedData);
					return;
				case TpkCompressionType.Lz4:
					CompressWithLz4(uncompressedData);
					return;
				default:
					throw new ArgumentOutOfRangeException(nameof(compressionType));
			}
		}

		private void CompressWithLz4(byte[] uncompressedBytes)
		{
			byte[] buffer = new byte[LZ4Codec.MaximumOutputSize(uncompressedBytes.Length)];
			int compressedSize = LZ4Codec.Encode(uncompressedBytes, buffer, LZ4Level.L12_MAX);

			if (compressedSize < 0)
				throw new Exception("Could not compress data");

			CompressedBytes = new byte[compressedSize];
			Array.Copy(buffer, CompressedBytes, compressedSize);
			CompressedSize = compressedSize;
			DecompressedSize = uncompressedBytes.Length;
		}

		private void StoreWithNoCompression(byte[] uncompressedBytes)
		{
			CompressedBytes = uncompressedBytes;
			CompressedSize = uncompressedBytes.Length;
			DecompressedSize = uncompressedBytes.Length;
		}
	}
}
