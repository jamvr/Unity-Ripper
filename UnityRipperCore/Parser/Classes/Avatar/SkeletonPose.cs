using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Avatars
{
	public sealed class SkeletonPose : IStreamReadable, IYAMLExportable
	{
		public SkeletonPose(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			m_x = stream.ReadArray(() => new XForm(m_assetsFile));
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_X", X.ExportYAML());
			return node;
		}

		public IReadOnlyList<XForm> X => m_x;

		private readonly IAssetsFile m_assetsFile;

		private XForm[] m_x;
	}
}
