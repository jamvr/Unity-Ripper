using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Meshes
{
	public sealed class CompressedMesh : IStreamReadable, IYAMLExportable
	{
		public CompressedMesh(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;

			if(IsReadMeshData)
			{
				Vertices = new PackedFloatVector();
				UV = new PackedFloatVector();
				if(IsReadBindPoses)
				{
					BindPoses = new PackedFloatVector();
				}
				Normals = new PackedFloatVector();
				Tangents = new PackedFloatVector();
				Weight = new PackedIntVector();
				NormalSigns = new PackedIntVector();
				TangentSigns = new PackedIntVector();
				if(IsReadFloatColors)
				{
					FloatColors = new PackedFloatVector();
				}
				BoneIndices = new PackedIntVector();
				Triangles = new PackedIntVector();
			}

			if(IsReadPlainColors)
			{
				LocalAABB = new AABB();
			}
			else
			{
				if (IsReadCompressedColors)
				{
					Colors = new PackedIntVector();
				}
			}
		}

		public void Read(EndianStream stream)
		{
			if(IsReadMeshData)
			{
				Vertices.Read(stream);
				UV.Read(stream);
				if(IsReadBindPoses)
				{
					BindPoses.Read(stream);
				}
				Normals.Read(stream);
				Tangents.Read(stream);
				Weight.Read(stream);
				NormalSigns.Read(stream);
				TangentSigns.Read(stream);

				if(IsReadFloatColors)
				{
					FloatColors.Read(stream);
				}

				BoneIndices.Read(stream);
				Triangles.Read(stream);
			}

			if(IsReadPlainColors)
			{
				LocalAABB.Read(stream);
				m_plainColors = stream.ReadArray<Color>();
#warning TODO: todo what?
				m_collisionTriangles = stream.ReadByteArray();
				CollisionVertexCount = stream.ReadInt32();
			}
			else
			{
				if (IsReadCompressedColors)
				{
					Colors.Read(stream);
				}
				else
				{
					UVInfo = stream.ReadUInt32();
				}
			}
		}

		public YAMLNode ExportYAML()
		{
#warning TODO: support different versions
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_Vertices", Vertices.ExportYAML());
			node.Add("m_UV", UV.ExportYAML());
			node.Add("m_Normals", Normals.ExportYAML());
			node.Add("m_Tangents", Tangents.ExportYAML());
			node.Add("m_Weights", Weight.ExportYAML());
			node.Add("m_NormalSigns", NormalSigns.ExportYAML());
			node.Add("m_TangentSigns", TangentSigns.ExportYAML());
			if(IsReadFloatColors)
			{
				node.Add("m_FloatColors", FloatColors.ExportYAML());
			}
			else
			{
				node.Add("m_FloatColors", PackedFloatVector.Empty.ExportYAML());
			}
			node.Add("m_BoneIndices", BoneIndices.ExportYAML());
			node.Add("m_Triangles", Triangles.ExportYAML());
			node.Add("m_UVInfo", UVInfo);
			return node;
		}

		public PackedFloatVector Vertices { get; }
		public PackedFloatVector UV { get; }
		public PackedFloatVector BindPoses { get; }
		public PackedFloatVector Normals { get; }
		public PackedFloatVector Tangents { get; }
		public PackedIntVector Weight { get; }
		public PackedIntVector NormalSigns { get; }
		public PackedIntVector TangentSigns { get; }
		public PackedFloatVector FloatColors { get; }
		public PackedIntVector BoneIndices { get; }
		public PackedIntVector Triangles { get; }
		public IReadOnlyList<Color> PlainColors => m_plainColors;
		public IReadOnlyList<byte> CollisionTriangles => m_collisionTriangles;
		public int CollisionVertexCount { get; private set; }
#warning TODO: rename
		public AABB LocalAABB { get; }
		public PackedIntVector Colors { get; }
		public uint UVInfo { get; private set; }

		/// <summary>
		/// 2.6.0 and greater
		/// </summary>
		public bool IsReadMeshData => Version.IsGreaterEqual(2, 6);
		/// <summary>
		/// Less than 5.0.0
		/// </summary>
		public bool IsReadBindPoses => Version.IsLess(5);
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		public bool IsReadFloatColors => Version.IsGreaterEqual(5);
		/// <summary>
		/// Less than 5.0.0
		/// </summary>
		public bool IsReadCompressedColors => Version.IsLess(5);

		private Version Version => m_assetsFile.Version;

		/// <summary>
		/// Less than 3.5.0
		/// </summary>
		private bool IsReadPlainColors => Version.IsLess(3, 5);

		private Color[] m_plainColors;
		private byte[] m_collisionTriangles;

		private readonly IAssetsFile m_assetsFile;
	}
}
