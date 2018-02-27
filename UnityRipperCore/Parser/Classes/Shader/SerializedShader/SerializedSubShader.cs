using System;
using System.Collections.Generic;
using System.Text;
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

		public StringBuilder ToString(StringBuilder sb, SShader shader)
		{
			sb.AppendIntent(1).Append("Subshader").Append(' ').Append('{').Append('\n');
			if(LOD != 0)
			{
				sb.AppendIntent(2).Append("LOD").Append(' ').Append(LOD).Append('\n');
			}
			Tags.ToString(sb, 2);
			foreach(SerializedPass pass in Passes)
			{
				pass.ToString(sb, shader);
			}
			sb.AppendIntent(1).Append('}').Append('\n');
			return sb;
		}

		public IReadOnlyList<SerializedPass> Passes => m_passes;
		public SerializedTagMap Tags { get; } = new SerializedTagMap();
		public int LOD { get; private set; }

		private readonly IAssetsFile m_assetsFile;

		private SerializedPass[] m_passes;
	}
}
