using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class BlendTreeConstant : IStreamReadable, IYAMLExportable
	{
		public BlendTreeConstant(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			m_nodeArray = stream.ReadArray(() => new OffsetPtr<BlendTreeNodeConstant>(new BlendTreeNodeConstant(m_assetsFile)));
		}

		public YAMLNode ExportYAML()
		{
			throw new NotSupportedException();
		}

		public IReadOnlyList<OffsetPtr<BlendTreeNodeConstant>> NodeArray => m_nodeArray;

		private readonly IAssetsFile m_assetsFile;

		private OffsetPtr<BlendTreeNodeConstant>[] m_nodeArray;
	}
}
