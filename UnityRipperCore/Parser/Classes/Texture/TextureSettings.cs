using System;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.Textures
{
	public class TextureSettings : IStreamReadable
	{
		public TextureSettings(IAssetsFile assetsFile)
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
				WrapMode = (TextureWrapMode)stream.ReadInt32();
			}
			else
			{
				WrapU = (TextureWrapMode)stream.ReadInt32();
				WrapV = (TextureWrapMode)stream.ReadInt32();
				WrapW = (TextureWrapMode)stream.ReadInt32();
			}
		}

		public FilterMode FilterMode { get; private set; }
		public int Aniso { get; private set; }
		public float MipBias { get; private set; }
		public TextureWrapMode WrapMode { get; private set; }
		public TextureWrapMode WrapU { get; private set; }
		public TextureWrapMode WrapV { get; private set; }
		public TextureWrapMode WrapW { get; private set; }
		
		/// <summary>
		/// Less than 2017.1
		/// </summary>
		public bool IsReadWrapMode => Version.IsLess(2017);

		private Version Version => m_assetsFile.Version;

		private readonly IAssetsFile m_assetsFile;
	}
}
