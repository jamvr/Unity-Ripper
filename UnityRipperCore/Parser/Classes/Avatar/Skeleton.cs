using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Avatars
{
	public sealed class Skeleton : IStreamReadable, IYAMLExportable
	{
		public Skeleton(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			m_node = stream.ReadArray<Node>();
			m_ID = stream.ReadUInt32Array();
			m_axesArray = stream.ReadArray(() => new Axes(m_assetsFile));
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_Node", Node.ExportYAML());
			node.Add("m_ID", ID.ExportYAML(true));
			node.Add("m_AxesArray", AxesArray.ExportYAML());
			return node;
		}

		public IReadOnlyList<Node> Node => m_node;
		public IReadOnlyList<uint> ID => m_ID;
		public IReadOnlyList<Axes> AxesArray => m_axesArray;

		private readonly IAssetsFile m_assetsFile;

		private Node[] m_node;
		private uint[] m_ID;
		private Axes[] m_axesArray;
	}
}
