/*#define CHECK_ARGS
//#define CHECK_EOF
//#define LOCAL_SHADOW

using System;
using System.IO;

namespace Lz4
{
    public class Lz4DecoderStream : Stream
	{
		private enum DecodePhase
		{
			ReadToken,
			ReadExLiteralLength,
			CopyLiteral,
			ReadOffset,
			ReadExMatchLength,
			CopyMatch,
		}

		public Lz4DecoderStream(Stream input, long inputLength = long.MaxValue)
        {
            Reset(input, inputLength);
        }

        public void Reset(Stream input, long inputLength = long.MaxValue)
        {
            this.m_inputLength = inputLength;
            this.m_baseStream = input;

            m_phase = DecodePhase.ReadToken;

            m_decodeBufferPos = 0;

            m_literalLength = 0;
            m_matchLength = 0;
            m_matchDestination = 0;

            m_inputBufferPosition = DecBufLen;
            m_inputBufferEnd = DecBufLen;
        }

        public override void Close()
        {
            m_baseStream = null;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int readLeft = count;
            switch (m_phase)
            {
                case DecodePhase.ReadToken:
                    goto readToken;

                case DecodePhase.ReadExLiteralLength:
                    goto readExLiteralLength;

                case DecodePhase.CopyLiteral:
                    goto copyLiteral;

                case DecodePhase.ReadOffset:
                    goto readOffset;

                case DecodePhase.ReadExMatchLength:
                    goto readExMatchLength;

                case DecodePhase.CopyMatch:
                    goto copyMatch;
            }

            readToken:
            int token;
            if (m_inputBufferPosition < m_inputBufferEnd)
            {
                token = m_decodeBuffer[m_inputBufferPosition++];
            }
            else
            {
                token = ReadInputByte();
#if CHECK_EOF
                if (token == -1)
                    goto finish;
#endif
            }

            m_literalLength = token >> 4;
            m_matchLength = (token & 0xF) + 4;

            switch (m_literalLength)
            {
                case 0:
                    m_phase = DecodePhase.ReadOffset;
                    goto readOffset;

                case 0xF:
                    m_phase = DecodePhase.ReadExLiteralLength;
                    goto readExLiteralLength;

                default:
                    m_phase = DecodePhase.CopyLiteral;
                    goto copyLiteral;
            }

            readExLiteralLength:
            int exLiteralLen;
            if (m_inputBufferPosition < m_inputBufferEnd)
            {
                exLiteralLen = m_decodeBuffer[m_inputBufferPosition++];
            }
            else
            {
#if LOCAL_SHADOW
				this.inBufPos = inBufPos;
#endif
                exLiteralLen = ReadInputByte();
#if LOCAL_SHADOW
				inBufPos = this.inBufPos;
				inBufEnd = this.inBufEnd;
#endif

#if CHECK_EOF
                if (exLiteralLen == -1)
                    goto finish;
#endif
            }

            m_literalLength += exLiteralLen;
            if (exLiteralLen == 255)
                goto readExLiteralLength;

            m_phase = DecodePhase.CopyLiteral;
            goto copyLiteral;

            copyLiteral:
            int readCount = m_literalLength < readLeft ? m_literalLength : readLeft;
            if (readCount != 0)
            {
				int read;
                if (m_inputBufferPosition + readCount <= m_inputBufferEnd)
                {
                    for (int c = readCount; c-- != 0;)
                        buffer[offset++] = m_decodeBuffer[m_inputBufferPosition++];

					read = readCount;
                }
                else
                {
#if LOCAL_SHADOW
					this.inBufPos = inBufPos;
#endif
					read = ReadInput(buffer, offset, readCount);
#if LOCAL_SHADOW
					inBufPos = this.inBufPos;
					inBufEnd = this.inBufEnd;
#endif
#if CHECK_EOF
                    if (read == 0)
                        goto finish;
#endif
                }

                offset += read;
                readLeft -= read;

                m_literalLength -= read;

                if (m_literalLength != 0)
                    goto copyLiteral;
            }

            if (readLeft == 0)
                goto finish;

            m_phase = DecodePhase.ReadOffset;
            goto readOffset;

            readOffset:
            if (m_inputBufferPosition + 1 < m_inputBufferEnd)
            {
                m_matchDestination = (m_decodeBuffer[m_inputBufferPosition + 1] << 8) | m_decodeBuffer[m_inputBufferPosition];
                m_inputBufferPosition += 2;
            }
            else
            {
#if LOCAL_SHADOW
				this.inBufPos = inBufPos;
#endif
                m_matchDestination = ReadInputInt16();
#if LOCAL_SHADOW
				inBufPos = this.inBufPos;
				inBufEnd = this.inBufEnd;
#endif
#if CHECK_EOF
                if (m_matchDestination == -1)
                    goto finish;
#endif
            }

            if (m_matchLength == 15 + 4)
            {
                m_phase = DecodePhase.ReadExMatchLength;
                goto readExMatchLength;
            }
            else
            {
                m_phase = DecodePhase.CopyMatch;
                goto copyMatch;
            }

            readExMatchLength:
            int exMatLen;
            if (m_inputBufferPosition < m_inputBufferEnd)
            {
                exMatLen = m_decodeBuffer[m_inputBufferPosition++];
            }
            else
            {
#if LOCAL_SHADOW
				this.inBufPos = inBufPos;
#endif
                exMatLen = ReadInputByte();
#if LOCAL_SHADOW
				inBufPos = this.inBufPos;
				inBufEnd = this.inBufEnd;
#endif
#if CHECK_EOF
                if (exMatLen == -1)
                    goto finish;
#endif
            }

            m_matchLength += exMatLen;
            if (exMatLen == 255)
                goto readExMatchLength;

            m_phase = DecodePhase.CopyMatch;
            goto copyMatch;

            copyMatch:
			readCount = m_matchLength < readLeft ? m_matchLength : readLeft;
            if (readCount != 0)
            {
                int alreadyRead = count - readLeft;

                int decodeCount = m_matchDestination - alreadyRead;
                if (decodeCount > 0)
                {
                    //offset is fairly far back, we need to pull from the buffer

                    int source = m_decodeBufferPos - decodeCount;
                    if (source < 0)
                        source += DecBufLen;
                    int bufCnt = decodeCount < readCount ? decodeCount : readCount;

                    for (int c = bufCnt; c-- != 0;)
                        buffer[offset++] = m_decodeBuffer[source++ & DecBufMask];
                }
                else
                {
                    decodeCount = 0;
                }

                int sourceOffset = offset - m_matchDestination;
                for (int i = decodeCount; i < readCount; i++)
                    buffer[offset++] = buffer[sourceOffset++];

                readLeft -= readCount;
                m_matchLength -= readCount;
            }

            if (readLeft == 0)
                goto finish;

            m_phase = DecodePhase.ReadToken;
            goto readToken;

            finish:
			int resRead = count - readLeft;

            int nToBuf = resRead < DecBufLen ? resRead : DecBufLen;
            int repPos = offset - nToBuf;

            if (nToBuf == DecBufLen)
            {
				Buffer.BlockCopy(buffer, repPos, m_decodeBuffer, 0, DecBufLen);
                m_decodeBufferPos = 0;
            }
            else
            {
                int decPos = m_decodeBufferPos;

                while (nToBuf-- != 0)
                    m_decodeBuffer[decPos++ & DecBufMask] = buffer[repPos++];

                m_decodeBufferPos = decPos & DecBufMask;
            }

#if LOCAL_SHADOW
			this.phase = phase;
			this.inBufPos = inBufPos;
#endif
            return resRead;
        }

        private int ReadInputByte()
        {
            if (m_inputBufferPosition == m_inputBufferEnd)
            {
                int nRead = m_baseStream.Read(m_decodeBuffer, DecBufLen,
					InputChunkSize < m_inputLength ? InputChunkSize : (int)m_inputLength);

#if CHECK_EOF
                if (nRead == 0)
                    return -1;
#endif

                m_inputLength -= nRead;

                m_inputBufferPosition = DecBufLen;
                m_inputBufferEnd = DecBufLen + nRead;
            }

            return m_decodeBuffer[m_inputBufferPosition++];
        }

        private int ReadInputInt16()
        {
            if (m_inputBufferPosition == m_inputBufferEnd)
            {
                int nRead = m_baseStream.Read(m_decodeBuffer, DecBufLen,
					InputChunkSize < m_inputLength ? InputChunkSize : (int)m_inputLength);

#if CHECK_EOF
                if (nRead == 0)
                    return -1;
#endif

                m_inputLength -= nRead;

                m_inputBufferPosition = DecBufLen;
                m_inputBufferEnd = DecBufLen + nRead;
            }

            if (m_inputBufferEnd - m_inputBufferPosition == 1)
            {
                m_decodeBuffer[DecBufLen] = m_decodeBuffer[m_inputBufferPosition];

                int nRead = m_baseStream.Read(m_decodeBuffer, DecBufLen + 1,
					InputChunkSize - 1 < m_inputLength ? InputChunkSize - 1 : (int)m_inputLength);

#if CHECK_EOF
                if (nRead == 0)
                {
                    m_inputBufferPosition = DecBufLen;
                    m_inputBufferEnd = DecBufLen + 1;

                    return -1;
                }
#endif

                m_inputLength -= nRead;

                m_inputBufferPosition = DecBufLen;
                m_inputBufferEnd = DecBufLen + nRead + 1;
            }

            int ret = (m_decodeBuffer[m_inputBufferPosition + 1] << 8) | m_decodeBuffer[m_inputBufferPosition];
            m_inputBufferPosition += 2;

            return ret;
        }

        private int ReadInput(byte[] buffer, int offset, int count)
        {
            int readLeft = count;			
            int inputBufferLength = m_inputBufferEnd - m_inputBufferPosition;
            int inputLeft = readLeft < inputBufferLength ? readLeft : inputBufferLength;
            if (inputLeft != 0)
            {
                for (int c = inputLeft; c-- != 0;)
                    buffer[offset++] = m_decodeBuffer[m_inputBufferPosition++];
				
                readLeft -= inputLeft;
            }

            if (readLeft != 0)
            {
                int nRead;

                if (readLeft >= InputChunkSize)
                {
                    nRead = m_baseStream.Read(buffer, offset,
                        readLeft < m_inputLength ? readLeft : (int)m_inputLength);
                    readLeft -= nRead;
                }
                else
                {
                    nRead = m_baseStream.Read(m_decodeBuffer, DecBufLen,
                        InputChunkSize < m_inputLength ? InputChunkSize : (int)m_inputLength);

                    m_inputBufferPosition = DecBufLen;
                    m_inputBufferEnd = DecBufLen + nRead;

                    inputLeft = readLeft < nRead ? readLeft : nRead;
					
                    for (int c = inputLeft; c-- != 0;)
                        buffer[offset++] = m_decodeBuffer[m_inputBufferPosition++];
					
                    readLeft -= inputLeft;
                }

                m_inputLength -= nRead;
            }

            return count - readLeft;
        }

        #region Stream internals

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override void Flush()
        {
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

		#endregion
		
		//because we might not be able to match back across invocations,
		//we have to keep the last window's worth of bytes around for reuse
		//we use a circular buffer for this - every time we write into this
		//buffer, we also write the same into our output buffer

		private const int DecBufLen = 0x10000;
		private const int DecBufMask = 0xFFFF;

		private const int InputChunkSize = 128;
		
		private readonly byte[] m_decodeBuffer = new byte[DecBufLen + InputChunkSize];

		private long m_inputLength;
		private Stream m_baseStream;
		private int m_decodeBufferPos;
		private int m_inputBufferPosition;
		private int m_inputBufferEnd;

		//we keep track of which phase we're in so that we can jump right back
		//into the correct part of decoding
		private DecodePhase m_phase;

		//state within interruptable phases and across phase boundaries is
		//kept here - again, so that we can punt out and restart freely
		private int m_literalLength;
		private int m_matchLength;
		private int m_matchDestination;

	}
}
*/