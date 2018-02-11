using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public class Vector4f : IStreamReadable, IYAMLExportable
	{
		public Vector4f()
		{
		}

		public Vector4f(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public void Read(EndianStream stream)
		{
			X = stream.ReadSingle();
			Y = stream.ReadSingle();
			Z = stream.ReadSingle();
			W = stream.ReadSingle();
		}
		
		public void CopyFrom(Vector4f copy)
		{
			X = copy.X;
			Y = copy.Y;
			Z = copy.Z;
			W = copy.W;
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Style = MappingStyle.Flow;
			node.Add("x", X);
			node.Add("y", Y);
			node.Add("z", Z);
			node.Add("w", W);
			return node;
		}

		public override string ToString()
		{
			return $"[{X.ToString("0.00")}, {Y.ToString("0.00")}, {Z.ToString("0.00")}, {W.ToString("0.00")}]";
		}

		public float X { get; private set; }
		public float Y { get; private set; }
		public float Z { get; private set; }
		public float W { get; private set; }
	}
}
