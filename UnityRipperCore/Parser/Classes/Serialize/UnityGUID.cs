using System;
using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class UnityGUID : IStreamReadable, IYAMLExportable
	{
		static UnityGUID()
		{
			MissingReference = new UnityGUID();
			MissingReference.m_data[0] = 0xD0000000;
			MissingReference.m_data[1] = 0x5DEADF00;
			MissingReference.m_data[2] = 0xEADBEEF1;
			MissingReference.m_data[3] = 0x0000000D;
		}

		public UnityGUID()
		{
		}

		public UnityGUID(Guid guid):
			this(guid.ToByteArray())
		{
		}

		public UnityGUID(byte[] guidData)
		{
			m_data[0] = BitConverter.ToUInt32(guidData, 0);
			m_data[1] = BitConverter.ToUInt32(guidData, 4);
			m_data[2] = BitConverter.ToUInt32(guidData, 8);
			m_data[3] = BitConverter.ToUInt32(guidData, 12);
		}

		public void Read(EndianStream stream)
		{
			m_data[0] = stream.ReadUInt32();
			m_data[1] = stream.ReadUInt32();
			m_data[2] = stream.ReadUInt32();
			m_data[3] = stream.ReadUInt32();
		}

		public YAMLNode ExportYAML()
		{
			YAMLScalarNode node = new YAMLScalarNode();
			node.SetValue(ToString());
			return node;
		}

		public override string ToString()
		{
			return $"{m_data[3].ToString("x8")}{m_data[2].ToString("x8")}" +
				$"{m_data[1].ToString("x8")}{m_data[0].ToString("x8")}";
		}

		public static readonly UnityGUID MissingReference;

		public IReadOnlyList<uint> Data => m_data;

		private readonly uint[] m_data = new uint[4];
	}
}
