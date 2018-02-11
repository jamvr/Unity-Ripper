using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class Float : IStreamReadable, IYAMLExportable
	{
		public Float()
		{
		}

		public Float(float value)
		{
			Value = value;
		}

		public void Read(EndianStream stream)
		{
			Value = stream.ReadSingle();
		}

		public YAMLNode ExportYAML()
		{
			YAMLScalarNode node = new YAMLScalarNode();
			node.SetValue(Value);
			return node;
		}

		public float Value { get; private set; }
	}
}
