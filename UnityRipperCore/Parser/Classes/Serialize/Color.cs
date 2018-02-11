using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class Color : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			R = stream.ReadSingle();
			G = stream.ReadSingle();
			B = stream.ReadSingle();
			A = stream.ReadSingle();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Style = MappingStyle.Flow;
			node.Add("r", R);
			node.Add("g", G);
			node.Add("b", B);
			node.Add("a", A);
			return node;
			throw new System.NotImplementedException();
		}

		public float R { get; private set; }
		public float G { get; private set; }
		public float B { get; private set; }
		public float A { get; private set; }
	}
}
