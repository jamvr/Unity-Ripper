using System.Collections.Generic;

namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class MotionNeighborList : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			m_neighborArray = stream.ReadUInt32Array();
		}

		public IReadOnlyList<uint> NeighborArray => m_neighborArray;

		private uint[] m_neighborArray;
	}
}
