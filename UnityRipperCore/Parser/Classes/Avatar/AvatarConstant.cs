using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Avatars
{
	public sealed class AvatarConstant : IStreamReadable, IYAMLExportable
	{
		public AvatarConstant(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;

			AvatarSkeleton = new OffsetPtr<Skeleton>(new Skeleton(assetsFile));
			RootMotionBoneX = new XForm(assetsFile);
			AvatarSkeletonPose = new OffsetPtr<SkeletonPose>(new SkeletonPose(assetsFile));
			DefaultPose = new OffsetPtr<SkeletonPose>(new SkeletonPose(assetsFile));
			Human = new OffsetPtr<Human>(new Human(assetsFile));
			RootMotionSkeleton = new OffsetPtr<Skeleton>(new Skeleton(assetsFile));
			RootMotionSkeletonPose = new OffsetPtr<SkeletonPose>(new SkeletonPose(assetsFile));
		}

		public void Read(EndianStream stream)
		{
			AvatarSkeleton.Read(stream);
			AvatarSkeletonPose.Read(stream);
			DefaultPose.Read(stream);
			m_skeletonNameIDArray = stream.ReadUInt32Array();
			Human.Read(stream);
			m_humanSkeletonIndexArray = stream.ReadInt32Array();
			m_humanSkeletonReverseIndexArray = stream.ReadInt32Array();
			RootMotionBoneIndex = stream.ReadInt32();
			RootMotionBoneX.Read(stream);
			RootMotionSkeleton.Read(stream);
			RootMotionSkeletonPose.Read(stream);
			m_rootMotionSkeletonIndexArray = stream.ReadInt32Array();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.AddSerializedVersion(SerializedVersion);
			node.Add("m_AvatarSkeleton", AvatarSkeleton.ExportYAML());
			node.Add("m_AvatarSkeletonPose", AvatarSkeletonPose.ExportYAML());
			node.Add("m_DefaultPose", DefaultPose.ExportYAML());
			node.Add("m_SkeletonNameIDArray", SkeletonNameIDArray.ExportYAML(true));
			node.Add("m_Human", Human.ExportYAML());
			node.Add("m_HumanSkeletonIndexArray", HumanSkeletonIndexArray.ExportYAML(true));
			node.Add("m_HumanSkeletonReverseIndexArray", HumanSkeletonReverseIndexArray.ExportYAML(true));
#warning other values crash Unity. Why?
			node.Add("m_RootMotionBoneIndex", -1/*RootMotionBoneIndex*/);
			node.Add("m_RootMotionBoneX", RootMotionBoneX.ExportYAML());
			node.Add("m_RootMotionSkeleton", RootMotionSkeleton.ExportYAML());
			node.Add("m_RootMotionSkeletonPose", RootMotionSkeletonPose.ExportYAML());
			node.Add("m_RootMotionSkeletonIndexArray", RootMotionSkeletonIndexArray.ExportYAML(true));
			return node;
		}

		public OffsetPtr<Skeleton> AvatarSkeleton { get; }
		public OffsetPtr<SkeletonPose> AvatarSkeletonPose { get; }
		public OffsetPtr<SkeletonPose> DefaultPose { get; }
		public IReadOnlyList<uint> SkeletonNameIDArray => m_skeletonNameIDArray;
		public OffsetPtr<Human> Human { get; }
		public IReadOnlyList<int> HumanSkeletonIndexArray => m_humanSkeletonIndexArray;
		public IReadOnlyList<int> HumanSkeletonReverseIndexArray => m_humanSkeletonReverseIndexArray;
		public int RootMotionBoneIndex { get; private set; }
		public XForm RootMotionBoneX { get; }
		public OffsetPtr<Skeleton> RootMotionSkeleton { get; }
		public OffsetPtr<SkeletonPose> RootMotionSkeletonPose { get; }
		public IReadOnlyList<int> RootMotionSkeletonIndexArray => m_rootMotionSkeletonIndexArray;

		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 3;
			}
		}

		private readonly IAssetsFile m_assetsFile;

		private uint[] m_skeletonNameIDArray;
		private int[] m_humanSkeletonIndexArray;
		private int[] m_humanSkeletonReverseIndexArray;
		private int[] m_rootMotionSkeletonIndexArray;
	}
}
