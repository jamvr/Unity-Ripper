using System;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class Vector2f : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			X = stream.ReadSingle();
			Y = stream.ReadSingle();
		}

		public void CopyFrom(Vector2f copy)
		{
			X = copy.X;
			Y = copy.Y;
		}

		public float GetMember(int index)
		{
			if (index == 0)
			{
				return X;
			}
			if(index == 1)
			{
				return Y;
			}
			throw new ArgumentException($"Invalid index {index}", nameof(index));
		}
		
		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Style = MappingStyle.Flow;
			node.Add("x", X);
			node.Add("y", Y);
			return node;
		}

		public override string ToString()
		{
			return $"[{X.ToString("0.00")}, {Y.ToString("0.00")}]";
		}

		public float X { get; private set; }
		public float Y { get; private set; }
	}
}
