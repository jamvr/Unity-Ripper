using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Meshes
{
	public sealed class BlendShapeChannel : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			Name = stream.ReadStringAligned();
			NameHash = stream.ReadUInt32();
			FrameIndex = stream.ReadInt32();
			FrameCount = stream.ReadInt32();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("name", Name);
			node.Add("nameHash", NameHash);
			node.Add("frameIndex", FrameIndex);
			node.Add("frameCount", FrameCount);
			return node;
		}

		public string Name { get; private set; }
		public uint NameHash { get; private set; }
		public int FrameIndex { get; private set; }
		public int FrameCount { get; private set; }
	}
}
