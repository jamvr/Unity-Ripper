using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public class OffsetPtr<T> : IStreamReadable, IYAMLExportable
		where T: IStreamReadable, IYAMLExportable
	{
		public OffsetPtr(T instance)
		{
			Instance = instance;
		}

		public void Read(EndianStream stream)
		{
			Instance.Read(stream);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("data", Instance.ExportYAML());
			return node;
		}

		public T Instance { get; }
	}
}
