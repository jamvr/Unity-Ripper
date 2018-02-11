using System;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class Vector3f : IStreamReadable, IYAMLExportable
	{
		public Vector3f()
		{
		}

		public Vector3f(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public void Read(EndianStream stream)
		{
			X = stream.ReadSingle();
			Y = stream.ReadSingle();
			Z = stream.ReadSingle();
		}

		public void CopyFrom(Vector3f copy)
		{
			X = copy.X;
			Y = copy.Y;
			Z = copy.Z;
		}

		public float GetMember(int index)
		{
			if (index == 0)
			{
				return X;
			}
			if (index == 1)
			{
				return Y;
			}
			if (index == 2)
			{
				return Z;
			}
			throw new ArgumentException($"Invalid index {index}", nameof(index));
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Style = MappingStyle.Flow;
			node.Add("x", X);
			node.Add("y", Y);
			node.Add("z", Z);
			return node;
		}

		public override string ToString()
		{
			return $"[{X.ToString("0.00")}, {Y.ToString("0.00")}, {Z.ToString("0.00")}]";
		}

		public float X { get; private set; }
		public float Y { get; private set; }
		public float Z { get; private set; }

		public static readonly Vector3f Empty = new Vector3f();
	}
}
