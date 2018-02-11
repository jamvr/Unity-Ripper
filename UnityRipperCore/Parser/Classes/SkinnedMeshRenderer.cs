using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class SkinnedMeshRenderer : Renderer
	{
		public SkinnedMeshRenderer(AssetInfo assetInfo):
			base(assetInfo)
		{
			Mesh = new PPtr<Mesh>(AssetsFile);
			RootBone = new PPtr<Transform>(AssetsFile);

			if(IsReadOffscreen)
			{
				DisableAnimationWhenOffscreen = new PPtr<Animation>(AssetsFile);
			}
			if(IsReadBindPose)
			{
				BindPose = new Matrix4x4f();
			}
			if(IsReadAABB)
			{
				AABB = new AABB();
			}
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			Quality = stream.ReadInt32();
			UpdateWhenOffscreen = stream.ReadBoolean();

			if(IsReadSkinNormals)
			{
				SkinNormals = stream.ReadBoolean();
			}
			if(IsReadSkinMotionVector)
			{
				SkinnedMotionVectors = stream.ReadBoolean();
			}
			stream.AlignStream(AlignType.Align4);

			if(IsReadOffscreen)
			{
				DisableAnimationWhenOffscreen.Read(stream);
			}

			Mesh.Read(stream);

			int bonesCount = stream.ReadInt32();
			m_bones = new PPtr<Transform>[bonesCount];
			for(int i = 0; i < bonesCount; i++)
			{
				PPtr<Transform> bone = new PPtr<Transform>(AssetsFile);
				bone.Read(stream);
				m_bones[i] = bone;
			}
			stream.AlignStream(AlignType.Align4);

			if(IsReadBindPose)
			{
				BindPose.Read(stream);
			}

			if(IsReadAABB)
			{
				if(IsReadWeights)
				{
					int weightCount = stream.ReadInt32();
					m_blendShapeWeights = new float[weightCount];
					for (int i = 0; i < weightCount; i++)
					{
						float weight = stream.ReadSingle();
						m_blendShapeWeights[i] = weight;
					}
				}

				if(IsReadRootBone)
				{
					RootBone.Read(stream);
				}

				AABB.Read(stream);
				DirtyAABB = stream.ReadBoolean();
				stream.AlignStream(AlignType.Align4);
			}
		}

		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}

			if(IsReadOffscreen)
			{
				if(!DisableAnimationWhenOffscreen.IsNull)
				{
					Animation anim = DisableAnimationWhenOffscreen.GetObject();
					if (anim == null)
					{
						if(isLog)
						{
							Logger.Log(LogType.Warning, LogCategory.Export, $"{ToLogString()} m_DisableAnimationWhenOffscreen {DisableAnimationWhenOffscreen.ToLogString()} wasn't found ");
						}
					}
					else
					{
						yield return anim;
					}
				}
			}
			if(!Mesh.IsNull)
			{
				Mesh mesh = Mesh.FindObject();
				if (mesh == null)
				{
					Logger.Log(LogType.Warning, LogCategory.Export, $"{ToLogString()} m_Mesh {Mesh.ToLogString()} wasn't found ");
				}
				else
				{
					yield return mesh;
				}
			}
			foreach (PPtr<Transform> ptr in Bones)
			{
				yield return ptr.GetObject();
			}
			if (!RootBone.IsNull)
			{
				yield return RootBone.GetObject();
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.AddSerializedVersion(SerializedVersion);
			node.Add("m_Quality", Quality);
			node.Add("m_UpdateWhenOffscreen", UpdateWhenOffscreen);
			node.Add("m_skinnedMotionVectors", SkinnedMotionVectors);
			node.Add("m_Mesh", Mesh.ExportYAML());
			node.Add("m_Bones", m_bones.ExportYAML());
			node.Add("m_BlendShapeWeights", m_blendShapeWeights.ExportYAML());
			node.Add("m_RootBone", RootBone.ExportYAML());
			node.Add("m_AABB", AABB.ExportYAML());
			node.Add("m_DirtyAABB", DirtyAABB);
			return node;
		}

		public int Quality { get; private set; }
		public bool UpdateWhenOffscreen { get; private set; }
		public bool SkinNormals { get; private set; }
		public bool SkinnedMotionVectors { get; private set; }
		public PPtr<Animation> DisableAnimationWhenOffscreen { get; }
		public PPtr<Mesh> Mesh { get; }
		public IReadOnlyList<PPtr<Transform>> Bones => m_bones;
		public Matrix4x4f BindPose { get; }
		public IReadOnlyList<float> BlendShapeWeights => m_blendShapeWeights;
		public PPtr<Transform> RootBone { get; }
		public AABB AABB { get; }
		public bool DirtyAABB { get; private set; }

		/// <summary>
		/// Less 3.2.0
		/// </summary>
		private bool IsReadSkinNormals => Version.IsLess(3, 2);
		/// <summary>
		/// 5.4.0 and greater
		/// </summary>
		private bool IsReadSkinMotionVector => Version.IsGreaterEqual(5, 4);
		/// <summary>
		/// Less 2.6.0
		/// </summary>
		private bool IsReadOffscreen => Version.IsLess(2, 6);
		/// <summary>
		/// Less 3.0.0
		/// </summary>
		private bool IsReadBindPose => Version.IsLess(3);
		/// <summary>
		/// 4.3.0 and greater
		/// </summary>
		private bool IsReadWeights => Version.IsGreaterEqual(4, 3);
		/// <summary>
		/// 3.5.0 and greater
		/// </summary>
		private bool IsReadRootBone => Version.IsGreaterEqual(3, 5);
		/// <summary>
		/// 3.4.0 and greater
		/// </summary>
		private bool IsReadAABB => Version.IsGreaterEqual(3, 4);

		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 2;
			}
		}

		private PPtr<Transform>[] m_bones = null;
		private float[] m_blendShapeWeights = null;
	}
}
