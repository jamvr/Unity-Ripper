using System;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Meshes
{
	public sealed class SubMesh : IStreamReadable, IYAMLExportable
	{
		public SubMesh(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}

			m_assetsFile = assetsFile;
			if(IsReadVertex)
			{
				LocalAABB = new AABB();
			}
		}

		public void Read(EndianStream stream)
		{
			FirstByte = stream.ReadUInt32();
			//what is this in case of triangle strips?
			IndexCount = stream.ReadUInt32();
			//isTriStrip
			Topology = stream.ReadInt32();

			if(IsReadTriangleCount)
			{
				TriangleCount = stream.ReadUInt32();
			}
			if(IsReadVertex)
			{
				if(IsReadBaseVertex)
				{
					BaseVertex = stream.ReadUInt32();
				}
				FirstVertex = stream.ReadUInt32();
				VertexCount = stream.ReadUInt32();
				LocalAABB.Read(stream);
			}
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.AddSerializedVersion(SerializedVersion);
			node.Add("firstByte", FirstByte);
			node.Add("indexCount", IndexCount);
			node.Add("topology", Topology);
			node.Add("firstVertex", FirstVertex);
			node.Add("vertexCount", VertexCount);
			node.Add("localAABB", LocalAABB.ExportYAML());
			return node;
		}

		public uint FirstByte { get; private set; }
		public uint IndexCount { get; private set; }
		public int Topology { get; private set; }
		public uint TriangleCount { get; private set; }
		public uint BaseVertex { get; private set; }
		public uint FirstVertex { get; private set; }
		public uint VertexCount { get; private set; }
		public AABB LocalAABB { get; private set; }

		private Version Version => m_assetsFile.Version;

		/// <summary>
		/// Less than 4.0.0
		/// </summary>
		private bool IsReadTriangleCount => Version.IsLess(4);
		/// <summary>
		/// 3.0.0 and greater
		/// </summary>
		private bool IsReadVertex => Version.IsGreaterEqual(3);
		/// <summary>
		/// 2017.3 and greater
		/// </summary>
		private bool IsReadBaseVertex => Version.IsGreaterEqual(2017, 3);

		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 2;
			}
		}

		private readonly IAssetsFile m_assetsFile;
	}
}
