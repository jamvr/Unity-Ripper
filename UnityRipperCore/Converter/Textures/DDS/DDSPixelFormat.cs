using System.IO;

namespace UnityRipper.Converter.Textures.DDS
{
	public struct DDSPixelFormat
	{
		public void Write(BinaryWriter writer)
		{
			writer.Write(StructSize);
			writer.Write((uint)Flags);
			writer.Write(FourCC);
			writer.Write(RGBBitCount);
			writer.Write(RBitMask);
			writer.Write(GBitMask);
			writer.Write(BBitMask);
			writer.Write(ABitMask);
		}
		
		public DDPFFlags Flags { get; set; }
		public uint FourCC { get; set; }
		public uint RGBBitCount { get; set; }
		public uint RBitMask { get; set; }
		public uint GBitMask { get; set; }
		public uint BBitMask { get; set; }
		public uint ABitMask { get; set; }

		private const uint StructSize = 0x20;
	}
}
