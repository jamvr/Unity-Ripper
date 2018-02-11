using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers.Editor
{
	public sealed class AnimatorStateTransition : AnimatorTransitionBase
	{
		public AnimatorStateTransition(IAssetsFile assetsFile) :
			base(CreateAssetsInfo(assetsFile))
		{
		}

		private static AssetInfo CreateAssetsInfo(IAssetsFile assetsFile)
		{
			int classID = (int)ClassIDType.AnimatorStateTransition;
			ClassIDMap classIDMap = new ClassIDMap(classID, classID);
			AssetInfo assetInfo = new AssetInfo(assetsFile, 0, classIDMap);
			return assetInfo;
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.AddSerializedVersion(SerializedVersion);
			node.Add("m_TransitionDuration", TransitionDuration);
			node.Add("m_TransitionOffset", TransitionOffset);
			node.Add("m_ExitTime", ExitTime);
			node.Add("m_HasExitTime", HasExitTime);
			node.Add("m_HasFixedDuration", HasFixedDuration);
			node.Add("m_InterruptionSource", InterruptionSource);
			node.Add("m_OrderedInterruption", OrderedInterruption);
			node.Add("m_CanTransitionToSelf", CanTransitionToSelf);
			return node;
		}

		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 3;
			}
		}

		public float TransitionDuration { get; private set; }
		public float TransitionOffset { get; private set; }
		public float ExitTime { get; private set; }
		public bool HasExitTime { get; private set; }
		public bool HasFixedDuration { get; private set; }
		public bool InterruptionSource { get; private set; }
		public bool OrderedInterruption { get; private set; }
		public bool CanTransitionToSelf { get; private set; }
	}
}
