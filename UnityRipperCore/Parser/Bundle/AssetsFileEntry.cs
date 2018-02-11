using System;
using System.IO;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Bundles
{
	internal class AssetsFileEntry
	{
		public AssetsFileEntry(Stream stream, string name, long offset, long length)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}
			if (offset < 0)
			{
				throw new ArgumentException(nameof(offset));
			}
			if (length <= 0)
			{
				throw new ArgumentException(nameof(length));
			}

			m_stream = stream;
			Name = name;
			m_offset = offset;
			m_length = length;
		}

		public AssetsFile CreateAssetsFile(IAssetCollection collection)
		{
			AssetsFile assetsFile = new AssetsFile(collection, Name);
			m_stream.Position = m_offset;
			assetsFile.Parse(m_stream);
			long read = m_stream.Position - m_offset;
			if(read != m_length)
			{
				//throw new Exception($"Read {read} but expected {m_length}");
			}
			return assetsFile;
		}

		public override string ToString()
		{
			return Name;
		}
		
		public bool IsAssetsFile
		{
			get
			{
				string ext = Path.GetExtension(Name);
				if (ext == ManifestExtention)
				{
					return false;
				}
				if (ext == ResourceExtntion)
				{
					return false;
				}
				if (ext == ResExtntion)
				{
					return false;
				}
				return true;
			}
		}

		public string Name { get; }

		private readonly Stream m_stream;
		private readonly long m_offset;
		private readonly long m_length;

		private const string ManifestExtention = ".manifest";
		private const string ResourceExtntion = ".resource";
		private const string ResExtntion = ".resS";
	}
}
