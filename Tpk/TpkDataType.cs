using AssetRipper.Tpk.EngineAssets;
using AssetRipper.Tpk.TypeTrees;

namespace AssetRipper.Tpk
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
		/// <summary>
		/// Custom json data
		/// </summary>
		Json = 3,
		/// <summary>
		/// Lists of reference assemblies in the editor
		/// </summary>
		ReferenceAssemblies = 4,
		/// <summary>
		/// Lists of default Unity assets and their export ids
		/// </summary>
		EngineAssets = 5,
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
				TpkDataType.Json => new TpkJsonBlob(),
				TpkDataType.EngineAssets => new TpkEngineAssetsBlob(),
				_ => throw new NotSupportedException($"Data type {dataType} not supported"),
			};
		}

		public static TpkDataBlob ToBlob(this TpkDataType dataType, byte[] blobData)
		{
			TpkDataBlob blob = dataType.ToBlob();
			blob.Read(blobData);
			return blob;
		}

		public static TpkDataBlob ToBlob(this TpkDataType dataType, Stream blobDataStream)
		{
			TpkDataBlob blob = dataType.ToBlob();
			blob.Read(blobDataStream);
			return blob;
		}
	}
}
