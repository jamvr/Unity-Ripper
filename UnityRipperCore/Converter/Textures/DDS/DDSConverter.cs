using System.IO;

namespace UnityRipper.Converter.Textures.DDS
{
	public static class DDSConverter
	{
		public static byte[] ConvertToDDS(byte[] data, int offset, DDSConvertParameters @params)
		{
			using (MemoryStream stream = new MemoryStream(data))
			{
				stream.Position = offset;
				return ConvertToDDS(stream, @params);
			}
		}

		public static byte[] ConvertToDDS(Stream stream, DDSConvertParameters @params)
		{
			byte[] buffer = new byte[4 + HeaderSize + @params.DataLength];
			using (MemoryStream memStream = new MemoryStream(buffer))
			{
				using (BinaryWriter writer = new BinaryWriter(memStream))
				{
					writer.Write(MagicNumber);
					writer.Write(HeaderSize);
					DDSDFlags flags = DDSDFlags.DDSD_CAPS | DDSDFlags.DDSD_HEIGHT | DDSDFlags.DDSD_WIDTH |
						DDSDFlags.DDSD_PIXELFORMAT | @params.MipMapFlag;
					writer.Write((uint)flags);
					writer.Write(@params.Height);
					writer.Write(@params.Width);
					writer.Write(@params.PitchOrLinearSize);
					writer.Write(Depth);
					writer.Write(@params.MipMapCount);
					for (int i = 0; i < 11; i++) // reserved
					{
						writer.Write(0);
					}
					DDSPixelFormat pixelFormat = new DDSPixelFormat()
					{
						Flags = @params.PixelFormatFlags,
						FourCC = @params.FourCC,
						RGBBitCount = @params.RGBBitCount,
						RBitMask = @params.RBitMask,
						GBitMask = @params.GBitMask,
						BBitMask = @params.BBitMask,
						ABitMask = @params.ABitMask,
					};
					pixelFormat.Write(writer);
					writer.Write((uint)@params.Caps);
					writer.Write((uint)Caps2);
					writer.Write(0); // caps3
					writer.Write(0); // caps4
					writer.Write(0); // reserved

					stream.CopyStream(writer.BaseStream, @params.DataLength);
				}
			}
			return buffer;
		}

		private const uint MagicNumber = 0x20534444;
		private const uint HeaderSize = 0x7C;

		private const int Depth = 0;
		private const DDSCaps2Flags Caps2 = 0;
	}
}
