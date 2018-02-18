namespace UnityRipper.Converter.Textures.DDS
{
	public struct DDSConvertParameters
	{
		public int DataLength { get; set; }
		public int MipMapCount { get; set; }
		public DDSDFlags MipMapFlag => MipMapCount == 1 ? 0 : DDSDFlags.DDSD_MIPMAPCOUNT;
		public int Width { get; set; }
		public int Height { get; set; }
		public bool IsPitchOrLinearSize { get; set; }
		public int PitchOrLinearSize => IsPitchOrLinearSize ? Height * Width / 2 : 0;
		public DDPFFlags PixelFormatFlags { get; set; }
		public uint FourCC { get; set; }
		public uint RGBBitCount { get; set; }
		public uint RBitMask { get; set; }
		public uint GBitMask { get; set; }
		public uint BBitMask { get; set; }
		public uint ABitMask { get; set; }
		public DDSCapsFlags Caps { get; set; }
	}
}
