using AssetRipper.TpkCreation.TypeTrees;

namespace AssetRipper.TpkCreation
{
	public enum TpkDataType : byte
	{
		/// <summary>
		/// Information about the structure of Unity assets
		/// </summary>
		TypeTreeInformation = 0,
		/// <summary>
		/// A collection of blobs
		/// </summary>
		Collection = 1,
		/// <summary>
		/// A file system archive
		/// </summary>
		FileSystem = 2,
	}

	public static class TpkDataTypeExtensions
	{
		public static TpkDataBlob ToBlob(this TpkDataType dataType)
		{
			return dataType switch
			{
				TpkDataType.TypeTreeInformation => new TpkTypeTreeBlob(),
				TpkDataType.Collection => new TpkCollectionBlob(),
				TpkDataType.FileSystem => new TpkFileSystemBlob(),
				_ => throw new NotSupportedException($"Data type {dataType} not supported"),
			};
		}

		public static TpkDataBlob ToBlob(this TpkDataType dataType, byte[] blobData)
		{
			TpkDataBlob blob = dataType.ToBlob();
			blob.Read(blobData);
			return blob;
		}
	}
}
