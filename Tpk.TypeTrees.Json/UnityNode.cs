using AssetRipper.HashAlgorithms;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AssetRipper.Tpk.TypeTrees.Json
{
	public sealed class UnityNode
	{
		[AllowNull]
		public string TypeName { get; set => field = value ?? ""; } = "";
		/// <summary>
		/// The original type name as obtained from the json file
		/// </summary>
		[JsonIgnore]
		[AllowNull]
		public string OriginalTypeName
		{
			get => string.IsNullOrEmpty(originalTypeName) ? TypeName : originalTypeName;
			set => originalTypeName = value ?? "";
		}
		[AllowNull]
		public string Name { get; set => field = value ?? ""; } = "";
		/// <summary>
		/// The original name as obtained from the json file
		/// </summary>
		[JsonIgnore]
		[AllowNull]
		public string OriginalName
		{
			get => string.IsNullOrEmpty(originalName) ? Name : originalName;
			set => originalName = value ?? "";
		}
		public int ByteSize { get; set; }
		public short Version { get; set; }
		public byte TypeFlags { get; set; }
		public uint MetaFlag { get; set; }
		[AllowNull]
		public List<UnityNode> SubNodes { get; set => field = value ?? []; } = [];

		private string originalTypeName = "";
		private string originalName = "";

		/// <summary>
		/// Deep clones a node and all its subnodes<br/>
		/// Warning: Deep cloning a node with a circular hierarchy will cause an endless loop
		/// </summary>
		/// <returns>The new node</returns>
		public UnityNode DeepClone()
		{
			return new UnityNode
			{
				TypeName = TypeName,
				originalTypeName = originalTypeName,
				Name = Name,
				originalName = originalName,
				ByteSize = ByteSize,
				Version = Version,
				TypeFlags = TypeFlags,
				MetaFlag = MetaFlag,
				SubNodes = SubNodes.ConvertAll(x => x.DeepClone()),
			};
		}

		public string ToJsonString(bool indented = false)
		{
			return indented
				? JsonSerializer.Serialize(this, UnityInfoSerializerContext.WriteIndentedContext.UnityNode)
				: JsonSerializer.Serialize(this, UnityInfoSerializerContext.WriteNotIndentedContext.UnityNode);
		}

		public static UnityNode? FromJsonString(string jsonString)
		{
			return JsonSerializer.Deserialize(jsonString, UnityInfoSerializerContext.Default.UnityNode);
		}

		/// <summary>
		/// Computes the hash of the node and all its subnodes.
		/// </summary>
		/// <remarks>
		/// This is used in m_OldTypeHash.
		/// </remarks>
		/// <returns>A 16-byte array containing the hash.</returns>
		public byte[] ComputeHash()
		{
			MD4 md4 = new();
			TransformUnityNodeAndChildren(md4, this);
			return md4.ComputeHash(Array.Empty<byte>());

			static void TransformUnityNodeAndChildren(MD4 md4, UnityNode node)
			{
				TransformString(md4, node.TypeName);
				TransformString(md4, node.Name);
				TransformInt32(md4, node.ByteSize);
				TransformInt32(md4, node.TypeFlags);
				TransformInt32(md4, node.Version);
				TransformInt32(md4, (int)(node.MetaFlag & 0x4000));

				for (int i = 0; i < node.SubNodes.Count; i++)
				{
					TransformUnityNodeAndChildren(md4, node.SubNodes[i]);
				}
			}
			static void TransformString(MD4 md4, string value)
			{
				byte[] data = Encoding.UTF8.GetBytes(value);
				md4.TransformBlock(data, 0, data.Length, null, default);
			}
			static void TransformInt32(MD4 md4, int value)
			{
				byte[] data = ArrayPool<byte>.Shared.Rent(sizeof(int));
				BinaryPrimitives.WriteInt32LittleEndian(data, value);
				md4.TransformBlock(data, 0, sizeof(int), null, default);
				ArrayPool<byte>.Shared.Return(data);
			}
		}
	}
}