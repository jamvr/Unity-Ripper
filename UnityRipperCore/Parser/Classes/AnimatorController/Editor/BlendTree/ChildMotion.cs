using System;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers.Editor
{
	public sealed class ChildMotion : IYAMLExportable
	{
		public ChildMotion(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new Exception(nameof(assetsFile));
			}

			Motion = new PPtr<Motion>(assetsFile);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.AddSerializedVersion(SerializedVersion);
			node.Add("m_Motion", Motion.ExportYAML());
			node.Add("m_Threshold", Threshold);
			node.Add("m_Position", Position.ExportYAML());
			node.Add("m_TimeScale", TimeScale);
			node.Add("m_CycleOffset", CycleOffset);
			node.Add("m_DirectBlendParameter", DirectBlendParameter);
			node.Add("m_Mirror", Mirror);
			return node;
		}

		public PPtr<Motion> Motion { get; }
		public float Threshold { get; private set; }
		public Vector2f Position { get; } = new Vector2f();
		public float TimeScale { get; private set; }
		public float CycleOffset { get; private set; }
		public string DirectBlendParameter { get; private set; }
		public bool Mirror { get; private set; }

		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 2;
			}
		}
	}
}
