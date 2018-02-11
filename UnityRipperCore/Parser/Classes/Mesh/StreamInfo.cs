using System;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.Meshes
{
	public sealed class StreamInfo : IStreamReadable
	{
		public StreamInfo(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			ChannelMask = stream.ReadUInt32();
			Offset = stream.ReadUInt32();
			Stride = stream.ReadByte();

			if(IsReadAlign)
			{
				Align = stream.ReadUInt32();
#warning TODO: align(4)
				throw new NotImplementedException();
			}
			else
			{
				DividerOp = stream.ReadByte();
				Frequency = stream.ReadUInt16();
			}
		}

		public uint ChannelMask { get; private set; }
		public uint Offset { get; private set; }
		public byte Stride { get; private set; }
		public uint Align { get; private set; }
		public byte DividerOp { get; private set; }
		public ushort Frequency { get; private set; }

		private Version Version => m_assetsFile.Version;

		/// <summary>
		/// Less than 4.0.0
		/// </summary>
		private bool IsReadAlign => Version.IsLess(4);

		private readonly IAssetsFile m_assetsFile;
	}
}
