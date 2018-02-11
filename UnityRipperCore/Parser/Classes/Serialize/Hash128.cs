using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class Hash128 : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			stream.Read(m_data, 0, 16);
		}

		public YAMLNode ExportYAML()
		{
			YAMLScalarNode node = new YAMLScalarNode();

			throw new System.NotImplementedException();
			//return node;
		}

		private readonly byte[] m_data = new byte[16];
	}
}
