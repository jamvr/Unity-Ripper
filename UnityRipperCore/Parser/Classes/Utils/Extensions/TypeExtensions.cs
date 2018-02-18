using System;

namespace UnityRipper.Classes
{
	public static class TypeExtensions
	{
		public static ClassIDType ToUnityClassType(this Type _this)
		{
			if (Enum.TryParse(_this.Name, out ClassIDType classID))
			{
				switch(classID)
				{
					case ClassIDType.Object:
					case ClassIDType.GameObject:
					case ClassIDType.Component:
					case ClassIDType.Transform:
					case ClassIDType.Material:
					case ClassIDType.Shader:
					case ClassIDType.Mesh:
					case ClassIDType.Texture:
					case ClassIDType.AnimationClip:
					case ClassIDType.Avatar:
					case ClassIDType.RuntimeAnimatorController:
					case ClassIDType.MonoBehaviour:
					case ClassIDType.MonoScript:
						return classID;

					default:
						throw new Exception($"Unsupported  type {_this}");
				}
			}

			throw new Exception($"{_this} is not Unity class type");
		}
	}
}
