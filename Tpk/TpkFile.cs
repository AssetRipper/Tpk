using AssetRipper.Tpk.Compression;
using AssetRipper.Tpk.Exceptions;

namespace AssetRipper.Tpk
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

		private TpkFile()
		{
		}

		public static TpkFile FromFile(string path)
		{
			using FileStream stream = File.OpenRead(path);
			return FromStream(stream);
		}

		public static TpkFile FromStream(Stream stream)
		{
			TpkFile file = new TpkFile();
			using BinaryReader reader = new BinaryReader(stream);
			file.Read(reader);
			return file;
		}

		public static TpkFile FromData(byte[] uncompressedData, TpkCompressionType compressionType)
		{
			TpkFile file = new TpkFile();
			file.StoreData(uncompressedData, compressionType);
			return file;
		}

		public static TpkFile FromBlob(TpkDataBlob blob, TpkCompressionType compressionType)
		{
			TpkFile file = new TpkFile();
			file.StoreDataBlob(blob, compressionType);
			return file;
		}

		private void Read(BinaryReader reader)
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

		private void Write(BinaryWriter writer)
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

		public byte[] WriteToMemory()
		{
			using MemoryStream memoryStream = new MemoryStream();
			using BinaryWriter writer = new BinaryWriter(memoryStream);
			Write(writer);
			return memoryStream.ToArray();
		}

		public void WriteToFile(string path)
		{
			using FileStream stream = File.Create(path);
			using BinaryWriter writer = new BinaryWriter(stream);
			Write(writer);
		}

		public TpkDataBlob GetDataBlob()
		{
			Stream stream = GetDecompressedStream();
			return DataType.ToBlob(stream);
			//byte[] data = GetDecompressedData();
			//return DataType.ToBlob(data);
		}

		private void StoreDataBlob(TpkDataBlob blob, TpkCompressionType compressionType)
		{
			using MemoryStream memoryStream = new MemoryStream();
			using BinaryWriter writer = new BinaryWriter(memoryStream);
			blob.Write(writer);
			StoreData(memoryStream.ToArray(), compressionType);
			DataType = blob.DataType;
		}

		public byte[] GetDecompressedData()
		{
			return CompressionType switch
			{
				TpkCompressionType.None => CompressedBytes,
				TpkCompressionType.Lz4 => Lz4Handler.Decompress(CompressedBytes, DecompressedSize),
				TpkCompressionType.Lzma => LzmaHandler.Decompress(CompressedBytes, DecompressedSize),
				TpkCompressionType.Brotli => BrotliHandler.Decompress(CompressedBytes),
				_ => throw new NotSupportedException($"Compression type {CompressionType} is not supported"),
			};
		}

		public Stream GetDecompressedStream()
		{
			return CompressionType switch
			{
				TpkCompressionType.None => new MemoryStream(CompressedBytes),
				TpkCompressionType.Lz4 => new MemoryStream(Lz4Handler.Decompress(CompressedBytes, DecompressedSize)),
				TpkCompressionType.Lzma => LzmaHandler.DecompressStream(CompressedBytes, DecompressedSize),
				TpkCompressionType.Brotli => BrotliHandler.Decompress(new MemoryStream(CompressedBytes)),
				_ => throw new NotSupportedException($"Compression type {CompressionType} is not supported"),
			};
		}

		private void StoreData(byte[] uncompressedBytes, TpkCompressionType compressionType)
		{
			CompressionType = compressionType;
			CompressedBytes = compressionType switch
			{
				TpkCompressionType.None => uncompressedBytes,
				TpkCompressionType.Lz4 => Lz4Handler.Compress(uncompressedBytes),
				TpkCompressionType.Lzma => LzmaHandler.Compress(uncompressedBytes),
				TpkCompressionType.Brotli => BrotliHandler.Compress(uncompressedBytes),
				_ => throw new ArgumentOutOfRangeException(nameof(compressionType)),
			};
			CompressedSize = CompressedBytes.Length;
			DecompressedSize = uncompressedBytes.Length;
		}
	}
}
