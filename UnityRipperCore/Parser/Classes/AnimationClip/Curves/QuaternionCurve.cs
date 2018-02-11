using System;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimationClips
{
	public sealed class QuaternionCurve : IStreamReadable, IYAMLExportable
	{
		public QuaternionCurve(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			Curve = new AnimationCurveTpl<Quaternionf>(assetsFile);
		}

		public QuaternionCurve(IAssetsFile assetsFile, string path) :
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
			Path = stream.ReadStringAligned();
		}
		
		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("curve", Curve.ExportYAML());
			node.Add("path", Path);

			return node;
		}

		public AnimationCurveTpl<Quaternionf> Curve { get; }
		public string Path { get; set; }
	}
}
