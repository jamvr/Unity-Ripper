using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AvatarMasks
{
	public sealed class TransformMaskElement : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			Path = stream.ReadStringAligned();
			Weight = stream.ReadSingle();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_Path", Path);
			node.Add("m_Weight", Weight);
			return node;
		}

		public string Path { get; private set; }
		public float Weight { get; private set; }
	}
}
