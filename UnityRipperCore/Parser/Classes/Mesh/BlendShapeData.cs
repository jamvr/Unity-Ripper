using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Meshes
{
	public sealed class BlendShapeData : IStreamReadable, IYAMLExportable
	{
		public BlendShapeData(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			if (IsReadChannels)
			{
				m_vertices = stream.ReadArray<BlendShapeVertex>();
				m_shapes = stream.ReadArray(() => new BlendShape(m_assetsFile));
				m_channels = stream.ReadArray<BlendShapeChannel>();
				stream.AlignStream(AlignType.Align4);

				m_fullWeights = stream.ReadSingleArray();
			}
			else
			{
				m_shapes = stream.ReadArray(() => new BlendShape(m_assetsFile));
				m_vertices = stream.ReadArray<BlendShapeVertex>();
			}
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("vertices", m_vertices.ExportYAML());
			node.Add("shapes", m_shapes.ExportYAML());
			node.Add("channels", m_channels.ExportYAML());
			node.Add("fullWeights", m_fullWeights.ExportYAML());
			return node;
		}

		public IReadOnlyList<BlendShapeVertex> Vertices => m_vertices;
		public IReadOnlyList<BlendShape> Shapes => m_shapes;
		public IReadOnlyList<BlendShapeChannel> Channels => m_channels;
		public IReadOnlyList<float> FullWeights => m_fullWeights;

		private Version Version => m_assetsFile.Version;

		/// <summary>
		/// 4.3.0 and greater
		/// </summary>
		private bool IsReadChannels => Version.IsGreaterEqual(4, 3);

		private BlendShapeVertex[] m_vertices;
		private BlendShape[] m_shapes;
		private BlendShapeChannel[] m_channels;
		private float[] m_fullWeights;

		private readonly IAssetsFile m_assetsFile;
	}
}
