using System;
using System.Collections.Generic;

namespace UnityRipper.Classes.Shaders
{
	public sealed class ShaderSubProgram : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			int magic = stream.ReadInt32();
			if (magic != 201509030)
			{
				throw new Exception("Magic number doesn't match");
			}

			ProgramType = (ShaderGpuProgramType)stream.ReadInt32();
			int unknown1 = stream.ReadInt32();
			int unknown2 = stream.ReadInt32();
			int unknown3 = stream.ReadInt32();

			m_keywords = stream.ReadStringArray();
			ProgramData = stream.ReadStringAligned();
			ChannelsSourceMap |= stream.ReadInt32();

			m_channels = stream.ReadArray<BindChannels>();

			int unknownCount = stream.ReadInt32();
			for (int i = 0; i < unknownCount; i++)
			{
				string bindName = stream.ReadStringAligned();
				if (i != 0)
				{
					// set const buffer
				}
				int unknown5 = stream.ReadInt32();

#warning sepratate class, individual paramters
				m_parameters = stream.ReadArray<ShaderParameter>();
			}

			int unknownCount2 = stream.ReadInt32();
			for (int i = 0; i < unknownCount2; i++)
			{
				string unkString = stream.ReadStringAligned();
				int unknown6 = stream.ReadInt32();
				int unknown7 = stream.ReadInt32();
				int unknown8 = stream.ReadInt32();

				if (unknown6 == 0)
				{
					// texture param
				}
				else if (unknown6 == 1)
				{
					// bind constant buffer
				}
				else if (unknown6 == 2)
				{
					// buffer param
				}

			}
		}

		public ShaderGpuProgramType ProgramType { get; private set; }
		public IReadOnlyList<string> Keywords => m_keywords;
		public string ProgramData { get; private set; }
		public int ChannelsSourceMap { get; private set; }
		public IReadOnlyList<BindChannels> Channels => m_channels;
		public IReadOnlyList<ShaderParameter> Parameters => m_parameters;

		private string[] m_keywords;
		private BindChannels[] m_channels;
		private ShaderParameter[] m_parameters;
	}
}
