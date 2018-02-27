using System.IO;

namespace UnityRipper.Converter.Textures.KTX
{
	public static class KTXConverter
	{
		public static byte[] WrapToKXT(byte[] data, int offset, KTXConvertParameters @params)
		{
			using (MemoryStream stream = new MemoryStream(data))
			{
				stream.Position = offset;
				return WrapToKXT(stream, @params);
			}
		}

		public static byte[] WrapToKXT(Stream stream, KTXConvertParameters @params)
		{
			byte[] buffer = new byte[HeaderSize + @params.DataLength];
			using (MemoryStream memStream = new MemoryStream(buffer))
			{
				using (BinaryWriter writer = new BinaryWriter(memStream))
				{
					writer.Write(Identifier);
					writer.Write(EndianessLE);
					writer.Write((uint)Type);
					writer.Write(TypeSize);
					writer.Write((uint)Format);
					writer.Write((uint)@params.InternalFormat);
					writer.Write((uint)@params.BaseInternalFormat);
					writer.Write(@params.Width);
					writer.Write(@params.Height);
					writer.Write(PixelDepth);
					writer.Write(NumberOfArrayElements);
					writer.Write(NumberOfFaces);
					writer.Write(NumberOfMipmapLevels);
					writer.Write(BytesOfKeyValueData);
					writer.Write(@params.DataLength);

					stream.CopyStream(writer.BaseStream, @params.DataLength);
				}
			}
			return buffer;
		}

		private const int HeaderSize = 68;


		private static readonly byte[] Identifier = { 0xAB, 0x4B, 0x54, 0x58, 0x20, 0x31, 0x31, 0xBB, 0x0D, 0x0A, 0x1A, 0x0A };
		private const uint EndianessLE = 0x04030201;
		private const uint EndianessBE = 0x01020304;

		// for compressed - 0
		private const KTXType Type = 0;
		private const int TypeSize = 1;
		// for compressed - 0
		private const KTXFormat Format = 0;
		private const int PixelDepth = 0;
		private const int NumberOfArrayElements = 0;
		// number of cubemap faces
		private const int NumberOfFaces = 1;
		private const int NumberOfMipmapLevels = 1;
		private const int BytesOfKeyValueData = 0;

	}
}
