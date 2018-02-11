using System.Collections.Generic;
using UnityRipper.Classes.AnimatorOverrideControllers;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class AnimatorOverrideController : RuntimeAnimatorController
	{
		public AnimatorOverrideController(AssetInfo assetInfo) :
			base(assetInfo)
		{
			Controller = new PPtr<RuntimeAnimatorController>(AssetsFile);
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			Controller.Read(stream);
			m_clips = stream.ReadArray(() => new AnimationClipOverride(AssetsFile));
		}

		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}
			RuntimeAnimatorController runetime = Controller.FindObject();
			if(runetime == null)
			{
				if(isLog)
				{
					Logger.Log(LogType.Warning, LogCategory.Export, $"{ToLogString()} m_Controller {Controller.ToLogString()} wasn't found ");
				}
			}
			else
			{
				yield return runetime;
			}
			foreach (AnimationClipOverride clip in Clips)
			{
				foreach (Object @object in clip.FetchDependencies(isLog))
				{
					yield return @object;
				}
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.Add("m_Controller", Controller.ExportYAML());
			node.Add("m_Clips", Clips.ExportYAML());
			return node;
		}

		public override string ExportExtension => "overrideController";

		public PPtr<RuntimeAnimatorController> Controller { get; }
		public IReadOnlyList<AnimationClipOverride> Clips => m_clips;

		private AnimationClipOverride[] m_clips;
	}
}
