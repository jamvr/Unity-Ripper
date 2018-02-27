namespace UnityRipper.Classes.Shaders
{
	public sealed class UAVParameter : IStreamReadable
	{
		public UAVParameter()
		{
		}

		public UAVParameter(string name, int index, int originalIndex)
		{
			Name = name;
			NameIndex = -1;
			Index = index;
			OriginalIndex = originalIndex;
		}

		public void Read(EndianStream stream)
		{
			NameIndex = stream.ReadInt32();
			Index = stream.ReadInt32();
			OriginalIndex = stream.ReadInt32();
		}

		public string Name { get; private set; }
		public int NameIndex { get; private set; }
		public int Index { get; private set; }
		public int OriginalIndex { get; private set; }
	}
}
