using System.Collections.Generic;
using UnityRipper.Classes.Materials;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public static class IDictionaryExtensions
	{
		/*public static YAMLNode ExportYAML(this IDictionary<FastPropertyName, float> _this, Version version)
		{
			YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Block;
			foreach (var kvp in _this)
			{
#warning TODO: write depend on version
				YAMLMappingNode map = new YAMLMappingNode();
				/*map.Add("first", kvp.Key.ExportYAML());
				map.Add("second", kvp.Value.ExportYAML());*//*
				map.Add(kvp.Key.Value, kvp.Value);
				node.Add(map);
			}
			return node;
		}

		public static YAMLNode ExportYAML<T>(this IDictionary<FastPropertyName, T> _this, Version version)
			where T : IYAMLExportable
		{
			YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Block;
			foreach (var kvp in _this)
			{
#warning TODO: write depend on version
				YAMLMappingNode map = new YAMLMappingNode();
				/*map.Add("first", kvp.Key.ExportYAML());
				map.Add("second", kvp.Value.ExportYAML());*//*
				map.Add(kvp.Key.Value, kvp.Value.ExportYAML());
				node.Add(map);
			}
			return node;
		}*/
	}
}
