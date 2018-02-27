using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public StringBuilder ToString(StringBuilder sb, SShader shader, ShaderType type)
		{
			if(SubPrograms.Count > 0)
			{
				sb.AppendIntent(3).Append("Program").Append(' ');
				sb.Append('"').Append(type.ToProgramTypeString()).Append('"');
				sb.Append(' ').Append('{').Append('\n');
				foreach (SerializedSubProgram subProgram in SubPrograms)
				{
					GPUPlatform platform = subProgram.GpuProgramType.ToGPUPlatform();
					int index = shader.Platforms.IndexOf(platform);
					ShaderSubProgramBlob blob = shader.SubProgramBlobs[index];
					int count = SubPrograms.Where(t => t.GpuProgramType == subProgram.GpuProgramType).Select(t => t.ShaderHardwareTier).Distinct().Count();
					subProgram.ToString(sb, blob, count > 1);
				}
				sb.AppendIntent(3).Append('}').Append('\n');
			}
			return sb;
		}

		public IReadOnlyList<SerializedSubProgram> SubPrograms => m_subPrograms;

		private readonly IAssetsFile m_assetsFile;

		private SerializedSubProgram[] m_subPrograms;
	}
}
