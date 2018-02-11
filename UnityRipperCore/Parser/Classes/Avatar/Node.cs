using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Avatars
{
	public sealed class Node : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			ParentId = stream.ReadInt32();
			AxesId = stream.ReadInt32();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_ParentId", ParentId);
			node.Add("m_AxesId", AxesId);
			return node;
		}

		public int ParentId { get; private set; }
		public int AxesId { get; private set; }
	}
}
