using System.Collections.Generic;

namespace UnityRipper.Exporter.YAML
{
	public static class IDictionaryExtensions
	{
		public static YAMLNode ExportYAML(this IReadOnlyDictionary<uint, string> _this)
		{
#warning TODO: check
			YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Block;
			foreach (var kvp in _this)
			{
				YAMLMappingNode map = new YAMLMappingNode();
				map.Add(kvp.Key.ToString(), kvp.Value);
				node.Add(map);
			}
			return node;
		}

		public static YAMLNode ExportYAML(this IReadOnlyDictionary<string, string> _this)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			foreach(var kvp in _this)
			{
				node.Add(kvp.Key, kvp.Value);
			}
			return node;
		}

		public static YAMLNode ExportYAML(this IReadOnlyDictionary<string, float> _this)
		{
			YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Block;
			foreach (var kvp in _this)
			{
				YAMLMappingNode map = new YAMLMappingNode();
				map.Add(kvp.Key, kvp.Value);
				node.Add(map);
			}
			return node;
		}

		public static YAMLNode ExportYAML<T>(this IReadOnlyDictionary<string, T> _this)
			where T: IYAMLExportable
		{
			YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Block;
			foreach (var kvp in _this)
			{
				YAMLMappingNode map = new YAMLMappingNode();
				map.Add(kvp.Key, kvp.Value.ExportYAML());
				node.Add(map);
			}
			return node;
		}

		public static YAMLNode ExportYAML<T>(this IReadOnlyDictionary<T, float> _this)
			where T: IYAMLExportable
		{
			YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Block;
			foreach (var kvp in _this)
			{
				YAMLMappingNode map = new YAMLMappingNode();
				YAMLNode key = kvp.Key.ExportYAML();
				if(key.NodeType == YAMLNodeType.Scalar)
				{
					map.Add(key, kvp.Value);
				}
				else
				{
					map.Add("first", key);
					map.Add("second", kvp.Value);
				}
				node.Add(map);
			}
			return node;
		}

		public static YAMLNode ExportYAML<T1, T2>(this IReadOnlyDictionary<T1, T2> _this)
			where T1 : IYAMLExportable
			where T2 : IYAMLExportable
		{
			YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Block;
			foreach (var kvp in _this)
			{
				YAMLMappingNode map = new YAMLMappingNode();
				YAMLNode key = kvp.Key.ExportYAML();
				if (key.NodeType == YAMLNodeType.Scalar)
				{
					map.Add(key, kvp.Value.ExportYAML());
				}
				else
				{
					map.Add("first", key);
					map.Add("second", kvp.Value.ExportYAML());
				}
				node.Add(map);
			}
			return node;
		}

		public static YAMLNode ExportYAMLEnum<T1, T2>(this IReadOnlyDictionary<T1, T2> _this)
			where T1 : IYAMLExportable
			where T2 : IEnumerable<IYAMLExportable>
		{
			YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Block;
			foreach (var kvp in _this)
			{
				YAMLMappingNode map = new YAMLMappingNode();
				YAMLNode key = kvp.Key.ExportYAML();
				if (key.NodeType == YAMLNodeType.Scalar)
				{
					map.Add(key, kvp.Value.ExportYAML());
				}
				else
				{
					map.Add("first", key);
					map.Add("second", kvp.Value.ExportYAML());
				}
				node.Add(map);
			}
			return node;
		}
	}
}
