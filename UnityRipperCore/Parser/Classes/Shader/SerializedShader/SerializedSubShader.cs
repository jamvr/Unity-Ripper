using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedSubShader : IStreamReadable
	{
		public SerializedSubShader(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new Exception(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			m_passes = stream.ReadArray(() => new SerializedPass(m_assetsFile));
			Tags.Read(stream);
			LOD = stream.ReadInt32();
		}

		public IReadOnlyList<SerializedPass> Passes => m_passes;
		public SerializedTagMap Tags { get; } = new SerializedTagMap();
		public int LOD { get; private set; }

		private readonly IAssetsFile m_assetsFile;

		private SerializedPass[] m_passes;
	}
}
