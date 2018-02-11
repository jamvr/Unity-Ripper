using System;
using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers.Editor
{
	public abstract class AnimatorTransitionBase : NamedObject
	{
		public AnimatorTransitionBase(AssetInfo assetsInfo) :
			base(assetsInfo)
		{
			DstStateMachine = new PPtr<AnimatorStateMachine>(AssetsFile);
			DstState = new PPtr<AnimatorState>(AssetsFile);
		}

		public sealed override void Read(EndianStream stream)
		{
			throw new NotSupportedException();
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.Add("m_Conditions", Conditions.ExportYAML());
			node.Add("m_DstStateMachine", DstStateMachine.ExportYAML());
			node.Add("m_DstState", DstState.ExportYAML());
			node.Add("m_Solo", Solo);
			node.Add("m_Mute", Mute);
			node.Add("m_IsExit", IsExit);
			return node;
		}

		public override string ExportExtension => throw new NotSupportedException();

		public IReadOnlyList<AnimatorCondition> Conditions => m_conditions;
		public PPtr<AnimatorStateMachine> DstStateMachine { get; }
		public PPtr<AnimatorState> DstState { get; }
		public bool Solo { get; private set; }
		public bool Mute { get; private set; }
		public bool IsExit { get; private set; }

		private AnimatorCondition[] m_conditions;
	}
}
