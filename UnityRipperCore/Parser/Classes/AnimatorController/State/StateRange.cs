namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class StateRange : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			StartIndex = stream.ReadUInt32();
			Count = stream.ReadUInt32();
		}

		public uint StartIndex { get; private set; }
		public uint Count { get; private set; }
	}
}
