using System.Collections.Generic;
using System.Text;

namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedTagMap : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			m_tags.Read(stream);
		}

		public StringBuilder ToString(StringBuilder sb, int intent)
		{
			if(Tags.Count != 0)
			{
				sb.AppendIntent(intent).Append("Tags").Append(' ').Append('{').Append(' ');
				foreach(var kvp in Tags)
				{
					sb.Append('"').Append(kvp.Key).Append('"').Append(' ').Append('=').Append(' ');
					sb.Append('"').Append(kvp.Value).Append('"').Append(' ');
				}
				sb.Append('}').Append('\n');
			}
			return sb;
		}

		public IReadOnlyDictionary<string, string> Tags => m_tags;

		private readonly Dictionary<string, string> m_tags = new Dictionary<string, string>();
	}
}
