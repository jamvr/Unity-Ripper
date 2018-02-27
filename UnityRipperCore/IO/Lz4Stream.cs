using System;
using System.IO;

namespace UnityRipper
{
	public class Lz4Stream : Stream
	{
		private enum DecodePhase
		{
			ReadToken,
			ReadExLiteralLength,
			CopyLiteral,
			ReadOffset,
			ReadExMatchLength,
			CopyMatch,

			Finish,
		}

		/// <summary>
		/// Whole base stream is compressed data
		/// </summary>
		/// <param name="baseStream">Stream with compressed data</param>
		public Lz4Stream(Stream baseStream) :
			this(baseStream, (int)(baseStream.Length - baseStream.Position))
		{
			long size = baseStream.Length - baseStream.Position;
			if (size > int.MaxValue)
			{
				throw new Exception($"Compressed size {size} is too big");
			}
		}

		/// <summary>
		/// Part of base stream is compressed data
		/// </summary>
		/// <param name="baseStream">Stream with compressed data</param>
		/// <param name="compressSize">Amount of comprassed data</param>
		public Lz4Stream(Stream baseStream, int compressSize)
		{
			if(baseStream == null)
			{
				throw new ArgumentNullException("Base stream is null");
			}
			if(compressSize <= 0)
			{
				throw new ArgumentException($"Compress length {compressSize} must be greater then 0");
			}

			BaseStream = baseStream;
			m_inputLength = compressSize;
			
			m_phase = DecodePhase.ReadToken;
			
			m_inputBufferPosition = 0;
			m_inputBufferEnd = 0;
			m_decodeBufferPos = 0;

			m_literalLength = 0;
			m_matchLength = 0;
			m_matchDestination = 0;
		}

		public override void Flush()
		{
		}

		public override void Close()
		{
			BaseStream = null;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}
		
		public override int Read(byte[] buffer, int offset, int count)
		{
			if(buffer == null)
			{
				throw new ArgumentNullException("Buffer is null");
			}
			if(offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentException($"Incorrect offset {offset} for buffer length {buffer.Length}");
			}
			if(offset + count > buffer.Length)
			{
				throw new ArgumentException($"Incorrect count {count} for buffer length {buffer.Length} and offset {offset}");
			}
			
			int readLeft = count;
			bool processing = true;
			while (processing)
			{
				switch (m_phase)
				{
					case DecodePhase.ReadToken:
						ReadToken();
						break;

					case DecodePhase.ReadExLiteralLength:
						ReadExLiteralLength();
						break;

					case DecodePhase.CopyLiteral:
						CopyLiteral(buffer, ref offset, ref readLeft);
						break;

					case DecodePhase.ReadOffset:
						ReadOffset();
						break;

					case DecodePhase.ReadExMatchLength:
						ReadExMatchLength();
						break;

					case DecodePhase.CopyMatch:
						CopyMatch(buffer, ref offset, ref readLeft, count);
						break;

					case DecodePhase.Finish:
						Finish(buffer, offset, readLeft, count);
						processing = false;
						break;

					default:
						throw new Exception($"Unknonw decode phase {m_phase}");
				}
			}
			
			return count - readLeft;
		}

		public int Read(MemoryStream memStream, int length)
		{
			if(memStream.Position > int.MaxValue)
			{
				throw new Exception($"Memory stream position {memStream.Position} is too big");
			}
			memStream.SetLength(memStream.Position + length);
			byte[] buffer = memStream.GetBuffer();
			int read = Read(buffer, (int)memStream.Position, length);
			memStream.Position += read;
			if(read != length)
			{
				throw new Exception($"Read {read} less then expected {length}");
			}
			return read;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}
		
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		private void ReadToken()
		{
			int token = ReadInputByte();

			m_literalLength = token >> 4;
			m_matchLength = (token & 0xF) + 4;

			switch(m_literalLength)
			{
				case 0:
					m_phase = DecodePhase.ReadOffset;
					break;

				case 0xF:
					m_phase = DecodePhase.ReadExLiteralLength;
					break;

				default:
					m_phase = DecodePhase.CopyLiteral;
					break;
			}
		}

		private void ReadExLiteralLength()
		{
			int exLiteralLength = ReadInputByte();
			m_literalLength += exLiteralLength;
			if(exLiteralLength == byte.MaxValue)
			{
				m_phase = DecodePhase.ReadExLiteralLength;
			}
			else
			{
				m_phase = DecodePhase.CopyLiteral;
			}
		}

		private void CopyLiteral(byte[] buffer, ref int offset, ref int readLeft)
		{
			int readCount = m_literalLength < readLeft ? m_literalLength : readLeft;
			if(readCount != 0)
			{
				int read = ReadInput(buffer, offset, readCount);

				readLeft -= read;
				offset += read;

				m_literalLength -= read;
				if(m_literalLength != 0)
				{
					m_phase = DecodePhase.CopyLiteral;
					return;
				}
			}

			if (readLeft == 0)
			{
				m_phase = DecodePhase.Finish;
			}
			else
			{
				m_phase = DecodePhase.ReadOffset;
			}
		}

		private void ReadOffset()
		{
			m_matchDestination = ReadInputInt16();
			if(m_matchLength == 15 + 4)
			{
				m_phase = DecodePhase.ReadExMatchLength;
			}
			else
			{
				m_phase = DecodePhase.CopyMatch;
			}
		}

		private void ReadExMatchLength()
		{
			int exMatchLength = ReadInputByte();
			m_matchLength += exMatchLength;
			if(exMatchLength == byte.MaxValue)
			{
				m_phase = DecodePhase.ReadExMatchLength;
			}
			else
			{
				m_phase = DecodePhase.CopyMatch;
			}
		}

		private void CopyMatch(byte[] buffer, ref int offset, ref int readLeft, int count)
		{
			int readCount = m_matchLength < readLeft ? m_matchLength : readLeft;
			if (readCount != 0)
			{
				int read = count - readLeft;
				int decodeCount = m_matchDestination - read;
				if(decodeCount > 0)
				{
					//offset is fairly far back, we need to pull from the buffer
					int source = m_decodeBufferPos - decodeCount;
					if(source < 0)
					{
						source += DecodeBufferLength;
					}
					int destCount = decodeCount < readCount ? decodeCount : readCount;
					for(int i = 0; i < destCount; i++)
					{
						buffer[offset] = m_decodeBuffer[source & DecodeBufferMask];
						offset++;
						source++;
					}
				}
				else
				{
					decodeCount = 0;
				}

				int sourceOffset = offset - m_matchDestination;
				for(int i = decodeCount; i < readCount; i ++)
				{
					buffer[offset] = buffer[sourceOffset];
					offset++;
					sourceOffset++;
				}

				readLeft -= readCount;
				m_matchLength -= readCount;
			}

			if (readLeft == 0)
			{
				m_phase = DecodePhase.Finish;
			}
			else
			{
				m_phase = DecodePhase.ReadToken;
			}
		}

		private void Finish(byte[] buffer, int offset, int readLeft, int count)
		{
			int read = count - readLeft;
			int toBuffer = read < DecodeBufferLength ? read : DecodeBufferLength;
			int repPos = offset - toBuffer;

			if(toBuffer == DecodeBufferLength)
			{
				Buffer.BlockCopy(buffer, repPos, m_decodeBuffer, 0, DecodeBufferLength);
				m_decodeBufferPos = 0;
			}
			else
			{
				int decodePosition = m_decodeBufferPos;
				for(int i = 0; i < toBuffer; i++)
				{
					m_decodeBuffer[decodePosition & DecodeBufferMask] = buffer[repPos];
					decodePosition++;
					repPos++;
				}

				m_decodeBufferPos = decodePosition & DecodeBufferMask;
			}
		}

		private int ReadInputByte()
		{
			if (m_inputBufferPosition == m_inputBufferEnd)
			{
				FillInputBuffer();
			}

			return m_inputBuffer[m_inputBufferPosition++];
		}

		private int ReadInputInt16()
		{
			if (m_inputBufferPosition == m_inputBufferEnd)
			{
				FillInputBuffer();
			}

			if(m_inputBufferEnd - m_inputBufferPosition == 1)
			{
				// read last byte and refill
				m_inputBuffer[0] = m_inputBuffer[m_inputBufferPosition];
				FillInputBuffer(1);
			}

			int ret = m_inputBuffer[m_inputBufferPosition++];
			ret |= m_inputBuffer[m_inputBufferPosition++] << 8;
			return ret;
		}

		private int ReadInput(byte[] buffer, int offset, int count)
		{
			int readLeft = count;
			int read = FillBuffer(buffer, offset, count);
			offset += read;
			readLeft -= read;

			if (readLeft != 0)
			{
				if (readLeft >= InputChunkSize)
				{
					int readCount = readLeft < m_inputLength ? readLeft : m_inputLength;
					read = BaseStream.Read(buffer, offset, readCount);
					if (read != readCount)
					{
						throw new Exception("Unable to read enough data");
					}
					readLeft -= read;
					m_inputLength -= read;
				}
				else
				{
					FillInputBuffer();
					read = FillBuffer(buffer, offset, readLeft);
					readLeft -= read;
				}
			}

			return count - readLeft;
		}

		private void FillInputBuffer(int offset = 0)
		{
			int count = InputChunkSize < m_inputLength ? InputChunkSize : m_inputLength;
			count -= offset;
			int read = BaseStream.Read(m_inputBuffer, offset, count);
			if (read == 0)
			{
#warning replace this place to m_phase = Finish for partial reading
				throw new Exception("No data left");
			}
			if(read != count)
			{
				throw new Exception("Unable to read enough data");
			}

			m_inputLength -= read;

			m_inputBufferPosition = 0;
			m_inputBufferEnd = read + offset;
		}

		private int FillBuffer(byte[] buffer, int offset, int count)
		{
			int inputBufferLength = m_inputBufferEnd - m_inputBufferPosition;
			int inputLeft = count < inputBufferLength ? count : inputBufferLength;
			for (int i = 0; i < inputLeft; i++)
			{
				buffer[offset] = m_inputBuffer[m_inputBufferPosition];
				offset++;
				m_inputBufferPosition++;
			}
			return inputLeft;
		}

		public override bool CanSeek => false;
		public override bool CanRead => true;
		public override bool CanWrite => false;

		public override long Length => throw new NotSupportedException();
		public override long Position
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

		private Stream BaseStream { get; set; }

		private const int InputChunkSize = 128;
		private const int DecodeBufferLength = 0x10000;
		private const int DecodeBufferMask = 0xFFFF;

		private readonly byte[] m_inputBuffer = new byte[InputChunkSize];
		private readonly byte[] m_decodeBuffer = new byte[DecodeBufferLength];
		
		private DecodePhase m_phase;
		private int m_inputLength = 0;
		private int m_inputBufferPosition = 0;
		private int m_inputBufferEnd = 0;
		private int m_decodeBufferPos;

		//state within interruptable phases and across phase boundaries is
		//kept here - again, so that we can punt out and restart freely
		private int m_literalLength;
		private int m_matchLength;
		private int m_matchDestination;
	}
}
