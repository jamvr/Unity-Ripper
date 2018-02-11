using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimationClips
{
	public sealed class CompressedAnimationCurve : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			Path = stream.ReadStringAligned();
			Times.Read(stream);
			Values.Read(stream);
			Slopes.Read(stream);
			PreInfinity = stream.ReadInt32();
			PostInfinity = stream.ReadInt32();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_Path", Path);
			node.Add("m_Times", Times.ExportYAML());
			node.Add("m_Values", Values.ExportYAML());
			node.Add("m_Slopes", Slopes.ExportYAML());
			node.Add("m_PreInfinity", PreInfinity);
			node.Add("m_PostInfinity", PostInfinity);
			return node;
		}

		public string Path { get; private set; }
		public PackedIntVector Times { get; } = new PackedIntVector();
		public PackedQuatVector Values { get; } = new PackedQuatVector();
		public PackedFloatVector Slopes { get; } = new PackedFloatVector();
		public int PreInfinity { get; private set; }
		public int PostInfinity { get; private set; }
	}
}
