using System;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class InnerPPtr<T> : IPPtr<T>
		where T: Object
	{
		public InnerPPtr(Prefab prefab)
		{
			if(prefab == null)
			{
				throw new ArgumentNullException(nameof(prefab));
			}
			m_prefab = prefab;
		}

		public void Read(EndianStream stream)
		{
			throw new NotSupportedException();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Style = MappingStyle.Flow;
			node.Add("fileID", m_prefab.ExportID);
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

		public bool IsNull => true;

		private readonly Prefab m_prefab;
	}
}
