#warning delete
/*using System;

namespace UnityRipper.AssetsFiles
{
	public static class UnityClassTypeExtention
	{
		public static AssetType ToAssetType(this UnityClassType _this)
		{
			switch (_this)
			{
				case UnityClassType.Object:
					throw new NotSupportedException();

				case UnityClassType.Material:
				case UnityClassType.Mesh:
				case UnityClassType.Shader:
				case UnityClassType.AnimationClip:
					return AssetType.Serialized;

				case UnityClassType.Texture:
				case UnityClassType.MonoScript:
					return AssetType.Meta;

				case UnityClassType.GameObject:
				case UnityClassType.Component:
				case UnityClassType.Transform:
				case UnityClassType.MeshRenderer:
				case UnityClassType.Renderer:
				case UnityClassType.MeshFilter:
				case UnityClassType.NamedObject:
				case UnityClassType.SkinnedMeshRenderer:
				case UnityClassType.RectTransform:
					throw new NotSupportedException();

				default:
					throw new NotImplementedException();
			}
		}
	}
}
*/