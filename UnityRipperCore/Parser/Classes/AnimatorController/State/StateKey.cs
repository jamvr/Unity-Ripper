namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class StateKey : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			StateID = stream.ReadUInt32();
			LayerIndex = stream.ReadInt32();
		}

		public uint StateID { get; private set; }
		public int LayerIndex { get; private set; }
	}
}
