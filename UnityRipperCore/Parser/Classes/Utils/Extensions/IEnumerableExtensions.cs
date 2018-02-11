using System.Collections.Generic;
using UnityRipper.Classes.GameObjects;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public static class IEnumerableExtensions
	{
		public static YAMLNode ExportYAML3(this IEnumerable<Vector4f> _this)
		{
			YAMLSequenceNode node = new YAMLSequenceNode();
			foreach (Vector4f vector in _this)
			{
				node.Add(vector.ExportYAML3());
			}
			return node;
		}
		
		public static YAMLNode ExportYAML(this IEnumerable<Float> _this)
		{
			YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Flow;
			foreach (Float value in _this)
			{
				node.Add(value.ExportYAML());
			}
			return node;
		}
		
		public static YAMLNode ExportYAML(this IEnumerable<ComponentPair> _this)
		{
			YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Block;
			foreach (var t in _this)
			{
#warning TODO: missing component
				if (t.Component.IsValid)
				{
					node.Add(t.ExportYAML());
				}
			}
			return node;
		}
	}
}
