using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class AABB : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			Center.Read(stream);
			Extent.Read(stream);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_Center", Center.ExportYAML());
			node.Add("m_Extent", Extent.ExportYAML());
			return node;
		}

		public Vector3f Center { get; } = new Vector3f();
		public Vector3f Extent { get; } = new Vector3f();
	}
}
