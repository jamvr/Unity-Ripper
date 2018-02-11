using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimationClips
{
	public sealed class FloatCurve : IStreamReadable, IYAMLExportable
	{
		public FloatCurve(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			
			Curve = new AnimationCurveTpl<Float>(assetsFile);
			Script = new PPtr<MonoScript>(assetsFile);
		}

		public FloatCurve(IAssetsFile assetsFile, string path) :
			this(assetsFile)
		{
			Path = path;
			Curve.PreInfinity = 2;
			Curve.PostInfinity = 2;
			Curve.RotationOrder = 4;
		}

		public void Read(EndianStream stream)
		{
			Curve.Read(stream);
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
			MonoScript script = Script.FindObject();
			if (script == null)
			{
				if(isLog)
				{
					Logger.Log(LogType.Warning, LogCategory.Export, $"FloatCurve's script {Script.ToLogString()} wasn't found ");
				}
			}
			else
			{
				yield return script;
			}
		}

		public AnimationCurveTpl<Float> Curve { get; }
		public string Attribute { get; private set; }
		public string Path { get; set; }
		public int ClassID { get; private set; }
		public PPtr<MonoScript> Script { get; }
	}
}
