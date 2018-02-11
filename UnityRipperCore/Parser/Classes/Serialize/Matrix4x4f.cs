using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class Matrix4x4f : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			m_e[0, 0] = stream.ReadSingle();
			m_e[0, 1] = stream.ReadSingle();
			m_e[0, 2] = stream.ReadSingle();
			m_e[0, 3] = stream.ReadSingle();
			m_e[1, 0] = stream.ReadSingle();
			m_e[1, 1] = stream.ReadSingle();
			m_e[1, 2] = stream.ReadSingle();
			m_e[1, 3] = stream.ReadSingle();
			m_e[2, 0] = stream.ReadSingle();
			m_e[2, 1] = stream.ReadSingle();
			m_e[2, 2] = stream.ReadSingle();
			m_e[2, 3] = stream.ReadSingle();
			m_e[3, 0] = stream.ReadSingle();
			m_e[3, 1] = stream.ReadSingle();
			m_e[3, 2] = stream.ReadSingle();
			m_e[3, 3] = stream.ReadSingle();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("e00", E00);
			node.Add("e01", E01);
			node.Add("e02", E02);
			node.Add("e03", E03);
			node.Add("e10", E10);
			node.Add("e11", E11);
			node.Add("e12", E12);
			node.Add("e13", E13);
			node.Add("e20", E20);
			node.Add("e21", E21);
			node.Add("e22", E22);
			node.Add("e23", E23);
			node.Add("e30", E30);
			node.Add("e31", E31);
			node.Add("e32", E32);
			node.Add("e33", E33);
			return node;
		}

		public float E00 => m_e[0, 0];
		public float E01 => m_e[0, 1];
		public float E02 => m_e[0, 2];
		public float E03 => m_e[0, 3];
		public float E10 => m_e[1, 0];
		public float E11 => m_e[1, 1];
		public float E12 => m_e[1, 2];
		public float E13 => m_e[1, 3];
		public float E20 => m_e[2, 0];
		public float E21 => m_e[2, 1];
		public float E22 => m_e[2, 2];
		public float E23 => m_e[2, 3];
		public float E30 => m_e[3, 0];
		public float E31 => m_e[3, 1];
		public float E32 => m_e[3, 2];
		public float E33 => m_e[3, 3];

		private float[,] m_e = new float[4, 4];
	}
}
