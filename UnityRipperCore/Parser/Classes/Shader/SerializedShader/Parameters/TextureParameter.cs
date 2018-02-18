using System;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.Shaders
{
	public sealed class TextureParameter : IStreamReadable
	{
		public TextureParameter(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new Exception(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			NameIndex = stream.ReadInt32();
			Index = stream.ReadInt32();
			SamplerIndex = stream.ReadInt32();

			if(IsReadMultiSampled)
			{
				MultiSampled = stream.ReadBoolean();
			}
			Dim = stream.ReadByte();
			stream.AlignStream(AlignType.Align4);
		}

		public int NameIndex { get; private set; }
		public int Index { get; private set; }
		public int SamplerIndex { get; private set; }
		public bool MultiSampled { get; private set; }
		public byte Dim { get; private set; }

		/// <summary>
		/// 2017.3 and greater
		/// </summary>
		public bool IsReadMultiSampled => Version.IsGreaterEqual(2017, 3);

		private Version Version => m_assetsFile.Version;

		private readonly IAssetsFile m_assetsFile;
	}
}
