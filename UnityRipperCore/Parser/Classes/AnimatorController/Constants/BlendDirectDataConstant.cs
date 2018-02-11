using System;
using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class BlendDirectDataConstant : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			m_childBlendEventIDArray = stream.ReadUInt32Array();
			NormalizedBlendValues = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);
		}

		public YAMLNode ExportYAML()
		{
			throw new NotSupportedException();
		}

		public IReadOnlyList<uint> ChildBlendEventIDArray => m_childBlendEventIDArray;
		public bool NormalizedBlendValues { get; private set; }

		private uint[] m_childBlendEventIDArray;
	}
}
