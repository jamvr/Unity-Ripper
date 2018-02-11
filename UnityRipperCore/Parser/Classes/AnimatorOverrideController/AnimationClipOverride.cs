using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorOverrideControllers
{
	public sealed class AnimationClipOverride : IStreamReadable, IYAMLExportable
	{
		public AnimationClipOverride(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}

			OriginalClip = new PPtr<AnimationClip>(assetsFile);
			OverrideClip = new PPtr<AnimationClip>(assetsFile);
		}
		public void Read(EndianStream stream)
		{
			OriginalClip.Read(stream);
			OverrideClip.Read(stream);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_OriginalClip", OriginalClip.ExportYAML());
			node.Add("m_OverrideClip", OverrideClip.ExportYAML());
			return node;
		}

		public IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			AnimationClip clip = OriginalClip.FindObject();
			if (clip == null)
			{
				if(isLog)
				{
					Logger.Log(LogType.Warning, LogCategory.Export, $"AnimationClipOverride's m_OriginalClip {OriginalClip.ToLogString()} wasn't found ");
				}
			}
			else
			{
				yield return clip;
			}

			clip = OverrideClip.FindObject();
			if (clip == null)
			{
				if (isLog)
				{
					Logger.Log(LogType.Warning, LogCategory.Export, $"AnimationClipOverride's m_OverrideClip {OverrideClip.ToLogString()} wasn't found ");
				}
			}
			else
			{
				yield return clip;
			}
		}

		public PPtr<AnimationClip> OriginalClip { get; }
		public PPtr<AnimationClip> OverrideClip { get; }
	}
}
