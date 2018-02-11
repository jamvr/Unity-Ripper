using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Avatars
{
	public sealed class Human : IStreamReadable, IYAMLExportable
	{
		public Human(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;

			RootX = new XForm(assetsFile);
			Skeleton = new OffsetPtr<Skeleton>(new Skeleton(assetsFile));
			SkeletonPose = new OffsetPtr<SkeletonPose>(new SkeletonPose(assetsFile));
		}

		public void Read(EndianStream stream)
		{
			RootX.Read(stream);
			Skeleton.Read(stream);
			SkeletonPose.Read(stream);
			LeftHand.Read(stream);
			RightHand.Read(stream);
			m_handles = stream.ReadArray(() => new Handle(m_assetsFile));
			m_colliderArray = stream.ReadArray(() => new Collider(m_assetsFile));
			m_humanBoneIndex = stream.ReadInt32Array();
			m_humanBoneMass = stream.ReadSingleArray();
			m_colliderIndex = stream.ReadInt32Array();
			Scale = stream.ReadSingle();
			ArmTwist = stream.ReadSingle();
			ForeArmTwist = stream.ReadSingle();
			UpperLegTwist = stream.ReadSingle();
			LegTwist = stream.ReadSingle();
			ArmStretch = stream.ReadSingle();
			LegStretch = stream.ReadSingle();
			FeetSpacing = stream.ReadSingle();
			HasLeftHand = stream.ReadBoolean();
			HasRightHand = stream.ReadBoolean();
			HasTDoF = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.AddSerializedVersion(SerializedVersion);
			node.Add("m_RootX", RootX.ExportYAML());
			node.Add("m_Skeleton", Skeleton.ExportYAML());
			node.Add("m_SkeletonPose", SkeletonPose.ExportYAML());
			node.Add("m_LeftHand", LeftHand.ExportYAML());
			node.Add("m_RightHand", RightHand.ExportYAML());
			node.Add("m_Handles", Handles.ExportYAML());
			node.Add("m_ColliderArray", ColliderArray.ExportYAML());
			node.Add("m_HumanBoneIndex", HumanBoneIndex.ExportYAML(true));
			node.Add("m_HumanBoneMass", HumanBoneMass.ExportYAML());
			node.Add("m_ColliderIndex", ColliderIndex.ExportYAML(true));
			node.Add("m_Scale", Scale);
			node.Add("m_ArmTwist", ArmTwist);
			node.Add("m_ForeArmTwist", ForeArmTwist);
			node.Add("m_UpperLegTwist", UpperLegTwist);
			node.Add("m_LegTwist", LegTwist);
			node.Add("m_ArmStretch", ArmStretch);
			node.Add("m_LegStretch", LegStretch);
			node.Add("m_FeetSpacing", FeetSpacing);
			node.Add("m_HasLeftHand", HasLeftHand);
			node.Add("m_HasRightHand", HasRightHand);
			node.Add("m_HasTDoF", HasTDoF);
			return node;
		}

		public XForm RootX { get; }
		public OffsetPtr<Skeleton> Skeleton { get; }
		public OffsetPtr<SkeletonPose> SkeletonPose { get; }
		public OffsetPtr<Hand> LeftHand { get; } = new OffsetPtr<Hand>(new Hand());
		public OffsetPtr<Hand> RightHand { get; } = new OffsetPtr<Hand>(new Hand());
		public IReadOnlyList<Handle> Handles => m_handles;
		public IReadOnlyList<Collider> ColliderArray => m_colliderArray;
		public IReadOnlyList<int> HumanBoneIndex => m_humanBoneIndex;
		public IReadOnlyList<float> HumanBoneMass => m_humanBoneMass;
		public IReadOnlyList<int> ColliderIndex => m_colliderIndex;
		public float Scale { get; private set; }
		public float ArmTwist { get; private set; }
		public float ForeArmTwist { get; private set; }
		public float UpperLegTwist { get; private set; }
		public float LegTwist { get; private set; }
		public float ArmStretch { get; private set; }
		public float LegStretch { get; private set; }
		public float FeetSpacing { get; private set; }
		public bool HasLeftHand { get; private set; }
		public bool HasRightHand { get; private set; }
		public bool HasTDoF { get; private set; }
		
		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 2;
			}
		}

		private readonly IAssetsFile m_assetsFile;

		private Handle[] m_handles;
		private Collider[] m_colliderArray;
		private int[] m_humanBoneIndex;
		private float[] m_humanBoneMass;
		private int[] m_colliderIndex;
	}
}
