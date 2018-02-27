using System;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public abstract class InnerPPtr<T> : IPPtr<T>
		where T: Object
	{
		public void Read(EndianStream stream)
		{
			throw new NotSupportedException();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Style = MappingStyle.Flow;
			node.Add("fileID", PathIDString);
			return node;
		}

		public T FindObject()
		{
			throw new NotSupportedException();
		}

		public T GetObject()
		{
			throw new NotSupportedException();
		}

		public bool IsNull => false;

		protected abstract string PathIDString { get; }
	}
}
