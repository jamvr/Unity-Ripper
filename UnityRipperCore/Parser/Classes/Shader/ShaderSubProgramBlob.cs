using System;
using System.Collections.Generic;
using System.Text;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.Shaders
{
	public sealed class ShaderSubProgramBlob : IStreamReadable
	{
		public ShaderSubProgramBlob(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			long startPosition = stream.BaseStream.Position;
			int count = stream.ReadInt32();
			long headerPosition = stream.BaseStream.Position;

			m_subPrograms = new ShaderSubProgram[count];
			for (int i = 0; i < count; i++)
			{
				stream.BaseStream.Position = headerPosition + i * 8;
				int offset = stream.ReadInt32();
				int length = stream.ReadInt32();
				
				long dataPosition = startPosition + offset;
				stream.BaseStream.Position = dataPosition;
				ShaderSubProgram subProgram = new ShaderSubProgram(m_assetsFile);
				subProgram.Read(stream);
				if (stream.BaseStream.Position != dataPosition + length)
				{
					throw new Exception($"Read less {stream.BaseStream.Position - dataPosition} than expected {length}");
				}
				m_subPrograms[i] = subProgram;
			}
		}

		public StringBuilder ToString(StringBuilder sb, string header)
		{
			int j = 0;
			while (true)
			{
				int index = header.IndexOf(GpuProgramIndexName, j);
				if (index == -1)
				{
					break;
				}
				
				int length = index - j;
				sb.Append(header, j, length);
				j += length + GpuProgramIndexName.Length + 1;

				int subProgram = -1;
				for(int startIndex = j; j < header.Length; j++)
				{
					if(!Char.IsDigit(header[j]))
					{
						string numberStr = header.Substring(startIndex, j - startIndex);
						subProgram = int.Parse(numberStr);
						break;
					}
				}

				SubPrograms[subProgram].ToString(sb);
			}
			sb.Append(header, j, header.Length - j);
			return sb;
		}

		public IReadOnlyList<ShaderSubProgram> SubPrograms => m_subPrograms;
		public ShaderGpuProgramType Type => SubPrograms[0].ProgramType;
		
		private const string GpuProgramIndexName = "GpuProgramIndex";

		private readonly IAssetsFile m_assetsFile;

		private ShaderSubProgram[] m_subPrograms = new ShaderSubProgram[0];
	}
}
