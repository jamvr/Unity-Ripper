using System;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimationClips
{
	public sealed class Vector3Curve : IStreamReadable, IYAMLExportable
	{
		public Vector3Curve(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			Curve = new AnimationCurveTpl<Vector3f>(assetsFile);
		}

		public Vector3Curve(IAssetsFile assetsFile, string path):
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

		public AnimationCurveTpl<Vector3f> Curve { get; }
		public string Path { get; private set; }
	}
}
