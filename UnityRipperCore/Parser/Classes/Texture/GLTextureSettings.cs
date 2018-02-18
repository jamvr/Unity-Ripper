using System;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.Textures
{
	public class GLTextureSettings : IStreamReadable
	{
		public GLTextureSettings(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			FilterMode = (FilterMode)stream.ReadInt32();
			Aniso = stream.ReadInt32();
			MipBias = stream.ReadSingle();
			if (IsReadWrapMode)
			{
				WrapMode = (WrapMode)stream.ReadInt32();
			}
			else
			{
				WrapU = stream.ReadInt32();
				WrapV = stream.ReadInt32();
				WrapW = stream.ReadInt32();
			}
		}

		public FilterMode FilterMode { get; private set; }
		public int Aniso { get; private set; }
		public float MipBias { get; private set; }
		public WrapMode WrapMode { get; private set; }
		public int WrapU { get; private set; }
		public int WrapV { get; private set; }
		public int WrapW { get; private set; }
		
		/// <summary>
		/// Less than 2017.1
		/// </summary>
		public bool IsReadWrapMode => Version.IsLess(2017);

		private Version Version => m_assetsFile.Version;

		private readonly IAssetsFile m_assetsFile;
	}
}
