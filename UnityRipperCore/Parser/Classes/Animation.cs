using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class Animation : Behaviour
	{
		public Animation(AssetInfo assetInfo) :
			base(assetInfo)
		{
			DefaultAnimation = new PPtr<AnimationClip>(AssetsFile);
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);
			
			DefaultAnimation.Read(stream);
			m_animations = stream.ReadArray(() => new PPtr<AnimationClip>(AssetsFile));
			WrapMode = stream.ReadInt32();
			PlayAutomatically = stream.ReadBoolean();
			AnimatePhysics = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);

			CullingType = stream.ReadInt32();
		}

		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}

			AnimationClip defClip = DefaultAnimation.FindObject();
			if(defClip != null)
			{
				yield return defClip;
			}
			foreach(PPtr<AnimationClip> ptr in Animations)
			{
				AnimationClip clip = ptr.FindObject();
				if(clip != null)
				{
					yield return clip;
				}
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.AddSerializedVersion(SerializedVersion);
			node.Add("m_Animation", DefaultAnimation.ExportYAML());
			node.Add("m_Animations", Animations.ExportYAML());
			node.Add("m_WrapMode", WrapMode);
			node.Add("m_PlayAutomatically", PlayAutomatically);
			node.Add("m_AnimatePhysics", AnimatePhysics);
			node.Add("m_CullingType", CullingType);
			return node;
		}
		
		public PPtr<AnimationClip> DefaultAnimation { get; }
		public IReadOnlyList<PPtr<AnimationClip>> Animations => m_animations;
		public int WrapMode { get; private set; }
		public bool PlayAutomatically { get; private set; }
		public bool AnimatePhysics { get; private set; }
		public int CullingType { get; private set; }
	
		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 3;
			}
		}

		private PPtr<AnimationClip>[] m_animations;
	}
}
