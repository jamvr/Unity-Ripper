using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public static class Vector4fExtensions
	{
		/// <summary>
		/// Export to YAML format as Vector3f
		/// </summary>
		/// <returns>YAML node with 3 components set</returns>
		public static YAMLNode ExportYAML3(this Vector4f _this)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Style = MappingStyle.Flow;
			node.Add("x", _this.X);
			node.Add("y", _this.Y);
			node.Add("z", _this.Z);
			return node;
		}
	}
}
