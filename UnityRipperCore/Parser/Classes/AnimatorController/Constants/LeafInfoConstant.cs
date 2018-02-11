namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class LeafInfoConstant : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			m_IDArray = stream.ReadUInt32Array();
			IndexOffset = stream.ReadUInt32();
		}

		public uint[] IDArray => m_IDArray;
		public uint IndexOffset { get; private set; }
		
		private uint[] m_IDArray;
	}
}
