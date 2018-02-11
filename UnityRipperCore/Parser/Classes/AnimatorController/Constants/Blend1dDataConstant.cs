using System;
using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class Blend1dDataConstant : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			m_childThresholdArray = stream.ReadSingleArray();
		}

		public YAMLNode ExportYAML()
		{
			throw new NotSupportedException();
		}

		public IReadOnlyList<float> ChildThresholdArray => m_childThresholdArray;

		private float[] m_childThresholdArray;
	}
}
