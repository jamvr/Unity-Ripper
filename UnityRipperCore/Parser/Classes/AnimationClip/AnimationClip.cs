using System;
using System.Collections.Generic;
using System.IO;
using UnityRipper.Classes.AnimationClips;
using UnityRipper.Classes.AnimationClips.Editor;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class AnimationClip : Motion
	{
		public AnimationClip(AssetInfo assetInfo) :
			base(assetInfo)
		{
			MuscleClip = new ClipMuscleConstant(AssetsFile);
			ClipBindingConstant = new AnimationClipBindingConstant(AssetsFile);
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			if (IsReadAnimationType)
			{
				AnimationType = (AnimationType)stream.ReadInt32();
			}
			else
			{
				Legacy = stream.ReadBoolean();
			}
			Compressed = stream.ReadBoolean();
			UseHightQualityCurve = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);

			m_rotationCurves = stream.ReadArray(() => new QuaternionCurve(AssetsFile));
			m_compressedRotationCurves = stream.ReadArray<CompressedAnimationCurve>();

			if (Compressed)
			{
#warning TODO: decompress
				throw new NotImplementedException();
			}

			if (IsReadEulerCurves)
			{
				m_eulerCurves = stream.ReadArray(() => new Vector3Curve(AssetsFile));
			}

			m_positionCurves = stream.ReadArray(() => new Vector3Curve(AssetsFile));
			m_scaleCurves = stream.ReadArray(() => new Vector3Curve(AssetsFile));
			m_floatCurves = stream.ReadArray(() => new FloatCurve(AssetsFile));
			m_PPtrCurves = stream.ReadArray(() => new PPtrCurve(AssetsFile));

			SampleRate = stream.ReadSingle();
			WrapMode = stream.ReadInt32();

			Bounds.Read(stream);

			MuscleClipSize = stream.ReadUInt32();
			MuscleClip.Read(stream);
			ClipBindingConstant.Read(stream);

			m_events = stream.ReadArray(() => new AnimationEvent(AssetsFile));
			stream.AlignStream(AlignType.Align4);
		}

		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}
			foreach (FloatCurve curve in FloatCurves)
			{
				foreach (Object @object in curve.FetchDependencies(isLog))
				{
					yield return @object;
				}
			}
			foreach (PPtrCurve curve in PPtrCurves)
			{
				foreach(Object @object in curve.FetchDependencies(isLog))
				{
					yield return @object;
				}
			}
			foreach (Object @object in ClipBindingConstant.FetchDependencies(isLog))
			{
				yield return @object;
			}
			foreach (AnimationEvent @event in Events)
			{
				foreach (Object @object in @event.FetchDependencies(isLog))
				{
					yield return @object;
				}
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.AddSerializedVersion(SerializedVersion);
			node.Add("m_Legacy", IsReadAnimationType ? true : Legacy);
			node.Add("m_Compressed", Compressed);
			node.Add("m_UseHighQualityCurve", UseHightQualityCurve);

			if(IsExportGenericData)
			{
				ExportGenericData(node);
			}
			else
			{
				node.Add("m_RotationCurves", m_rotationCurves.ExportYAML());
				node.Add("m_CompressedRotationCurves", m_compressedRotationCurves.ExportYAML());
				if (IsReadEulerCurves)
				{
					node.Add("m_EulerCurves", m_eulerCurves.ExportYAML());
				}
				else
				{
					node.Add("m_EulerCurves", YAMLMappingNode.Empty);
				}
				node.Add("m_PositionCurves", m_positionCurves.ExportYAML());
				node.Add("m_ScaleCurves", m_scaleCurves.ExportYAML());
				node.Add("m_FloatCurves", m_floatCurves.ExportYAML());
			}
			
			node.Add("m_PPtrCurves", m_PPtrCurves.ExportYAML());
			node.Add("m_SampleRate", SampleRate);
			node.Add("m_WrapMode", WrapMode);
			node.Add("m_Bounds", Bounds.ExportYAML());
			node.Add("m_ClipBindingConstant", ClipBindingConstant.ExportYAML());
			node.Add("m_AnimationClipSettings", MuscleClip.ExportYAML());
			node.Add("m_EditorCurves", YAMLSequenceNode.Empty);
			node.Add("m_EulerEditorCurves", YAMLSequenceNode.Empty);
			node.Add("m_HasGenericRootTransform", false);
			node.Add("m_HasMotionFloatCurves", false);
			node.Add("m_GenerateMotionCurves", false);
			node.Add("m_Events", m_events.ExportYAML());
			
			return node;
		}

		private void ExportGenericData(YAMLMappingNode node)
		{
			IReadOnlyDictionary<uint, string> tos = RetrieveTOS();
			/*if(tos == null)
			{
				ExportEmptyGenericData(node);
				return;
			}*/

			ExportGenericData(node, tos);
		}

#warning TODO: it's too complicated and unintuitive. need to simplify
		private void ExportGenericData(YAMLMappingNode node, IReadOnlyDictionary<uint, string> tos)
		{
			StreamedClip streamedClip = MuscleClip.Clip.StreamedClip;
			DenseClip denseClip = MuscleClip.Clip.DenseClip;
			ConstantClip constantClip = MuscleClip.Clip.ConstantClip;

			IReadOnlyList<StreamedFrame> frames = streamedClip.GenerateFrames();
			Dictionary<uint, Vector3Curve> translations = new Dictionary<uint, Vector3Curve>();
			Dictionary<uint, QuaternionCurve> rotations = new Dictionary<uint, QuaternionCurve>();
			Dictionary<uint, Vector3Curve> scales = new Dictionary<uint, Vector3Curve>();
			Dictionary<uint, Vector3Curve> eulers = new Dictionary<uint, Vector3Curve>();
			Dictionary<uint, FloatCurve> floats = new Dictionary<uint, FloatCurve>();

			int frameCount = Math.Max(denseClip.FrameCount - 1, frames.Count - 2);
			float[] frameCurvesValue = new float[streamedClip.CurveCount];
			for (int frame = 0, streamFrame = 1; frame < frameCount; frame++, streamFrame++)
			{
				bool isAdd = true;
				float time;
				StreamedFrame streamedFrame = null;
				if (streamFrame < frames.Count)
				{
					streamedFrame = frames[streamFrame];
					time = streamedFrame.Time;
				}
				else
				{
					time = (float)frame / SampleRate;
				}
				// number of stream curves which has key in current frame
				int streamFrameCurveCount = streamFrame < frames.Count ? streamedFrame.Curves.Count : 0;
				// total amount of curves which has key in current frame
				int frameCurveCount = streamFrameCurveCount + (int)denseClip.CurveCount + constantClip.Constants.Count;
				int streamOffset = (int)streamedClip.CurveCount - streamFrameCurveCount;
				for (int curve = 0; curve < frameCurveCount;)
				{
					int curveIndex;
					IReadOnlyList<float> curvesValue;
					int offset;

					if (streamFrame < frames.Count && curve < streamedFrame.Curves.Count)
					{
#warning TODO: read TCB and convert to in/out slope
#warning TODO: write only few values
						for (int key = 0; key < streamedFrame.Curves.Count; key++)
						{
							frameCurvesValue[key] = streamedFrame.Curves[key].Value;
						}
						curveIndex = streamedFrame.Curves[curve].Index;
						curvesValue = frameCurvesValue;
						offset = 0;
					}
					else if (curve < streamFrameCurveCount + denseClip.CurveCount)
					{
						curveIndex = curve + streamOffset;
						curvesValue = denseClip.SampleArray;
						offset = streamFrameCurveCount - frame * (int)denseClip.CurveCount;
					}
					else
					{
						curveIndex = curve + streamOffset;
						curvesValue = constantClip.Constants;
						offset = streamFrameCurveCount + (int)denseClip.CurveCount;
						isAdd = frame == 0 || frame == frameCount - 1;
					}

					GenericBinding binding = ClipBindingConstant.FindBinding(curveIndex);
					uint pathHash = binding.Path;

					string path;
					if (pathHash == 0)
					{
						curve++;
						continue;
					}
					if (!tos.TryGetValue(pathHash, out path))
					{
						path = "dummy" + pathHash;
						//Logger.Log(LogType.Debug, LogCategory.Export, $"Can't find path '{binding.Path}' in TOS for {ToLogString()}");
					}

					switch (binding.BindingType)
					{
						case BindingType.Translation:
							float x = curvesValue[curve++ - offset];
							float y = curvesValue[curve++ - offset];
							float z = curvesValue[curve++ - offset];
							float w = 0;
							if (isAdd)
							{
								Vector3f trans = new Vector3f(x, y, z);

								Vector3Curve transCurve;
								if (!translations.TryGetValue(pathHash, out transCurve))
								{
									transCurve = new Vector3Curve(AssetsFile, path);
									translations[pathHash] = transCurve;
								}

								KeyframeTpl<Vector3f> transKey = new KeyframeTpl<Vector3f>(AssetsFile, time, trans);
								transCurve.Curve.Curve.Add(transKey);
							}
							break;

						case BindingType.Rotation:
							x = curvesValue[curve++ - offset];
							y = curvesValue[curve++ - offset];
							z = curvesValue[curve++ - offset];
							w = curvesValue[curve++ - offset];
							if (isAdd)
							{
								Quaternionf rot = new Quaternionf(x, y, z, w);

								QuaternionCurve rotCurve;
								if (!rotations.TryGetValue(pathHash, out rotCurve))
								{
									rotCurve = new QuaternionCurve(AssetsFile, path);
									rotations[pathHash] = rotCurve;
								}

								KeyframeTpl<Quaternionf> rotKey = new KeyframeTpl<Quaternionf>(AssetsFile, time, rot);
								rotCurve.Curve.Curve.Add(rotKey);
							}
							break;

						case BindingType.Scaling:
							x = curvesValue[curve++ - offset];
							y = curvesValue[curve++ - offset];
							z = curvesValue[curve++ - offset];
							if(isAdd)
							{
								Vector3f scale = new Vector3f(x, y, z);

								Vector3Curve scaleCurve;
								if (!scales.TryGetValue(pathHash, out scaleCurve))
								{
									scaleCurve = new Vector3Curve(AssetsFile, path);
									scales[pathHash] = scaleCurve;
								}

								KeyframeTpl<Vector3f> scaleKey = new KeyframeTpl<Vector3f>(AssetsFile, time, scale);
								scaleCurve.Curve.Curve.Add(scaleKey);
							}
							break;

						case BindingType.EulerRotation:
							x = curvesValue[curve++ - offset];
							y = curvesValue[curve++ - offset];
							z = curvesValue[curve++ - offset];
							if (isAdd)
							{
								Vector3f euler = new Vector3f(x, y, z);

								Vector3Curve eulerCurve;
								if (!eulers.TryGetValue(pathHash, out eulerCurve))
								{
									eulerCurve = new Vector3Curve(AssetsFile, path);
									eulers[pathHash] = eulerCurve;
								}

								KeyframeTpl<Vector3f> eulerKey = new KeyframeTpl<Vector3f>(AssetsFile, time, euler);
								eulerCurve.Curve.Curve.Add(eulerKey);
							}
							break;

						case BindingType.Floats:
							float value = curvesValue[curve++ - offset];
							if (isAdd)
							{
								Float @float = new Float(value);

								FloatCurve floatCurve;
								if (!floats.TryGetValue(pathHash, out floatCurve))
								{
									floatCurve = new FloatCurve(AssetsFile, path);
									floats[pathHash] = floatCurve;
								}

								KeyframeTpl<Float> floatKey = new KeyframeTpl<Float>(AssetsFile, time, @float);
								floatCurve.Curve.Curve.Add(floatKey);
							}
							break;

						default:
#warning TODO: ???
							curve++;
							//throw new NotImplementedException(binding.BindingType.ToString());
							break;
					}
				}
			}

			node.Add("m_RotationCurves", rotations.Values.ExportYAML());
			node.Add("m_CompressedRotationCurves", YAMLMappingNode.Empty);
			node.Add("m_EulerCurves", eulers.Values.ExportYAML());
			node.Add("m_PositionCurves", translations.Values.ExportYAML());
			node.Add("m_ScaleCurves", scales.Values.ExportYAML());
			node.Add("m_FloatCurves", floats.Values.ExportYAML());
		}

		private void ExportEmptyGenericData(YAMLMappingNode node)
		{
			node.Add("m_RotationCurves", YAMLMappingNode.Empty);
			node.Add("m_CompressedRotationCurves", YAMLMappingNode.Empty);
			node.Add("m_EulerCurves", YAMLMappingNode.Empty);
			node.Add("m_PositionCurves", YAMLMappingNode.Empty);
			node.Add("m_ScaleCurves", YAMLMappingNode.Empty);
			node.Add("m_FloatCurves", YAMLMappingNode.Empty);
		}

		private IReadOnlyDictionary<uint, string> RetrieveTOS()
		{
			Avatar avatar = FindAvatar();
			if(avatar == null)
			{
				//Logger.Log(LogType.Warning, LogCategory.Export, $"Avatar for {ToLogString()} wasn't found");
			}
			else
			{
				return avatar.TOS;
			}

#warning TODO: build TOS with transforms
			return new Dictionary<uint, string>() { { 0, string.Empty } };
		}

		private Avatar FindAvatar()
		{
			foreach (Object @object in AssetsFile.Collection.FetchObjects())
			{
				if (@object.ClassID != ClassIDType.Animator)
				{
					continue;
				}

				Animator animator = (Animator)@object;
				RuntimeAnimatorController runetime = animator.Controller.FindObject();
				if (runetime == null)
				{
					continue;
				}

				AnimatorOverrideController @override = runetime as AnimatorOverrideController;
				if (@override == null)
				{
					AnimatorController controller = (AnimatorController)runetime;
					foreach (PPtr<AnimationClip> clip in controller.AnimationClips)
					{
						if (clip.PathID == PathID)
						{
							return animator.Avatar.FindObject();
						}
					}
				}
				else
				{
					foreach (var clip in @override.Clips)
					{
						if (clip.OverrideClip.PathID == PathID)
						{
							return animator.Avatar.FindObject();
						}
					}
				}
			}
			return null;
		}
		
		public override string ExportExtension => "anim";
		
		public AnimationType AnimationType { get; private set; }
		public bool Legacy { get; private set; }
		public bool Compressed { get; private set; }
		public bool UseHightQualityCurve { get; private set; }
		public IReadOnlyList<QuaternionCurve> RotationCurves => m_rotationCurves;
		public IReadOnlyList<CompressedAnimationCurve> CompressedRotationCurves => m_compressedRotationCurves;
		public IReadOnlyList<Vector3Curve> EulerCurves => m_eulerCurves;
		public IReadOnlyList<Vector3Curve> PositionCurves => m_positionCurves;
		public IReadOnlyList<Vector3Curve> ScaleCurves => m_scaleCurves;
		public IReadOnlyList<FloatCurve> FloatCurves => m_floatCurves;
		public IReadOnlyList<PPtrCurve> PPtrCurves => m_PPtrCurves;
		public float SampleRate { get; private set; }
		public int WrapMode { get; private set; }
		public AABB Bounds { get; } = new AABB();
		public uint MuscleClipSize { get; private set; }
		public ClipMuscleConstant MuscleClip { get; }
		public AnimationClipBindingConstant ClipBindingConstant { get; }
		public IReadOnlyList<AnimationEvent> Events => m_events;

		/// <summary>
		/// Less than 5.0.0
		/// </summary>
		public bool IsReadAnimationType => Version.IsLess(5);
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		public bool IsReadLegacy => !IsReadAnimationType;
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		public bool IsReadEulerCurves => Version.IsGreaterEqual(5);

#warning what about humanoid?
		private bool IsExportGenericData
		{
			get
			{
				if(IsReadAnimationType && AnimationType == AnimationType.Mecanim)
				{
					return true;
				}
				if(MuscleClip.Clip.IsValid)
				{
					return true;
				}
				return false;
			}
		}

		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 6;
			}
		}

		private QuaternionCurve[] m_rotationCurves;
		private CompressedAnimationCurve[] m_compressedRotationCurves;
		private Vector3Curve[] m_eulerCurves;
		private Vector3Curve[] m_positionCurves;
		private Vector3Curve[] m_scaleCurves;
		private FloatCurve[] m_floatCurves;
		private PPtrCurve[] m_PPtrCurves;
		private AnimationEvent[] m_events;
	}
}
