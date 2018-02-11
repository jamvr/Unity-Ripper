using System.Collections.Generic;

namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedProgram : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			m_subPrograms = stream.ReadArray<SerializedSubProgram>();
		}

		public IReadOnlyList<SerializedSubProgram> SubPrograms => m_subPrograms;

		private SerializedSubProgram[] m_subPrograms;
	}
}
