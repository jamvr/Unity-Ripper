using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimationClips
{
	public sealed class PPtrKeyframe : IStreamReadable, IYAMLExportable
	{
		public PPtrKeyframe(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException("version");
			}
			Script = new PPtr<Object>(assetsFile);
		}

		public void Read(EndianStream stream)
		{
			Time = stream.ReadSingle();
			Script.Read(stream);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("time", Time);
			node.Add("value", Script.ExportYAML());
			return node;
		}

		public IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			Object script = Script.FindObject();
			if(script == null)
			{
				if(isLog)
				{
					Logger.Log(LogType.Warning, LogCategory.Export, $"PPtrKeyframe's script {Script.ToLogString()} wasn't found ");
				}
			}
			else
			{
				yield return script;
			}
		}

		public float Time { get; private set; }
		public PPtr<Object> Script { get; }
	}
}
