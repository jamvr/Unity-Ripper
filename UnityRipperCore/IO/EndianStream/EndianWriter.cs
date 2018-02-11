/*using System;
using System.IO;
using System.Text;

namespace UnityRipper
{
	public class EndianWriter : BinaryWriter
	{
		public EndianWriter(Stream stream):
			this(stream, EndianType.LittleEndian)
		{
		}

		public EndianWriter(Stream stream, EndianType endType) :
		   base(stream, Encoding.Default, true)
		{
			EndianType = endType;
		}

		public override void Write(short value)
		{
			Write(unchecked((ushort)value));
		}

		public override void Write(ushort value)
		{
			if (EndianType == EndianType.BigEndian)
			{
				m_buffer[0] = (byte)(value >> 8);
				m_buffer[1] = (byte)(value);
				base.Write(m_buffer, 0, 2);
			}

			base.Write(value);
		}

		public override void Write(int value)
		{
			Write(unchecked((uint)value));
		}

		public override void Write(uint value)
		{
			if (EndianType == EndianType.BigEndian)
			{
				m_buffer[0] = (byte)(value >> 24);
				m_buffer[1] = (byte)(value >> 16);
				m_buffer[2] = (byte)(value >> 8);
				m_buffer[3] = (byte)(value);
				base.Write(m_buffer, 0, 2);
			}

			base.Write(value);
		}

		public override void Write(long value)
		{
			Write(unchecked((uint)value));
		}

		public override void Write(ulong value)
		{
			if (EndianType == EndianType.BigEndian)
			{
				m_buffer[0] = (byte)(value >> 56);
				m_buffer[1] = (byte)(value >> 48);
				m_buffer[2] = (byte)(value >> 40);
				m_buffer[3] = (byte)(value >> 32);
				m_buffer[4] = (byte)(value >> 24);
				m_buffer[5] = (byte)(value >> 16);
				m_buffer[6] = (byte)(value >> 8);
				m_buffer[7] = (byte)(value);
				base.Write(m_buffer, 0, 2);
			}

			base.Write(value);
		}

		public override void Write(float value)
		{
			Write(unchecked((uint)value));
		}

		public override void Write(double value)
		{
			Write(unchecked((ulong)value));
		}

		public override void Write(string value)
		{
			throw new NotSupportedException();
		}

		public override void WriteStringAligned(string value)
		{

		}

		public EndianType EndianType { get; set; }

		private const int StringBufferSize = 8096;
		
		private readonly byte[] m_buffer = new byte[StringBufferSize];
	}
}
*/