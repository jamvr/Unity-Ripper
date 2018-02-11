using System.Collections.Generic;
using UnityRipper.Classes.AnimatorControllers;
using UnityRipper.Classes.AnimatorControllers.Editor;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class AnimatorController : RuntimeAnimatorController
	{
		public AnimatorController(AssetInfo assetsInfo):
			base(assetsInfo)
		{
			Controller = new ControllerConstant(AssetsFile);
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			ControllerSize = stream.ReadUInt32();
			Controller.Read(stream);
			TOS.Read(stream);
			m_animationClips = stream.ReadArray(() => new PPtr<AnimationClip>(AssetsFile));
			StateMachineBehaviourVectorDescription.Read(stream);
			m_stateMachineBehaviours = stream.ReadArray(() => new PPtr<MonoBehaviour>(AssetsFile));
			if(IsReadMultiThreadedStateMachine)
			{
				MultiThreadedStateMachine = stream.ReadBoolean();
			}
			stream.AlignStream(AlignType.Align4);
		}

		public override void Reset()
		{
			base.Reset();

			m_TOS.Clear();
		}

		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach (Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}

			foreach (PPtr<AnimationClip> ptr in AnimationClips)
			{
				AnimationClip clip = ptr.FindObject();
				if (clip == null)
				{
					if (isLog)
					{
						Logger.Log(LogType.Warning, LogCategory.Export, $"{ToLogString()} AnimationClips {ptr.ToLogString()} wasn't found ");
					}
				}
				else
				{
					yield return clip;
				}
			}

			foreach (PPtr<MonoBehaviour> ptr in StateMachineBehaviours)
			{
				MonoBehaviour behaviour = ptr.FindObject();
				if (behaviour == null)
				{
					if (isLog)
					{
						Logger.Log(LogType.Warning, LogCategory.Export, $"{ToLogString()} StateMachineBehaviours {ptr.ToLogString()} wasn't found ");
					}
				}
				else
				{
					yield return behaviour;
				}
			}
		}

#warning TODO: exporter for animator controller
		public YAMLDocument FetchSubDocuments()
		{
#warning TODO: build submachines, animstates, etc from data

			throw new System.NotImplementedException();
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
#warning TODO: build controller from data
			AnimatorControllerParameter[] @params = null;
			AnimatorControllerLayers[] layers = null;
			
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.AddSerializedVersion(SerializedVersion);
			node.Add("m_AnimatorParameters", @params.ExportYAML());
			node.Add("m_AnimatorLayers", layers.ExportYAML());
			return node;
		}

		public uint ControllerSize { get; private set; }
		public ControllerConstant Controller { get; }
		public IDictionary<uint, string> TOS => m_TOS;
		public IReadOnlyList<PPtr<AnimationClip>> AnimationClips => m_animationClips;
		public StateMachineBehaviourVectorDescription StateMachineBehaviourVectorDescription { get; } = new StateMachineBehaviourVectorDescription();
		public IReadOnlyList<PPtr<MonoBehaviour>> StateMachineBehaviours => m_stateMachineBehaviours;
		public bool MultiThreadedStateMachine { get; private set; }
		
		/// <summary>
		/// 5.4.0 and greater
		/// </summary>
		public bool IsReadMultiThreadedStateMachine => Version.IsGreaterEqual(5, 4);

		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 5;
			}
		}

		private readonly Dictionary<uint, string> m_TOS = new Dictionary<uint, string>();

		private PPtr<AnimationClip>[] m_animationClips;
		private PPtr<MonoBehaviour>[] m_stateMachineBehaviours;
	}
}