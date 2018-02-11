using System;
using System.Collections.Generic;
using System.IO;
using UnityRipper.Classes.AnimationClips.Editor;

namespace UnityRipper.Classes.AnimationClips
{
	public sealed class StreamedClip : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			m_data = stream.ReadUInt32Array();
			CurveCount = stream.ReadUInt32();
		}

		public IReadOnlyList<StreamedFrame> GenerateFrames()
		{
			List<StreamedFrame> frames = new List<StreamedFrame>();
			byte[] memStreamBuffer = new byte[m_data.Length * sizeof(uint)];
			Buffer.BlockCopy(m_data, 0, memStreamBuffer, 0, memStreamBuffer.Length);
			using (MemoryStream memStream = new MemoryStream(memStreamBuffer))
			{
				using (EndianStream stream = new EndianStream(memStream, EndianType.LittleEndian))
				{
					while (stream.BaseStream.Position < stream.BaseStream.Length)
					{
						StreamedFrame frame = new StreamedFrame();
						frame.Read(stream);
						frames.Add(frame);
					}
				}
			}
			return frames;
		}

		public bool IsValid => Data.Count > 0;
		
		public IReadOnlyList<uint> Data => m_data;
		public uint CurveCount { get; private set; }

		private uint[] m_data;
	}
}
