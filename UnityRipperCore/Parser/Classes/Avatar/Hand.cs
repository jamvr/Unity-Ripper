using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Avatars
{
	public sealed class Hand : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			m_handBoneIndex = stream.ReadInt32Array();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_HandBoneIndex", m_handBoneIndex.ExportYAML(true));
			return node;
		}

		public IReadOnlyList<int> HandBoneIndex => m_handBoneIndex;

		private int[] m_handBoneIndex;
	}
}
