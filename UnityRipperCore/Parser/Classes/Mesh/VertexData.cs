using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Meshes
{
	public sealed class VertexData : IStreamReadable, IYAMLExportable
	{
		public VertexData(IAssetsFile assetsFile, byte meshCompression)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			CurrentChannels = stream.ReadUInt32();
			VertexCount = stream.ReadUInt32();

			if (IsStreamFirst)
			{
				if (IsStreamSpecial && m_meshCompression != 0)
				{
#warning TODO: what is it?
					stream.BaseStream.Position += 12;
				}
				else
				{
					for(int i = 0; i < 4; i++)
					{
						StreamInfo streamInfo = new StreamInfo(m_assetsFile);
						streamInfo.Read(stream);
						m_streams[i] = streamInfo;
					}
				}
			}
			else
			{
				m_channels = stream.ReadArray<ChannelInfo>();
				stream.AlignStream(AlignType.Align4);

				if (IsReadStream)
				{
					m_streams = stream.ReadArray(() => new StreamInfo(m_assetsFile));
				}
			}

			m_data = stream.ReadByteArray();
			stream.AlignStream(AlignType.Align4);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_CurrentChannels", CurrentChannels);
			node.Add("m_VertexCount", VertexCount);
			node.Add("m_Channels", m_channels.ExportYAML());
			node.Add("m_DataSize", m_data.Length);
			node.Add("_typelessdata", m_data.ExportYAML());
			return node;
		}

		public uint CurrentChannels { get; private set; }
		public uint VertexCount { get; private set; }
		public IReadOnlyList<ChannelInfo> Channels => m_channels;
		public IReadOnlyList<StreamInfo> Streams => m_streams;
		public IReadOnlyList<byte> Data => m_data;

		private Version Version => m_assetsFile.Version;

		/// <summary>
		/// Less than 3.5.7
		/// </summary>
		private bool IsReadChannel => Version.IsLess(3, 5, 7);
		/// <summary>
		/// Less than 5.0.0
		/// </summary>
		private bool IsReadStream => Version.IsLess(5);
		/// <summary>
		/// Less than 4.0.0
		/// </summary>
		private bool IsStreamFirst => Version.IsLess(4);
		/// <summary>
		/// 3.5.0
		/// </summary>
		private bool IsStreamSpecial => Version.IsEqual(3, 5);

		private readonly IAssetsFile m_assetsFile;
		private readonly byte m_meshCompression = 0;

		private ChannelInfo[] m_channels;
		private StreamInfo[] m_streams;
		private byte[] m_data;
	}
}
