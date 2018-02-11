using System;
using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class SkeletonMask : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			m_data = stream.ReadArray<SkeletonMaskElement>();
		}

		public YAMLNode ExportYAML()
		{
			throw new NotSupportedException();
		}

		public IReadOnlyList<SkeletonMaskElement> Data => m_data;

		private SkeletonMaskElement[] m_data;
	}
}
