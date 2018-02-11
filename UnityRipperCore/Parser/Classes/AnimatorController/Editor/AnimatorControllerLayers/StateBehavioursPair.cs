using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers.Editor
{
	public sealed class StateBehavioursPair : IYAMLExportable
	{
		public StateBehavioursPair(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}

			State = new PPtr<AnimatorState>(assetsFile);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_State", State.ExportYAML());
			node.Add("m_StateMachineBehaviours", StateMachineBehaviours.ExportYAML());
			return node;
		}

		public PPtr<AnimatorState> State { get; }
		public IReadOnlyList<PPtr<MonoBehaviour>> StateMachineBehaviours => m_stateMachineBehaviours;

		private PPtr<MonoBehaviour>[] m_stateMachineBehaviours;
	}
}
