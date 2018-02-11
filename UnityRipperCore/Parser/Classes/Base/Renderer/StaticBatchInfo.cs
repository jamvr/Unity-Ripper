using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.MeshRenderers
{
	public sealed class StaticBatchInfo : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			FirstSubMesh = stream.ReadUInt16();
			SubMeshCount = stream.ReadUInt16();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();

#warning TODO: names
			node.Add("firstSubMesh", FirstSubMesh);
			node.Add("subMeshCount", SubMeshCount);
			return node;
		}

		public ushort FirstSubMesh { get; private set; }
		public ushort SubMeshCount { get; private set; }
	}
}
