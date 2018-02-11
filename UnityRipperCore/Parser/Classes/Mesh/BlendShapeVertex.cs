using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Meshes
{
	public sealed class BlendShapeVertex : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			Position.Read(stream);
			Normal.Read(stream);
			Tangent.Read(stream);
			Index = stream.ReadUInt32();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("vertex", Position.ExportYAML());
			node.Add("normal", Normal.ExportYAML());
			node.Add("tangent", Tangent.ExportYAML());
			node.Add("index", Index);
			return node;
		}

		public Vector3f Position { get; } = new Vector3f();
		public Vector3f Normal { get; } = new Vector3f();
		public Vector3f Tangent { get; } = new Vector3f();
		public uint Index { get; private set; }
	}
}
