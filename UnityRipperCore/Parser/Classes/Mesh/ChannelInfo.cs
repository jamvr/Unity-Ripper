using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Meshes
{
	public sealed class ChannelInfo : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			Stream = stream.ReadByte();
			Offset = stream.ReadByte();
			Format = stream.ReadByte();
			Dimension = stream.ReadByte();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("stream", Stream);
			node.Add("offset", Offset);
			node.Add("format", Format);
			node.Add("dimension", Dimension);
			return node;
		}

		public byte Stream { get; private set; }
		public byte Offset { get; private set; }
		public byte Format { get; private set; }
		public byte Dimension { get; private set; }
	}
}
