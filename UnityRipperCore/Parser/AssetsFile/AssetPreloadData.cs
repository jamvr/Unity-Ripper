using System;
using UnityRipper.Classes;

using Object = UnityRipper.Classes.Object;

namespace UnityRipper.AssetsFiles
{
	public class AssetPreloadData
	{
		public AssetPreloadData(AssetInfo info, long offset, long length)
		{
			if(info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}
			if (offset < 0)
			{
				throw new ArgumentException(nameof(offset));
			}
			if (length < 0)
			{
				throw new ArgumentException(nameof(length));
			}

			AssetInfo = info;
			m_offset = offset;
			m_length = length;
		}

		public void Parse(EndianStream stream)
		{
			switch (UnityType)
			{
				case ClassIDType.GameObject:
					Object = new GameObject(AssetInfo);
					break;

				case ClassIDType.Transform:
					Object = new Transform(AssetInfo);
					break;

				case ClassIDType.Material:
					Object = new Material(AssetInfo);
					break;

				case ClassIDType.MeshRenderer:
					Object = new MeshRenderer(AssetInfo);
					break;

				case ClassIDType.Texture2D:
					Object = new Texture2D(AssetInfo);
					break;

				case ClassIDType.MeshFilter:
					Object = new MeshFilter(AssetInfo);
					break;

				case ClassIDType.Mesh:
					Object = new Mesh(AssetInfo);
					break;

				case ClassIDType.Shader:
					if (SShader.IsSerialized(AssetInfo.AssetFile.Version))
					{
						Object = new SShader(AssetInfo);
					}
					else
					{
						Object = new Shader(AssetInfo);
					}
					break;

				case ClassIDType.TextAsset:
					Object = new TextAsset(AssetInfo);
					break;

				case ClassIDType.AnimationClip:
					Object = new AnimationClip(AssetInfo);
					break;

				case ClassIDType.Avatar:
					Object = new Avatar(AssetInfo);
					break;

				case ClassIDType.AnimatorController:
					Object = new AnimatorController(AssetInfo);
					break;

				case ClassIDType.Animator:
					Object = new Animator(AssetInfo);
					break;

				case ClassIDType.Animation:
					Object = new Animation(AssetInfo);
					break;

				case ClassIDType.MonoScript:
					Object = new MonoScript(AssetInfo);
					break;

				case ClassIDType.SkinnedMeshRenderer:
					Object = new SkinnedMeshRenderer(AssetInfo);
					break;

				case ClassIDType.BuildSettings:
					Object = new BuildSettings(AssetInfo);
					break;

				case ClassIDType.AssetBundle:
					Object = new AssetBundle(AssetInfo);
					break;

				case ClassIDType.AnimatorOverrideController:
					Object = new AnimatorOverrideController(AssetInfo);
					break;

				case ClassIDType.RectTransform:
					Object = new RectTransform(AssetInfo);
					break;

				default:
					return;
			}

			stream.BaseStream.Position = m_offset;
			if (Config.IsGenerateGUIDByContent)
			{
				if(m_length > int.MaxValue)
				{
					throw new Exception($"Can't read enough data {m_length}");
				}
				byte[] data = stream.ReadBytes((int)m_length);
				Object.Read(data);
			}
			else
			{
				stream.AlignPosition = m_offset;
				Object.Read(stream);
				long read = stream.BaseStream.Position - m_offset;
				if (read != m_length)
				{
					throw new Exception($"Read {read} but expected {m_length} for object {Object.GetType().Name}");
				}
			}
		}

		public override string ToString()
		{
			return $"{AssetInfo.PathID.ToString("x16")} {UnityType}";
		}
		
		public ClassIDType UnityType => AssetInfo.ClassMap.IDType;

		public AssetInfo AssetInfo { get; }
		public Object Object { get; private set; }

		private readonly long m_offset;
		private readonly long m_length;
	}
}
