using System;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers.Editor
{
	public sealed class ChildAnimatorStateMachine : IYAMLExportable
	{
		public ChildAnimatorStateMachine(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}

			StateMachine = new PPtr<AnimatorStateMachine>(assetsFile);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.AddSerializedVersion(SerializedVersion);
			node.Add("m_StateMachine", StateMachine.ExportYAML());
			node.Add("m_Position", Position.ExportYAML());
			return node;
		}

		public PPtr<AnimatorStateMachine> StateMachine { get; }
		public Vector3f Position { get; } = new Vector3f();

		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 1;
			}
		}
	}
}
