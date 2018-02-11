using System;
using System.Collections.Generic;

namespace UnityRipper.Classes.Shaders
{
	public sealed class ShaderSubProgramBlob : IStreamReadable
	{
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
				ShaderSubProgram subProgram = new ShaderSubProgram();
				subProgram.Read(stream);
				if (stream.BaseStream.Position != dataPosition + length)
				{
					throw new Exception($"Read less {stream.BaseStream.Position - dataPosition} than expected {length}");
				}
				SubPrograms[i] = subProgram;
			}
		}

		public ShaderSubProgram[] SubPrograms => m_subPrograms;

		private ShaderSubProgram[] m_subPrograms = new ShaderSubProgram[0];
	}
}
