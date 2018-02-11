using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimationClips
{
	public sealed class PPtrCurve : IStreamReadable, IYAMLExportable
	{
		public PPtrCurve(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
			Script = new PPtr<MonoScript>(assetsFile);
		}

		public void Read(EndianStream stream)
		{
			m_curve = stream.ReadArray(() => new PPtrKeyframe(m_assetsFile));
			stream.AlignStream(AlignType.Align4);

			Attribute = stream.ReadStringAligned();
			Path = stream.ReadStringAligned();
			ClassID = stream.ReadInt32();
			Script.Read(stream);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("curve", Curve.ExportYAML());
			node.Add("attribute", Attribute);
			node.Add("path", Path);
			node.Add("classID", ClassID);
			node.Add("script", Script.ExportYAML());
			return node;
		}

		public IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach(PPtrKeyframe keyframe in Curve)
			{
				foreach (Object @object in keyframe.FetchDependencies(isLog))
				{
					yield return @object;
				}
			}
			MonoScript script = Script.FindObject();
			if(script == null)
			{
				if(isLog)
				{
					Logger.Log(LogType.Warning, LogCategory.Export, $"PPtrCurve's script {Script.ToLogString()} wasn't found ");
				}
			}
			else
			{
				yield return script;
			}
		}

		public IReadOnlyList<PPtrKeyframe> Curve => m_curve;
		public string Attribute { get; private set; }
		public string Path { get; private set; }
		public int ClassID { get; private set; }
		public PPtr<MonoScript> Script { get; }

		private readonly IAssetsFile m_assetsFile;

		private PPtrKeyframe[] m_curve;
	}
}
