using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class PackedQuatVector : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			NumItems = stream.ReadUInt32();
			int size = stream.ReadInt32();
			m_data = new byte[size];
			stream.Read(m_data, 0, size);
			stream.AlignStream(AlignType.Align4);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_NumItems", NumItems);
			node.Add("m_Data", Data.ExportYAML());
			return node;
		}

		public uint NumItems { get; private set; }
		public IReadOnlyList<byte> Data => m_data;

		private byte[] m_data;
	}
}
