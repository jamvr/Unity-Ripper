using System.IO;

namespace UnityRipper.Converter.Textures.PVR
{
#warning TODO: vertical flip?
	public static class PVRConverter
	{
		public static byte[] ConvertToPVR(byte[] data, int offset, PVRConvertParameters @params)
		{
			using (MemoryStream stream = new MemoryStream(data))
			{
				stream.Position = offset;
				return ConvertToPVR(stream, @params);
			}
		}

		public static byte[] ConvertToPVR(Stream stream, PVRConvertParameters @params)
		{
			byte[] buffer = new byte[HeaderSize + @params.DataLength];
			using (MemoryStream memStream = new MemoryStream(buffer))
			{
				using (BinaryWriter writer = new BinaryWriter(memStream))
				{
					writer.Write(Version);
					writer.Write((uint)Flags);
					writer.Write((ulong)@params.PixelFormat);
					writer.Write((uint)ColourSpace);
					writer.Write((uint)ChannelType);
					writer.Write(@params.Height);
					writer.Write(@params.Width);
					writer.Write(Depth);
					writer.Write(NumSurfaces);
					writer.Write(NumFaces);
					writer.Write(@params.MipMapCount);
					writer.Write(MetaDataSize);

					stream.CopyStream(writer.BaseStream, @params.DataLength);
				}
			}
			return buffer;
		}

		private const int HeaderSize = 52;

		private const int Version = 0x03525650;
		private const PVRFlag Flags = PVRFlag.NoFlag;
		private const PVRColourSpace ColourSpace = PVRColourSpace.LinearRGB;
		private const PVRChannelType ChannelType = PVRChannelType.UnsignedByteNormalised;
		private const int Depth = 1;
		// For texture arrays
		private const int NumSurfaces = 1;
		// For cube maps
		private const int NumFaces = 1;
		private const int MetaDataSize = 0;
	}
}
