using System.Collections.Generic;

namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedProperties : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			m_props = stream.ReadArray<SerializedProperty>();
		}

		public IReadOnlyList<SerializedProperty> Props => m_props;

		private SerializedProperty[] m_props;
	}
}
