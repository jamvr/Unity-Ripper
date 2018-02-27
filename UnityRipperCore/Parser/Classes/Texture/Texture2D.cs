using System;
using System.Collections.Generic;
using UnityRipper.Classes.Textures;
using UnityRipper.Converter.Textures.DDS;
using UnityRipper.Converter.Textures.KTX;
using UnityRipper.Converter.Textures.PVR;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
#warning TODO: exclusive exporter (for meta)
	public class Texture2D : Texture
	{
		public Texture2D(AssetInfo assetInfo) :
			base(assetInfo)
		{
			TextureSettings = new TextureSettings(assetInfo.AssetFile);
			if(IsReadStreamData)
			{
				StreamData = new StreamingInfo();
			}
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			if(IsReadFallbackFormat)
			{
				ForcedFallbackFormat = stream.ReadInt32();
				DownscaleFallback = stream.ReadBoolean();
				stream.AlignStream(AlignType.Align4);
			}

			Width = stream.ReadInt32();
			Height = stream.ReadInt32();
			CompleteImageSize = stream.ReadInt32();
			TextureFormat = (TextureFormat)stream.ReadInt32();

			if(IsBoolMinMap)
			{
				MipMap = stream.ReadBoolean();
				if(MipMap)
				{
					int maxSide = Math.Max(Width, Height);
					MipCount = Convert.ToInt32(Math.Log(maxSide) / Math.Log(2));
				}
				else
				{
					MipCount = 1;
				}
			}
			else
			{
				MipCount = stream.ReadInt32();
			}

			if(IsReadIsReadable)
			{
				IsReadable = stream.ReadBoolean();
			}
			if(IsReadReadAllowed)
			{
				ReadAllowed = stream.ReadBoolean();
			}
			stream.AlignStream(AlignType.Align4);

			ImageCount = stream.ReadInt32();
			TextureDimension = stream.ReadInt32();
			TextureSettings.Read(stream);

			if(IsReadLightmapFormat)
			{
				LightmapFormat = stream.ReadInt32();
				if(IsReadColorSpace)
				{
					ColorSpace = stream.ReadInt32();
				}
			}

			m_imageData = stream.ReadByteArray();
			if(IsReadStreamData)
			{
				StreamData.Read(stream);
			}
		}

		public override byte[] ExportBinary()
		{
			byte[] data = m_imageData;
			int offset = 0;
			int length = data.Length;
			if(IsReadStreamData)
			{
				string path = StreamData.Path;
				if (path != string.Empty)
				{
					if(data.Length != 0)
					{
						throw new Exception("Texture contains data and resource path");
					}

					const string archivePrefix = "archive:/";
					if(path.StartsWith(archivePrefix))
					{
						path = path.Substring(archivePrefix.Length);
					}

					ResourcesFile res = AssetsFile.Collection.FindResourcesFile(AssetsFile, path);
					if(res == null)
					{
						Logger.Log(LogType.Warning, LogCategory.Export, $"Can't export '{Name}' because resources file '{path}' hasn't been found");
						return null;
					}
					data = res.Data;
					long longOffset = StreamData.Offset;
					long longSize = StreamData.Size;
					if(longOffset > int.MaxValue)
					{
						throw new Exception($"Unsupported offset value {longOffset}");
					}
					if (longSize > int.MaxValue)
					{
						throw new Exception($"Unsupported size value {longSize}");
					}
					offset = (int)longOffset;
					length = (int)longSize; 
				}
			}

			switch (TextureFormat.ToContainerType())
			{
				case ContainerType.None:
					return m_imageData;

				case ContainerType.DDS:
					return ExportDDS(data, offset, length);

				case ContainerType.PVR:
					return ExportPVR(data, offset, length);

				case ContainerType.KTX:
					return ExportKTX(data, offset, length);

				default:
					throw new NotSupportedException($"Unsupported texture container {TextureFormat.ToContainerType()}");
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			throw new NotSupportedException();
		}

		private byte[] ExportDDS(byte[] data, int offset, int length)
		{
			PrepareForDDSExport(ref data, ref offset, ref length);

			DDSConvertParameters @params = new DDSConvertParameters()
			{
				DataLength = length,
				MipMapCount = MipCount,
				Width = Width,
				Height = Height,
				IsPitchOrLinearSize = DDSIsPitchOrLinearSize,
				PixelFormatFlags = DDSPixelFormatFlags,
				FourCC = DDSFourCC,
				RGBBitCount = DDSRGBBitCount,
				RBitMask = DDSRBitMask,
				GBitMask = DDSGBitMask,
				BBitMask = DDSBBitMask,
				ABitMask = DDSABitMask,
				Caps = DDSCaps,
			};

			return DDSConverter.WrapToDDS(data, offset, @params);
		}

		private byte[] ExportPVR(byte[] data, int offset, int length)
		{
			PVRConvertParameters @params = new PVRConvertParameters()
			{
				DataLength = length,
				PixelFormat = PVRPixelFormat,
				Width = Width,
				Height = Height,
				MipMapCount = MipCount,
			};

			return PVRConverter.WraptToPVR(data, offset, @params);
		}

		private byte[] ExportKTX(byte[] data, int offset, int length)
		{
			KTXConvertParameters @params = new KTXConvertParameters()
			{
				DataLength = length,
				InternalFormat = KTXInternalFormat,
				BaseInternalFormat = KTXBaseInternalFormat,
				Width = Width,
				Height = Height,
			};

			return KTXConverter.WrapToKXT(data, offset, @params);
		}

		private void PrepareForDDSExport(ref byte[] data, ref int offset, ref int length)
		{
			if (IsSwapBytes)
			{
				byte[] swapedBytes = new byte[data.Length];
				for (int i = offset, j = offset + length - 1; j >= offset; i++, j--)
				{
					swapedBytes[j] = data[i];
				}
				data = swapedBytes;
			}

			switch (TextureFormat)
			{
				case TextureFormat.Alpha8:
					{
						byte[] bgra32 = new byte[length * 4];
						for(int i = offset, j = 0; i < offset + length;)
						{
							bgra32[j++] = 0xFF;
							bgra32[j++] = 0xFF;
							bgra32[j++] = 0xFF;
							bgra32[j++] = data[i++];
						}
						data = bgra32;
						offset = 0;
						length = bgra32.Length;
					}
					break;

				/*case TextureFormat.ARGB4444:
					{
						byte[] bgra32 = new byte[length * 2];
						for (int i = offset, j = 0; i < offset + length; i += 2)
						{
							int pixel = BitConverter.ToInt16(data, i);
							int c = 0x000F & pixel;
							bgra32[j++] = (byte)(c | c << 4);
							c = (0x00F0 & pixel) >> 4;
							bgra32[j++] = (byte)(c | c << 4);
							c = (0x0F00 & pixel) >> 8;
							bgra32[j++] = (byte)(c | c << 4);
							c = (0xF000 & pixel) >> 12;
							bgra32[j++] = (byte)(c | c << 4);
						}
						data = bgra32;
						offset = 0;
						length = bgra32.Length;
					}
					break;*/

				// we dont need swap colors in pixel because Unity read them in that order
				/*case TextureFormat.RGB24:
					{
						byte[] brg24 = new byte[length];
						for (int i = offset + 2, j = 0; i < offset + length; i += 6)
						{
							brg24[j++] = data[i--];
							brg24[j++] = data[i--];
							brg24[j++] = data[i--];
						}
						data = brg24;
						offset = 0;
					}
					break;*/

				case TextureFormat.RGBA32:
					{
						byte[] bgra32 = new byte[length];
						for (int i = offset, j = 0; i < offset + length; i += 4)
						{
							bgra32[j++] = data[i + 2];
							bgra32[j++] = data[i + 1];
							bgra32[j++] = data[i + 0];
							bgra32[j++] = data[i + 3];
						}
						data = bgra32;
						offset = 0;
						length = bgra32.Length;
					}
					break;

				case TextureFormat.ARGB32:
					{
						byte[] bgra32 = new byte[length];
						for (int i = offset, j = 0; i < offset + length; i += 4)
						{
							bgra32[j++] = data[i + 3];
							bgra32[j++] = data[i + 2];
							bgra32[j++] = data[i + 1];
							bgra32[j++] = data[i + 0];
						}
						data = bgra32;
						offset = 0;
						length = bgra32.Length;
					}
					break;

				case TextureFormat.R16:
					{
						byte[] bgra32 = new byte[length * 2];
						for (int i = offset, j = 2; i < offset + length; i += 2, j += 2)
						{
							float f = Half.ToHalf(data, i);
							bgra32[j++] = (byte)Math.Ceiling(f * 255.0); // R
							bgra32[j++] = 255; // A
						}
						data = bgra32;
						offset = 0;
						length = bgra32.Length;
					}
					break;

				case TextureFormat.RGBA4444:
					{
						byte[] argb16 = new byte[length];
						for (int i = offset, j = 0; i < offset + length; i += 2)
						{
							int pixel = BitConverter.ToUInt16(data, i);
							int c1 = (0x00F0 & pixel) >> 4; // B
							int c2 = (0x0F00 & pixel) >> 4; // G
							argb16[j++] = (byte)(c1 | c2);
							c1 = (0xF000 & pixel) >> 12; // R
							c2 = (0x000F & pixel) << 4; // A
							argb16[j++] = (byte)(c1 | c2);
						}
						data = argb16;
						offset = 0;
					}
					break;

				case TextureFormat.RG16:
					{
						byte[] bgra32 = new byte[length * 2];
						for (int i = offset, j = 1; i < offset + length; i += 4, j += 1)
						{
							bgra32[j++] = data[i + 1]; // G
							bgra32[j++] = data[i + 0]; // R
							bgra32[j++] = 255; // A
						}
						data = bgra32;
						offset = 0;
						length = bgra32.Length;
					}
					break;

				case TextureFormat.R8:
					{
						byte[] bgra32 = new byte[length * 4];
						for (int i = offset, j = 2; i < offset + length; i++, j += 2)
						{
							bgra32[j++] = data[i]; // R
							bgra32[j++] = 255; // A
						}
						data = bgra32;
						offset = 0;
						length = bgra32.Length;
					}
					break;
			}
		}
		
		public override string ExportExtension
		{
			get
			{
				if(Config.IsConvertTexturesToPNG)
				{
					return "png";
				}
				else
				{
					switch(TextureFormat.ToContainerType())
					{
						case ContainerType.None:
							switch(TextureFormat)
							{
								case TextureFormat.DXT1Crunched:
								case TextureFormat.DXT5Crunched:
								case TextureFormat.ETC_RGB4Crunched:
								case TextureFormat.ETC2_RGBA8Crunched:
									return "crn";
							}
							return "tex";

						case ContainerType.DDS:
							return "dds";

						case ContainerType.PVR:
							return "pvr";

						case ContainerType.KTX:
							return "ktx";

						default:
							throw new NotSupportedException($"Unsupported container type {TextureFormat.ToContainerType()}");
					}
				}
			}
		}
		
		private bool DDSIsPitchOrLinearSize
		{
			get
			{
				if (MipMap)
				{
					switch (TextureFormat)
					{
						case TextureFormat.DXT1:
						case TextureFormat.DXT1Crunched:
						case TextureFormat.DXT5:
						case TextureFormat.DXT5Crunched:
							return true;
					}
				}
				return false;
			}
		}

		private DDPFFlags DDSPixelFormatFlags
		{
			get
			{
				switch (TextureFormat)
				{
					case TextureFormat.Alpha8:
					case TextureFormat.ARGB4444:
					case TextureFormat.RGB24:
					case TextureFormat.RGBA32:
					case TextureFormat.ARGB32:
					case TextureFormat.R16:
					case TextureFormat.RGBA4444:
					case TextureFormat.BGRA32:
						return DDPFFlags.DDPF_RGB | DDPFFlags.DDPF_ALPHAPIXELS;

					case TextureFormat.RGB565:
						return DDPFFlags.DDPF_RGB;

					case TextureFormat.DXT1:
					case TextureFormat.DXT1Crunched:
					case TextureFormat.DXT5:
					case TextureFormat.DXT5Crunched:
						return DDPFFlags.DDPF_FOURCC;

					default:
						throw new NotSupportedException($"Texture format {TextureFormat} isn't supported");
				}
			}
		}

		private uint DDSFourCC
		{
			get
			{
				switch (TextureFormat)
				{
					case TextureFormat.DXT1:
					case TextureFormat.DXT1Crunched:
						// ASCII - 'DXT1'
						return 0x31545844;
					case TextureFormat.DXT5:
					case TextureFormat.DXT5Crunched:
						// ASCII - 'DXT5'
						return 0x35545844;

					default:
						return 0;
				}
			}
		}

		private uint DDSRGBBitCount
		{
			get
			{
				switch (TextureFormat)
				{
					case TextureFormat.ARGB4444:
						//return 32;
						return 16;

					case TextureFormat.RGBA4444:
						return 16;

					case TextureFormat.RGB24:
						//return 32;
						return 24;

					case TextureFormat.Alpha8:
					case TextureFormat.RGBA32:
					case TextureFormat.ARGB32:
					case TextureFormat.R16:
					case TextureFormat.BGRA32:
						return 32;

					case TextureFormat.RGB565:
						return 16;

					case TextureFormat.DXT1:
					case TextureFormat.DXT1Crunched:
					case TextureFormat.DXT5:
					case TextureFormat.DXT5Crunched:
						return 0;

					default:
						throw new NotSupportedException($"Texture format {TextureFormat} isn't supported");
				}
			}
		}

		private uint DDSRBitMask
		{
			get
			{
				switch (TextureFormat)
				{
					case TextureFormat.ARGB4444:
						//return 0xFF0000;
						return 0x0F00;

					case TextureFormat.RGBA4444:
						return 0x0F00;

					case TextureFormat.RGB24:
						return 0xFF0000;

					case TextureFormat.Alpha8:
					case TextureFormat.RGBA32:
					case TextureFormat.ARGB32:
					case TextureFormat.R16:
					case TextureFormat.BGRA32:
						return 0xFF0000;

					case TextureFormat.RGB565:
						return 0xF800;

					case TextureFormat.DXT1:
					case TextureFormat.DXT1Crunched:
					case TextureFormat.DXT5:
					case TextureFormat.DXT5Crunched:
						return 0;

					default:
						throw new NotSupportedException($"Texture format {TextureFormat} isn't supported");
				}
			}
		}

		private uint DDSGBitMask
		{
			get
			{
				switch (TextureFormat)
				{
					case TextureFormat.ARGB4444:
						//return 0xFF00;
						return 0xF0;

					case TextureFormat.RGBA4444:
						return 0xF0;

					case TextureFormat.RGB24:
						return 0xFF00;

					case TextureFormat.Alpha8:
					case TextureFormat.RGBA32:
					case TextureFormat.ARGB32:
					case TextureFormat.R16:
					case TextureFormat.BGRA32:
						return 0xFF00;

					case TextureFormat.RGB565:
						return 0x7E0;

					case TextureFormat.DXT1:
					case TextureFormat.DXT1Crunched:
					case TextureFormat.DXT5:
					case TextureFormat.DXT5Crunched:
						return 0;

					default:
						throw new NotSupportedException($"Texture format {TextureFormat} isn't supported");
				}
			}
		}

		private uint DDSBBitMask
		{
			get
			{
				switch (TextureFormat)
				{
					case TextureFormat.ARGB4444:
						//return 0xFF;
						return 0x0F;

					case TextureFormat.RGBA4444:
						return 0x0F;

					case TextureFormat.RGB24:
						return 0xFF;

					case TextureFormat.Alpha8:
					case TextureFormat.RGBA32:
					case TextureFormat.ARGB32:
					case TextureFormat.R16:
					case TextureFormat.BGRA32:
						return 0xFF;

					case TextureFormat.RGB565:
						return 0x1F;

					case TextureFormat.DXT1:
					case TextureFormat.DXT1Crunched:
					case TextureFormat.DXT5:
					case TextureFormat.DXT5Crunched:
						return 0;

					default:
						throw new NotSupportedException($"Texture format {TextureFormat} isn't supported");
				}
			}
		}

		private uint DDSABitMask
		{
			get
			{
				switch (TextureFormat)
				{
					case TextureFormat.ARGB4444:
						//return 0xFF000000;
						return 0xF000;

					case TextureFormat.RGBA4444:
						return 0xF000;

					case TextureFormat.RGB24:
						//return 0xFF000000;
						return 0x0;

					case TextureFormat.Alpha8:
					case TextureFormat.RGBA32:
					case TextureFormat.ARGB32:
					case TextureFormat.R16:
					case TextureFormat.BGRA32:
						return 0xFF000000;

					case TextureFormat.RGB565:
						return 0;

					case TextureFormat.DXT1:
					case TextureFormat.DXT1Crunched:
					case TextureFormat.DXT5:
					case TextureFormat.DXT5Crunched:
						return 0;

					default:
						throw new NotSupportedException($"Texture format {TextureFormat} isn't supported");
				}
			}
		}

		private DDSCapsFlags DDSCaps
		{
			get
			{
				if (IsBoolMinMap)
				{
					if (!MipMap)
					{
						return DDSCapsFlags.DDSCAPS_TEXTURE;
					}
				}
				return DDSCapsFlags.DDSCAPS_TEXTURE | DDSCapsFlags.DDSCAPS_MIPMAP | DDSCapsFlags.DDSCAPS_COMPLEX;
			}
		}

		private bool IsSwapBytes
		{
			get
			{
				if (Platform == Platform.XBox360)
				{
					switch (TextureFormat)
					{
						case TextureFormat.ARGB4444:
						case TextureFormat.RGB565:
						case TextureFormat.DXT1:
						case TextureFormat.DXT1Crunched:
						case TextureFormat.DXT5:
						case TextureFormat.DXT5Crunched:
							return true;
					}
				}
				return false;
			}
		}

		private KTXInternalFormat KTXInternalFormat
		{
			get
			{
				switch (TextureFormat)
				{
					case TextureFormat.RHalf:
						return KTXInternalFormat.R16F;

					case TextureFormat.RGHalf:
						return KTXInternalFormat.RG16F;

					case TextureFormat.RGBAHalf:
						return KTXInternalFormat.RGBA16F;

					case TextureFormat.RFloat:
						return KTXInternalFormat.R32F;

					case TextureFormat.RGFloat:
						return KTXInternalFormat.RG32F;

					case TextureFormat.RGBAFloat:
						return KTXInternalFormat.RGBA32F;

					case TextureFormat.BC4:
						return KTXInternalFormat.COMPRESSED_RED_RGTC1;

					case TextureFormat.BC5:
						return KTXInternalFormat.COMPRESSED_RG_RGTC2;

					case TextureFormat.BC6H:
						return KTXInternalFormat.COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT;

					case TextureFormat.BC7:
						return KTXInternalFormat.COMPRESSED_RGBA_BPTC_UNORM;

					case TextureFormat.PVRTC_RGB2:
						return KTXInternalFormat.COMPRESSED_RGB_PVRTC_2BPPV1_IMG;

					case TextureFormat.PVRTC_RGBA2:
						return KTXInternalFormat.COMPRESSED_RGBA_PVRTC_2BPPV1_IMG;

					case TextureFormat.PVRTC_RGB4:
						return KTXInternalFormat.COMPRESSED_RGB_PVRTC_4BPPV1_IMG;

					case TextureFormat.PVRTC_RGBA4:
						return KTXInternalFormat.COMPRESSED_RGBA_PVRTC_4BPPV1_IMG;

					case TextureFormat.ETC_RGB4Crunched:
					case TextureFormat.ETC_RGB4_3DS:
					case TextureFormat.ETC_RGB4:
						return KTXInternalFormat.ETC1_RGB8_OES;

					case TextureFormat.ATC_RGB4:
						return KTXInternalFormat.ATC_RGB_AMD;

					case TextureFormat.ATC_RGBA8:
						return KTXInternalFormat.ATC_RGBA_INTERPOLATED_ALPHA_AMD;

					case TextureFormat.EAC_R:
						return KTXInternalFormat.COMPRESSED_R11_EAC;

					case TextureFormat.EAC_R_SIGNED:
						return KTXInternalFormat.COMPRESSED_SIGNED_R11_EAC;

					case TextureFormat.EAC_RG:
						return KTXInternalFormat.COMPRESSED_RG11_EAC;

					case TextureFormat.EAC_RG_SIGNED:
						return KTXInternalFormat.COMPRESSED_SIGNED_RG11_EAC;

					case TextureFormat.ETC2_RGB:
						return KTXInternalFormat.COMPRESSED_RGB8_ETC2;

					case TextureFormat.ETC2_RGBA1:
						return KTXInternalFormat.COMPRESSED_RGB8_PUNCHTHROUGH_ALPHA1_ETC2;

					case TextureFormat.ETC2_RGBA8Crunched:
					case TextureFormat.ETC_RGBA8_3DS:
					case TextureFormat.ETC2_RGBA8:
						return KTXInternalFormat.COMPRESSED_RGBA8_ETC2_EAC;

					default:
						throw new NotSupportedException();
				}
			}
		}

		private KTXBaseInternalFormat KTXBaseInternalFormat
		{
			get
			{
				switch (TextureFormat)
				{
					case TextureFormat.RHalf:
					case TextureFormat.RFloat:
					case TextureFormat.BC4:
					case TextureFormat.EAC_R:
					case TextureFormat.EAC_R_SIGNED:
						return KTXBaseInternalFormat.RED;

					case TextureFormat.RGHalf:
					case TextureFormat.RGFloat:
					case TextureFormat.BC5:
					case TextureFormat.EAC_RG:
					case TextureFormat.EAC_RG_SIGNED:
						return KTXBaseInternalFormat.RG;

					case TextureFormat.BC6H:
					case TextureFormat.PVRTC_RGB2:
					case TextureFormat.PVRTC_RGB4:
					case TextureFormat.ETC_RGB4Crunched:
					case TextureFormat.ETC_RGB4_3DS:
					case TextureFormat.ETC_RGB4:
					case TextureFormat.ATC_RGB4:
					case TextureFormat.ETC2_RGB:
						return KTXBaseInternalFormat.RGB;

					case TextureFormat.RGBAHalf:
					case TextureFormat.RGBAFloat:
					case TextureFormat.BC7:
					case TextureFormat.PVRTC_RGBA2:
					case TextureFormat.PVRTC_RGBA4:
					case TextureFormat.ATC_RGBA8:
					case TextureFormat.ETC2_RGBA8Crunched:
					case TextureFormat.ETC_RGBA8_3DS:
					case TextureFormat.ETC2_RGBA8:
					case TextureFormat.ETC2_RGBA1:
						return KTXBaseInternalFormat.RGBA;

					default:
						throw new NotSupportedException();
				}
			}
		}

		private PVRPixelFormat PVRPixelFormat
		{
			get
			{
				switch (TextureFormat)
				{
					case TextureFormat.YUY2:
						return PVRPixelFormat.YUY2;

					case TextureFormat.PVRTC_RGB2:
						return PVRPixelFormat.PVRTC2bppRGB;

					case TextureFormat.PVRTC_RGBA2:
						return PVRPixelFormat.PVRTC2bppRGBA;

					case TextureFormat.PVRTC_RGB4:
						return PVRPixelFormat.PVRTC4bppRGB;

					case TextureFormat.PVRTC_RGBA4:
						return PVRPixelFormat.PVRTC4bppRGBA;

					case TextureFormat.ETC_RGB4Crunched:
					case TextureFormat.ETC_RGB4_3DS:
					case TextureFormat.ETC_RGB4:
						return PVRPixelFormat.ETC1;

					case TextureFormat.ETC2_RGB:
						return PVRPixelFormat.ETC2RGB;

					case TextureFormat.ETC2_RGBA1:
						return PVRPixelFormat.ETC2RGBA1;

					case TextureFormat.ETC2_RGBA8Crunched:
					case TextureFormat.ETC_RGBA8_3DS:
					case TextureFormat.ETC2_RGBA8:
						return PVRPixelFormat.ETC2RGBA;

					case TextureFormat.ASTC_RGB_4x4:
					case TextureFormat.ASTC_RGBA_4x4:
						return PVRPixelFormat.ASTC_4x4;

					case TextureFormat.ASTC_RGB_5x5:
					case TextureFormat.ASTC_RGBA_5x5:
						return PVRPixelFormat.ASTC_5x5;

					case TextureFormat.ASTC_RGB_6x6:
					case TextureFormat.ASTC_RGBA_6x6:
						return PVRPixelFormat.ASTC_6x6;

					case TextureFormat.ASTC_RGB_8x8:
					case TextureFormat.ASTC_RGBA_8x8:
						return PVRPixelFormat.ASTC_8x8;

					case TextureFormat.ASTC_RGB_10x10:
					case TextureFormat.ASTC_RGBA_10x10:
						return PVRPixelFormat.ASTC_10x10;

					case TextureFormat.ASTC_RGB_12x12:
					case TextureFormat.ASTC_RGBA_12x12:
						return PVRPixelFormat.ASTC_12x12;

					default:
						throw new NotSupportedException();
				}
			}
		}

		public int ForcedFallbackFormat { get; private set; }
		public bool DownscaleFallback { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int CompleteImageSize { get; private set; }
		public TextureFormat TextureFormat { get; private set; }
		public int MipCount { get; private set; }
		public bool MipMap { get; private set; }
		public bool IsReadable { get; private set; }
		public bool ReadAllowed { get; private set; }
		public int ImageCount { get; private set; }
		/// <summary>
		/// TextureDimension enum has beed changed at least one time
		/// so it's impossible to cast to enum directly
		/// </summary>
		public int TextureDimension { get; private set; }
		public TextureSettings TextureSettings { get; }
		public int LightmapFormat { get; private set; }
		public int ColorSpace { get; private set; }
		public IReadOnlyCollection<byte> ImageData => m_imageData;
		public StreamingInfo StreamData { get; }

		/// <summary>
		/// 2017.3 and greater
		/// </summary>
		public bool IsReadFallbackFormat => Version.IsGreaterEqual(2017, 3);
		/// <summary>
		/// Less than 5.2.0
		/// </summary>
		public bool IsBoolMinMap => Version.IsLess(5, 2);
		/// <summary>
		/// 2.6.0 and greater
		/// </summary>
		public bool IsReadIsReadable => Version.IsGreaterEqual(2, 6);
		/// <summary>
		/// From 3.0.0 to 5.5.0 exclusive
		/// </summary>
		public bool IsReadReadAllowed => Version.IsGreaterEqual(3) && Version.IsLess(5, 5);
		/// <summary>
		/// 3.0.0 and greater
		/// </summary>
		public bool IsReadLightmapFormat => Version.IsGreaterEqual(3);
		/// <summary>
		/// 3.5.0 and greater
		/// </summary>
		public bool IsReadColorSpace => Version.IsGreaterEqual(3, 5);
		/// <summary>
		/// 5.3.0 and greater
		/// </summary>
		public bool IsReadStreamData => Version.IsGreaterEqual(5, 3);

		private byte[] m_imageData;
	}
}
