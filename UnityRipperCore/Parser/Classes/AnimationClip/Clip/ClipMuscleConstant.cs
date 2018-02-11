using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimationClips
{
	public sealed class ClipMuscleConstant : IStreamReadable, IYAMLExportable
	{
		public ClipMuscleConstant(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}

			m_assetsFile = assetsFile;
			DeltaPose = new HumanPose(assetsFile);
			StartX = new XForm(assetsFile);
			LeftFootStartX = new XForm(assetsFile);
			RightFootStartX = new XForm(assetsFile);
			Clip = new Clip(assetsFile);

			if(IsReadStopX)
			{
				StopX = new XForm(assetsFile);
			}
			if(IsReadMotion)
			{
				MotionStartX = new XForm(assetsFile);
				MotionStopX = new XForm(assetsFile);
			}
			if (IsVector3)
			{
				AverageSpeed3 = new Vector3f();
			}
			else
			{
				AverageSpeed4 = new Vector4f();
			}
	}

		public void Read(EndianStream stream)
		{
			DeltaPose.Read(stream);
			StartX.Read(stream);
			if (IsReadStopX)
			{
				StopX.Read(stream);
			}
			LeftFootStartX.Read(stream);
			RightFootStartX.Read(stream);

			if (IsReadMotion)
			{
				MotionStartX.Read(stream);
				MotionStopX.Read(stream);
			}

			if(IsVector3)
			{
				AverageSpeed3.Read(stream);
			}
			else
			{
				AverageSpeed4.Read(stream);
			}

			Clip.Read(stream);

			StartTime = stream.ReadSingle();
			StopTime = stream.ReadSingle();
			OrientationOffsetY = stream.ReadSingle();
			Level = stream.ReadSingle();
			CycleOffset = stream.ReadSingle();
			AverageAngularSpeed = stream.ReadSingle();

			m_indexArray = stream.ReadInt32Array();
			m_valueArrayDelta = stream.ReadArray<ValueDelta>();

			if(IsReadValueArrayReferencePose)
			{
				m_valueArrayReferencePose = stream.ReadSingleArray();
			}

			Mirror = stream.ReadBoolean();
			LoopTime = stream.ReadBoolean();
			LoopBlend = stream.ReadBoolean();
			LoopBlendOrientation = stream.ReadBoolean();
			LoopBlendPositionY = stream.ReadBoolean();
			LoopBlendPositionXZ = stream.ReadBoolean();

			if(IsReadStartAtOrigin)
			{
				StartAtOrigin = stream.ReadBoolean();
			}

			KeepOriginalOrientation = stream.ReadBoolean();
			KeepOriginalPositionY = stream.ReadBoolean();
			KeepOriginalPositionXZ = stream.ReadBoolean();
			HeightFromFeet = stream.ReadBoolean();

			stream.AlignStream(AlignType.Align4);
		}
		
		public YAMLNode ExportYAML()
		{
#warning TODO: deal with this strage export
			return ExportAnimationClipSettingsYAML();
		}

		private YAMLNode ExportAnimationClipSettingsYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.AddSerializedVersion(AnimationClipSettingsSerializedVersion);
			node.Add("m_AdditiveReferencePoseClip", PPtr<AnimationClip>.Empty.ExportYAML());
			node.Add("m_AdditiveReferencePoseTime", 0);
			node.Add("m_StartTime", StartTime);
			node.Add("m_StopTime", StopTime);
			node.Add("m_OrientationOffsetY", OrientationOffsetY);
			node.Add("m_Level", Level);
			node.Add("m_CycleOffset", CycleOffset);
			node.Add("m_HasAdditiveReferencePose", false);
			node.Add("m_LoopTime", LoopTime);
			node.Add("m_LoopBlend", LoopBlend);
			node.Add("m_LoopBlendOrientation", LoopBlendOrientation);
			node.Add("m_LoopBlendPositionY", LoopBlendPositionY);
			node.Add("m_LoopBlendPositionXZ", LoopBlendPositionXZ);
			node.Add("m_KeepOriginalOrientation", KeepOriginalOrientation);
			node.Add("m_KeepOriginalPositionY", KeepOriginalPositionY);
			node.Add("m_KeepOriginalPositionXZ", KeepOriginalPositionXZ);
			node.Add("m_HeightFromFeet", HeightFromFeet);
			node.Add("m_Mirror", 0);
			return node;
		}

		public HumanPose DeltaPose { get; }
		public XForm StartX { get; }
		public XForm StopX { get; }
		public XForm LeftFootStartX { get; }
		public XForm RightFootStartX { get; }
		public XForm MotionStartX { get; }
		public XForm MotionStopX { get; }
		public Vector3f AverageSpeed3 { get; }
		public Vector4f AverageSpeed4 { get; }
		public Clip Clip { get; }
		public float StartTime { get; private set; }
		public float StopTime { get; private set; }
		public float OrientationOffsetY { get; private set; }
		public float Level { get; private set; }
		public float CycleOffset { get; private set; }
		public float AverageAngularSpeed { get; private set; }
		public IReadOnlyList<int> IndexArray => m_indexArray;
		public IReadOnlyList<ValueDelta> ValueArrayDelta => m_valueArrayDelta;
		public IReadOnlyList<float> ValueArrayReferencePose => m_valueArrayReferencePose;
		public bool Mirror { get; private set; }
		public bool LoopTime { get; private set; }
		public bool LoopBlend { get; private set; }
		public bool LoopBlendOrientation { get; private set; }
		public bool LoopBlendPositionY { get; private set; }
		public bool LoopBlendPositionXZ { get; private set; }
		public bool StartAtOrigin { get; private set; }
		public bool KeepOriginalOrientation { get; private set; }
		public bool KeepOriginalPositionY { get; private set; }
		public bool KeepOriginalPositionXZ { get; private set; }
		public bool HeightFromFeet { get; private set; }

		private Version Version => m_assetsFile.Version;

		/// <summary>
		/// 5.5.0 and greater
		/// </summary>
		public bool IsReadStopX => Version.IsGreaterEqual(5, 5);
		/// <summary>
		/// Less than 5.0.0
		/// </summary>
		public bool IsReadMotion => Version.IsLess(5);
		/// <summary>
		/// 5.4.0 and greater
		/// </summary>
		public bool IsVector3 => Version.IsGreaterEqual(5, 4);
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		public bool IsReadValueArrayReferencePose => Version.IsGreaterEqual(5, 0);
		/// <summary>
		/// 5.5.0 and greater
		/// </summary>
		public bool IsReadStartAtOrigin => Version.IsGreaterEqual(5, 5);

		private int AnimationClipSettingsSerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 2;
			}
		}

		private readonly IAssetsFile m_assetsFile;

		private int[] m_indexArray;
		private ValueDelta[] m_valueArrayDelta;
		private float[] m_valueArrayReferencePose;
	}
}
