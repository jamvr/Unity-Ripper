using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedProgram : IStreamReadable
	{
		public SerializedProgram(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new Exception(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			m_subPrograms = stream.ReadArray(() => new SerializedSubProgram(m_assetsFile));
		}

		public IReadOnlyList<SerializedSubProgram> SubPrograms => m_subPrograms;

		private readonly IAssetsFile m_assetsFile;

		private SerializedSubProgram[] m_subPrograms;
	}
}
