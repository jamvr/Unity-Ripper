using System.Collections.Generic;

namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedTagMap : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			m_tags.Read(stream);
		}

		public IReadOnlyDictionary<string, string> Tags => m_tags;

		private readonly Dictionary<string, string> m_tags = new Dictionary<string, string>();
	}
}
