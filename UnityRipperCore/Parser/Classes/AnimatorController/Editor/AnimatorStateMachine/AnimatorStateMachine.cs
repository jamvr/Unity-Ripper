using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers.Editor
{
	public sealed class AnimatorStateMachine : NamedObject
	{
		public AnimatorStateMachine(AnimatorController controller) :
			base(CreateAssetsInfo(controller.AssetsFile))
		{
			DefaultState = new PPtr<AnimatorState>(AssetsFile);
		}

		private static AssetInfo CreateAssetsInfo(IAssetsFile assetsFile)
		{
			int classID = (int)ClassIDType.AnimatorStateMachine;
			ClassIDMap classIDMap = new ClassIDMap(classID, classID);
			AssetInfo assetInfo = new AssetInfo(assetsFile, 0, classIDMap);
			return assetInfo;
		}

		public override void Read(EndianStream stream)
		{
			throw new NotSupportedException();
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.AddSerializedVersion(SerializedVersion);
			node.Add("m_Test", ChildStates.ExportYAML());
			node.Add("m_ChildStateMachines", ChildStateMachines.ExportYAML());
			node.Add("m_AnyStateTransitions", AnyStateTransitions.ExportYAML());
			node.Add("m_Test", EntryTransitions.ExportYAML());
			node.Add("m_StateMachineTransitions", StateMachineTransitions.ExportYAMLEnum());
			node.Add("m_StateMachineBehaviours", StateMachineBehaviours.ExportYAML());
			node.Add("m_Test", AnyStatePosition.ExportYAML());
			node.Add("m_EntryPosition", EntryPosition.ExportYAML());
			node.Add("m_Test", ExitPosition.ExportYAML());
			node.Add("m_ParentStateMachinePosition", ParentStateMachinePosition.ExportYAML());
			node.Add("m_DefaultState", DefaultState.ExportYAML());
			return node;
		}

		public IReadOnlyList<ChildAnimatorState> ChildStates => m_childStates;
		public IReadOnlyList<ChildAnimatorStateMachine> ChildStateMachines => m_childStateMachines;
		public IReadOnlyList<PPtr<AnimatorStateTransition>> AnyStateTransitions => m_anyStateTransitions;
		public IReadOnlyList<PPtr<AnimatorTransition>> EntryTransitions => m_entryTransitions;
		public IReadOnlyDictionary<PPtr<AnimatorStateMachine>, PPtr<AnimatorTransition>[]> StateMachineTransitions => m_stateMachineTransitions;
		public IReadOnlyList<PPtr<MonoBehaviour>> StateMachineBehaviours => m_stateMachineBehaviours;
		public Vector3f AnyStatePosition { get; } = new Vector3f();
		public Vector3f EntryPosition { get; } = new Vector3f();
		public Vector3f ExitPosition { get; } = new Vector3f();
		public Vector3f ParentStateMachinePosition { get; } = new Vector3f();
		public PPtr<AnimatorState> DefaultState { get; }

		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 5;
			}
		}

		private readonly Dictionary<PPtr<AnimatorStateMachine>, PPtr<AnimatorTransition>[]> m_stateMachineTransitions = new Dictionary<PPtr<AnimatorStateMachine>, PPtr<AnimatorTransition>[]>();

		private ChildAnimatorState[] m_childStates;
		private ChildAnimatorStateMachine[] m_childStateMachines;
		private PPtr<AnimatorStateTransition>[] m_anyStateTransitions;
		private PPtr<AnimatorTransition>[] m_entryTransitions;
		private PPtr<MonoBehaviour>[] m_stateMachineBehaviours;
	}
}
