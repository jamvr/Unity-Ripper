﻿namespace UnityRipper.Classes.Textures
{
	/// <summary>
	/// TextureDimension enum as it present in Unity 2017.3
	/// </summary>
	public enum TextureDimension
	{
		/// <summary>
		/// Texture type is not initialized or unknown.
		/// </summary>
		Unknown = -1,
		/// <summary>
		/// No texture is assigned.
		/// </summary>
		None = 0,
		/// <summary>
		/// Any texture type.
		/// </summary>
		Any = 1,
		/// <summary>
		/// 2D texture (Texture2D).
		/// </summary>
		Tex2D = 2,
		/// <summary>
		/// 3D volume texture (Texture3D).
		/// </summary>
		Tex3D = 3,
		/// <summary>
		/// Cubemap texture.
		/// </summary>
		Cube = 4,
		/// <summary>
		/// 2D array texture (Texture2DArray).
		/// </summary>
		Tex2DArray = 5,
		/// <summary>
		/// Cubemap array texture (CubemapArray).
		/// </summary>
		CubeArray = 6
	}
}
