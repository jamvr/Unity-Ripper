using System.Collections.Generic;
using UnityRipper.Classes.Meshes;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class Mesh : NamedObject
	{
		public Mesh(AssetInfo assetInfo) :
			base(assetInfo)
		{
			CompressedMesh = new CompressedMesh(AssetsFile);

			if (IsReadBlendShapes)
			{
				Shapes = new BlendShapeData(AssetsFile);
			}
			if (IsReadLocalAABB)
			{
				LocalAABB = new AABB();
			}
			if (IsReadCollision)
			{
				CollisionData = new CollisionMeshData();
			}
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			if(IsReadIndicesUsage)
			{
				IsUse16bitIndices = stream.ReadBoolean();
				stream.AlignStream(AlignType.Align4);
			}

			if(IsIndexBufferFirst)
			{
				m_indexBuffer = stream.ReadByteArray();
				stream.AlignStream(AlignType.Align4);
			}

			m_subMeshes = stream.ReadArray(() => new SubMesh(AssetsFile));

			if(IsReadBlendShapes)
			{
				Shapes.Read(stream);
				if (IsBindPosesFirst)
				{
					m_bindPoses = stream.ReadArray<Matrix4x4f>();
					m_boneNameHashes = stream.ReadUInt32Array();
					RootBoneNameHash = stream.ReadUInt32();
				}
			}

			if(!IsIndexBufferFirst)
			{
				MeshCompression = stream.ReadByte();
				if(IsReadBoolFlags)
				{
					if(IsReadStreamCompression)
					{
						StreamCompression = stream.ReadByte();
					}
					IsReadable = stream.ReadBoolean();
					KeepVertices = stream.ReadBoolean();
					KeepIndices = stream.ReadBoolean();
				}
				stream.AlignStream(AlignType.Align4);

				if (IsReadIndexFormat)
				{
					IndexFormat = stream.ReadInt32();
				}

				m_indexBuffer = stream.ReadByteArray();
				stream.AlignStream(AlignType.Align4);
			}

			if(IsReadVertices)
			{
				m_vertices = stream.ReadArray<Vector3f>();
				m_bindPoses = stream.ReadArray<Matrix4x4f>();

#warning TODO:
				throw new System.NotImplementedException();
			}
			else
			{
				m_skin = stream.ReadArray<BoneWeights4>();
				stream.AlignStream(AlignType.Align4);

				if(!IsBindPosesFirst)
				{
					m_bindPoses = stream.ReadArray<Matrix4x4f>();
				}

				VertexData = new VertexData(AssetsFile, MeshCompression);
				VertexData.Read(stream);
				CompressedMesh.Read(stream);
				if(IsReadLocalAABB)
				{
					LocalAABB.Read(stream);
				}
				MeshUsageFlags = stream.ReadInt32();

				if(IsReadCollision)
				{
					CollisionData.Read(stream);
				}
			}
		}

		public override void Reset()
		{
			base.Reset();

			if(!IsReadIndicesUsage)
			{
				IsUse16bitIndices = true;
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
#warning TODO: provide for null values

			YAMLMappingNode node = base.ExportYAMLRoot();
			node.AddSerializedVersion(SerializedVersion);			
			node.Add("m_SubMeshes", SubMeshes.ExportYAML());
			node.Add("m_Shapes", Shapes.ExportYAML());
			node.Add("m_BindPose", BindPoses.ExportYAML());
#warning ???
			node.Add("m_BoneNames", YAMLSequenceNode.Empty);
			node.Add("m_BoneNameHashes", BoneNameHashes.ExportYAML(false));
#warning ???
			node.Add("m_RootBoneName", YAMLScalarNode.Empty);
			node.Add("m_RootBoneNameHash", RootBoneNameHash);
			node.Add("m_MeshCompression", MeshCompression);
			node.Add("m_IsReadable", IsReadable);
			node.Add("m_KeepVertices", KeepVertices);
			node.Add("m_KeepIndices", KeepIndices);
			node.Add("m_IndexBuffer", IndexBuffer.ExportYAML());
			node.Add("m_Skin", Skin.ExportYAML());
			node.Add("m_VertexData", VertexData.ExportYAML());
			node.Add("m_CompressedMesh", CompressedMesh.ExportYAML());
			node.Add("m_LocalAABB", LocalAABB.ExportYAML());
			node.Add("m_MeshUsageFlags", MeshUsageFlags);
			if (IsReadCollision)
			{
				node.Add("m_BakedConvexCollisionMesh", CollisionData.BakedConvexCollisionMesh.ExportYAML());
				node.Add("m_BakedTriangleCollisionMesh", CollisionData.BakedTriangleCollisionMesh.ExportYAML());
			}
			else
			{
				node.Add("m_BakedConvexCollisionMesh", ArrayExtensions.EmptyBytes.ExportYAML());
				node.Add("m_BakedTriangleCollisionMesh", ArrayExtensions.EmptyBytes.ExportYAML());
			}
#warning ???
			node.Add("m_MeshOptimized", 0);
			
			return node;
		}

		public IReadOnlyList<byte> IndexBuffer => m_indexBuffer;
		public IReadOnlyList<SubMesh> SubMeshes => m_subMeshes;
		public IReadOnlyList<Matrix4x4f> BindPoses => m_bindPoses;
		public IReadOnlyList<uint> BoneNameHashes => m_boneNameHashes;
		public IReadOnlyList<Vector3f> Vertices => m_vertices;
		public IReadOnlyList<BoneWeights4> Skin => m_skin;
		public BlendShapeData Shapes { get; }
		public CompressedMesh CompressedMesh { get; }
		public AABB LocalAABB { get; }
		public CollisionMeshData CollisionData { get; }
		public VertexData VertexData { get; private set; }
		public bool IsUse16bitIndices { get; private set; }
		public uint RootBoneNameHash { get; private set; }
		public byte MeshCompression { get; private set; }
		public byte StreamCompression { get; private set; }
		public bool IsReadable { get; private set; }
		public bool KeepVertices { get; private set; }
		public bool KeepIndices { get; private set; }
		public int IndexFormat { get; private set; }
		public int MeshUsageFlags { get; private set; }

		/// <summary>
		/// Less than 3.5
		/// </summary>
		private bool IsReadIndicesUsage => Version.IsLess(3, 5);
		/// <summary>
		/// 2.5.0 and less
		/// </summary>
		private bool IsIndexBufferFirst => Version.IsLessEqual(2, 5);
		/// <summary>
		/// Greater than 4.1.0a
		/// </summary>
		private bool IsReadBlendShapes => Version.IsGreater(4, 1, 0, VersionType.Alpha);
		/// <summary>
		/// 4.3.0 and greater
		/// </summary>
		private bool IsBindPosesFirst => Version.IsGreaterEqual(4, 3);
		/// <summary>
		/// 4.0.0 and greater
		/// </summary>
		private bool IsReadBoolFlags => Version.IsGreaterEqual(4);
		/// <summary>
		/// Less than 5.0.0
		/// </summary>
		private bool IsReadStreamCompression => Version.IsLess(5);
		/// <summary>
		/// 2017.3 and greater
		/// </summary>
		private bool IsReadIndexFormat => Version.IsGreaterEqual(2017, 3);
		/// <summary>
		/// Less than 3.5.0
		/// </summary>
		private bool IsReadVertices => Version.IsLess(3, 5);
		/// <summary>
		/// 3.5.0 and greater
		/// </summary>
		private bool IsReadLocalAABB => Version.IsGreaterEqual(3, 5);
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		private bool IsReadCollision => Version.IsGreaterEqual(5);

		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 8;
			}
		}

		// separate class?
		//private float[] m_UV1;
		//private float[] m_UV2;

		private byte[] m_indexBuffer;
		private SubMesh[] m_subMeshes;
		private Matrix4x4f[] m_bindPoses;
		private uint[] m_boneNameHashes;
		private Vector3f[] m_vertices;
		private BoneWeights4[] m_skin;

	}
}
