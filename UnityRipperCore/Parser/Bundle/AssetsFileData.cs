using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnityRipper.Bundles
{
	internal class AssetsFileData : IDisposable
	{
		public AssetsFileData(Stream stream, bool isClosable)
		{
			m_stream = stream;
			m_isClosable = isClosable;
		}

		public void AddEntry(string name, long offset, long length)
		{
			AssetsFileEntry entry = new AssetsFileEntry(m_stream, name, offset, length);
			m_entries.Add(entry);
		}

		public void Dispose()
		{
			if(m_isClosable)
			{
				m_stream.Dispose();
				m_isClosable = false;
			}
			m_stream = null;
		}

		public IEnumerable<AssetsFileEntry> AssetsEntries => m_entries.Where(t => t.IsAssetsFile);
		public IEnumerable<AssetsFileEntry> ResourceEntries => m_entries.Where(t => !t.IsAssetsFile);

		private readonly List<AssetsFileEntry> m_entries = new List<AssetsFileEntry>();

		private Stream m_stream;
		private bool m_isClosable = false;
	}
}
