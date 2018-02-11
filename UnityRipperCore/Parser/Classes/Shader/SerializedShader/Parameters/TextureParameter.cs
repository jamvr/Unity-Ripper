namespace UnityRipper.Classes.Shaders
{
	public sealed class TextureParameter : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			NameIndex = stream.ReadInt32();
			Index = stream.ReadInt32();
			SamplerIndex = stream.ReadInt32();
			Dim = stream.ReadByte();
			stream.AlignStream(AlignType.Align4);
		}

		public int NameIndex { get; private set; }
		public int Index { get; private set; }
		public int SamplerIndex { get; private set; }
		public byte Dim { get; private set; }
	}
}
