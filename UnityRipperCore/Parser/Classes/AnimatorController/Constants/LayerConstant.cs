using System;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class LayerConstant : IStreamReadable, IYAMLExportable
	{
		public LayerConstant(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			BodyMask = new HumanPoseMask(assetsFile);
		}

		public void Read(EndianStream stream)
		{
			StateMachineIndex = stream.ReadUInt32();
			StateMachineMotionSetIndex = stream.ReadUInt32();
			BodyMask.Read(stream);
			SkeletonMask.Read(stream);
			Binding = stream.ReadUInt32();
			LayerBlendingMode = stream.ReadInt32();
			DefaultWeight = stream.ReadSingle();
			IKPass = stream.ReadBoolean();
			SyncedLayerAffectsTiming = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);
		}

		public YAMLNode ExportYAML()
		{
			throw new NotSupportedException();
		}

		public uint StateMachineIndex { get; private set; }
		public uint StateMachineMotionSetIndex { get; private set; }
		public HumanPoseMask BodyMask { get; }
		public OffsetPtr<SkeletonMask> SkeletonMask { get; } = new OffsetPtr<SkeletonMask>(new SkeletonMask());
		public uint Binding { get; private set; }
		public int LayerBlendingMode { get; private set; }
		public float DefaultWeight { get; private set; }
		public bool IKPass { get; private set; }
		public bool SyncedLayerAffectsTiming { get; private set; }
	}
}
