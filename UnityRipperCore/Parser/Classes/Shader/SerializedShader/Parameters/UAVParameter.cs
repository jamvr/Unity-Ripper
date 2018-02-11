namespace UnityRipper.Classes.Shaders
{
	public sealed class UAVParameter : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			NameIndex = stream.ReadInt32();
			Index = stream.ReadInt32();
			OriginalIndex = stream.ReadInt32();
		}

		public int NameIndex { get; private set; }
		public int Index { get; private set; }
		public int OriginalIndex { get; private set; }
	}
}
