namespace UnityRipper.Classes.Shaders
{
	public sealed class VectorParameter : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			NameIndex = stream.ReadInt32();
			Index = stream.ReadInt32();
			ArraySize = stream.ReadInt32();
			Type = stream.ReadByte();
			Dim = stream.ReadByte();
			stream.AlignStream(AlignType.Align4);
		}

		public int NameIndex { get; private set; }
		public int Index { get; private set; }
		public int ArraySize { get; private set; }
		public byte Type { get; private set; }
		public byte Dim { get; private set; }
	}
}
