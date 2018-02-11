using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers.Editor
{
	public sealed class BlendTree : Motion
	{
		public BlendTree(AssetInfo assetsInfo) :
			base(assetsInfo)
		{
		}

		private static AssetInfo CreateAssetsInfo(IAssetsFile assetsFile)
		{
			int classID = (int)ClassIDType.BlendTree;
			ClassIDMap classIDMap = new ClassIDMap(classID, classID);
			AssetInfo assetInfo = new AssetInfo(assetsFile, 0, classIDMap);
			return assetInfo;
		}

		public sealed override void Read(EndianStream stream)
		{
			throw new NotSupportedException();
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.Add("m_Childs", Childs.ExportYAML());
			node.Add("m_BlendParameter", BlendParameter);
			node.Add("m_BlendParameterY", BlendParameterY);
			node.Add("m_MinThreshold", MinThreshold);
			node.Add("m_MaxThreshold", MaxThreshold);
			node.Add("m_UseAutomaticThresholds", UseAutomaticThresholds);
			node.Add("m_NormalizedBlendValues", NormalizedBlendValues);
			node.Add("m_BlendType", BlendType);
			return node;
		}

		public override string ExportExtension => throw new NotSupportedException();

		public IReadOnlyList<ChildMotion> Childs => m_childs;
		public string BlendParameter { get; private set; }
		public string BlendParameterY { get; private set; }
		public float MinThreshold { get; private set; }
		public float MaxThreshold { get; private set; }
		public bool UseAutomaticThresholds { get; private set; }
		public bool NormalizedBlendValues { get; private set; }
		public int BlendType { get; private set; }

		private ChildMotion[] m_childs;
	}
}
