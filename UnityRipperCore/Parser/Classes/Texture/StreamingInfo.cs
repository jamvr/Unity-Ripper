namespace UnityRipper.Classes.Textures
{
	public class StreamingInfo : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			Offset = stream.ReadUInt32();
			Size = stream.ReadUInt32();
			Path = stream.ReadStringAligned();
		}

		public uint Offset { get; private set; }
		public uint Size { get; private set; }
		public string Path { get; private set; }
	}
}
