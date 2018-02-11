using System.Collections.Generic;

namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedSubShader : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			m_passes = stream.ReadArray<SerializedPass>();
			Tags.Read(stream);
			LOD = stream.ReadInt32();
		}

		public IReadOnlyList<SerializedPass> Passes => m_passes;
		public SerializedTagMap Tags { get; } = new SerializedTagMap();
		public int LOD { get; private set; }

		private SerializedPass[] m_passes;
	}
}
