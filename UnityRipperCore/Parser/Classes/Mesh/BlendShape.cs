using System;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Meshes
{
	public sealed class BlendShape : IStreamReadable, IYAMLExportable
	{
		public BlendShape(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}

			m_assetsFile = assetsFile;

			if(IsReadAABB)
			{
				AabbMinDelta = new Vector3f();
			}
		}

		public void Read(EndianStream stream)
		{
			FirstVertex = stream.ReadUInt32();
			VertexCount = stream.ReadUInt32();
			if (IsReadAABB)
			{
				AabbMinDelta.Read(stream);
				AabbMaxDelta.Read(stream);
			}
			HasNormals = stream.ReadBoolean();
			HasTangents = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("firstVertex", FirstVertex);
			node.Add("vertexCount", VertexCount);
			node.Add("hasNormals", HasNormals);
			node.Add("hasTangents", HasTangents);
			return node;
		}

		public uint FirstVertex { get; private set; }
		public uint VertexCount { get; private set; }
		public Vector3f AabbMinDelta { get; }
		public Vector3f AabbMaxDelta { get; }
		public bool HasNormals { get; private set; }
		public bool HasTangents { get; private set; }

		private Version Version => m_assetsFile.Version;

		/// <summary>
		/// Less than 4.3
		/// </summary>
		private bool IsReadAABB => Version.IsLess(4, 3);

		private readonly IAssetsFile m_assetsFile;
	}
}
