using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers.Editor
{
	public sealed class AnimatorControllerLayers : IYAMLExportable
	{
		public AnimatorControllerLayers(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}

			StateMachine = new PPtr<AnimatorStateMachine>(assetsFile);
			Mask = new PPtr<AvatarMask>(assetsFile);
			Controller = new PPtr<AnimatorController>(assetsFile);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_Name", Name);
			node.Add("m_StateMachine", StateMachine.ExportYAML());
			node.Add("m_Mask", Mask.ExportYAML());
			node.Add("m_Motions", Motions.ExportYAML());
			node.Add("m_Behaviours", Behaviours.ExportYAML());
			node.Add("m_BlendingMode", BlendingMode);
			node.Add("m_SyncedLayerIndex", SyncedLayerIndex);
			node.Add("m_DefaultWeight", DefaultWeight);
			node.Add("m_IKPass", IKPass);
			node.Add("m_SyncedLayerAffectsTiming", SyncedLayerAffectsTiming);
			node.Add("m_Controller", Controller.ExportYAML());
			return node;
		}

		public string Name { get; private set; }
		public PPtr<AnimatorStateMachine> StateMachine { get; }
		public PPtr<AvatarMask> Mask { get; }
		public IReadOnlyList<StateMotionPair> Motions => m_motions;
		public IReadOnlyList<StateBehavioursPair> Behaviours => m_behaviours;
		public int BlendingMode { get; private set; }
		public int SyncedLayerIndex { get; private set; }
		public float DefaultWeight { get; private set; }
		public bool IKPass { get; private set; }
		public bool SyncedLayerAffectsTiming { get; private set; }
		public PPtr<AnimatorController> Controller { get; }

		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 5;
			}
		}

		private StateMotionPair[] m_motions;
		private StateBehavioursPair[] m_behaviours;
	}
}
