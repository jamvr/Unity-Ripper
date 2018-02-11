using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class HumanPoseMask : IStreamReadable
	{
		public HumanPoseMask(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;

			m_word = new uint[IsReadThirdWord ? 3 : 2];
		}

		public void Read(EndianStream stream)
		{
			stream.Read(m_word, 0, m_word.Length);
		}
		
		public IReadOnlyList<uint> Word => m_word;

		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		public bool IsReadThirdWord => Version.IsGreaterEqual(5);

		private Version Version => m_assetsFile.Version;

		private readonly IAssetsFile m_assetsFile;

		private uint[] m_word = new uint[3];
	}
}
