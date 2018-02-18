namespace UnityRipper.Converter.Textures.KTX
{
	public struct KTXConvertParameters
	{
		public int DataLength { get; set; }
		public KTXInternalFormat InternalFormat { get; set; }
		public KTXBaseInternalFormat BaseInternalFormat { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
	}
}
