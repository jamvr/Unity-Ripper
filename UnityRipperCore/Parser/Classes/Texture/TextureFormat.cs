using System;

namespace UnityRipper.Classes.Textures
{
	public enum TextureFormat
	{
		Alpha8				= 0x1,
		ARGB4444,
		RGB24,
		RGBA32,
		ARGB32,

		RGB565				= 0x7,

		R16					= 0x9,
		DXT1,

		DXT5				= 0xC,
		RGBA4444,
		BGRA32,
		RHalf,
		RGHalf,
		RGBAHalf,
		RFloat,
		RGFloat,
		RGBAFloat,
		YUY2,
		RGB9e5Float,

		BC6H				= 0x18,
		BC7,
		BC4,
		BC5,
		
		DXT1Crunched		= 0x1C,
		DXT5Crunched,
		PVRTC_RGB2,
		PVRTC_RGBA2,
		PVRTC_RGB4,
		PVRTC_RGBA4,
		ETC_RGB4,
		ATC_RGB4,
		ATC_RGBA8,

		EAC_R				= 0x29,
		EAC_R_SIGNED,
		EAC_RG,
		EAC_RG_SIGNED,
		ETC2_RGB,
		ETC2_RGBA1,
		ETC2_RGBA8,
		ASTC_RGB_4x4,
		ASTC_RGB_5x5,
		ASTC_RGB_6x6,
		ASTC_RGB_8x8,
		ASTC_RGB_10x10,
		ASTC_RGB_12x12,
		ASTC_RGBA_4x4,
		ASTC_RGBA_5x5,
		ASTC_RGBA_6x6,
		ASTC_RGBA_8x8,
		ASTC_RGBA_10x10,
		ASTC_RGBA_12x12,
		ETC_RGB4_3DS,
		ETC_RGBA8_3DS,
		RG16,
		R8,
		ETC_RGB4Crunched,
		ETC2_RGBA8Crunched,
	}

	public static class TextureFormatExtensions
	{
		public static ContainerType ToContainerType(this TextureFormat _this)
		{
			switch(_this)
			{
				case TextureFormat.RGB9e5Float:
				case TextureFormat.DXT1Crunched:
				case TextureFormat.DXT5Crunched:
				case TextureFormat.ETC_RGB4Crunched:
				case TextureFormat.ETC2_RGBA8Crunched:
					return ContainerType.None;

				case TextureFormat.Alpha8:
				case TextureFormat.ARGB4444:
				case TextureFormat.RGB24:
				case TextureFormat.RGBA32:
				case TextureFormat.ARGB32:
				case TextureFormat.RGB565:
				case TextureFormat.R16:
				case TextureFormat.DXT1:
				case TextureFormat.DXT5:
				case TextureFormat.RGBA4444:
				case TextureFormat.BGRA32:
				case TextureFormat.RG16:
				case TextureFormat.R8:
					return ContainerType.DDS;
					
                case TextureFormat.YUY2:
				case TextureFormat.PVRTC_RGB2:
				case TextureFormat.PVRTC_RGBA2:
				case TextureFormat.PVRTC_RGB4:
				case TextureFormat.PVRTC_RGBA4:
				case TextureFormat.ETC_RGB4:
				case TextureFormat.ETC2_RGB:
				case TextureFormat.ETC2_RGBA1:
				case TextureFormat.ETC2_RGBA8:
				case TextureFormat.ASTC_RGB_4x4:
				case TextureFormat.ASTC_RGB_5x5:
				case TextureFormat.ASTC_RGB_6x6:
				case TextureFormat.ASTC_RGB_8x8:
				case TextureFormat.ASTC_RGB_10x10:
				case TextureFormat.ASTC_RGB_12x12:
				case TextureFormat.ASTC_RGBA_4x4:
				case TextureFormat.ASTC_RGBA_5x5:
				case TextureFormat.ASTC_RGBA_6x6:
				case TextureFormat.ASTC_RGBA_8x8:
				case TextureFormat.ASTC_RGBA_10x10:
				case TextureFormat.ASTC_RGBA_12x12:
				case TextureFormat.ETC_RGB4_3DS:
				case TextureFormat.ETC_RGBA8_3DS:
					return ContainerType.PVR;
					
				case TextureFormat.RHalf:
				case TextureFormat.RGHalf:
				case TextureFormat.RGBAHalf:
				case TextureFormat.RFloat:
				case TextureFormat.RGFloat:
				case TextureFormat.RGBAFloat:
				case TextureFormat.BC4:
				case TextureFormat.BC5:
				case TextureFormat.BC6H:
				case TextureFormat.BC7:
				case TextureFormat.ATC_RGB4:
				case TextureFormat.ATC_RGBA8:
				case TextureFormat.EAC_R:
				case TextureFormat.EAC_R_SIGNED:
				case TextureFormat.EAC_RG:
				case TextureFormat.EAC_RG_SIGNED:
					return ContainerType.KTX;

				default:
					throw new NotSupportedException($"Unsupported texture format {_this}");
			}
		}
	}
}
