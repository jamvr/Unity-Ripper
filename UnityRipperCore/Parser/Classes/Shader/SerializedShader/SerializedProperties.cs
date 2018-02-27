using System.Collections.Generic;
using System.Text;

namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedProperties : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			m_props = stream.ReadArray<SerializedProperty>();
		}

		public StringBuilder ToString(StringBuilder sb)
		{
			sb.Append('\t').Append("Properties").Append(' ').Append('{').Append('\n');
			foreach(SerializedProperty prop in Props)
			{
				prop.ToString(sb);
			}
			sb.Append('\t').Append('}').Append('\n');
			return sb;
		}

		public IReadOnlyList<SerializedProperty> Props => m_props;

		private SerializedProperty[] m_props;
	}
}
