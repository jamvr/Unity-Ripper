using System;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers.Editor
{
	public sealed class StateMotionPair : IYAMLExportable
	{
		public StateMotionPair(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}

			State = new PPtr<AnimatorState>(assetsFile);
			Motion = new PPtr<Motion>(assetsFile);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_State", State.ExportYAML());
			node.Add("m_Motion", Motion.ExportYAML());
			return node;
		}

		public PPtr<AnimatorState> State { get; }
		public PPtr<Motion> Motion { get; }
	}
}
