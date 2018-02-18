using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedSubProgram : IStreamReadable
	{
		public SerializedSubProgram(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new Exception(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			BlobIndex = stream.ReadUInt32();
			Channels.Read(stream);
			m_keywordIndices = stream.ReadUInt16Array();
			if(IsAlignKeywordIndices)
			{
				stream.AlignStream(AlignType.Align4);
			}

			ShaderHardwareTier = stream.ReadByte();
			GpuProgramType = stream.ReadByte();
			stream.AlignStream(AlignType.Align4);

			m_vectorParams = stream.ReadArray<VectorParameter>();
			m_matrixParams = stream.ReadArray<MatrixParameter>();
			m_textureParams = stream.ReadArray(() => new TextureParameter(m_assetsFile));
			m_bufferParams = stream.ReadArray<BufferBinding>();
			m_constantBuffers = stream.ReadArray(() => new ConstantBuffer(m_assetsFile));
			m_constantBufferBindings = stream.ReadArray<BufferBinding>();
			m_UAVParams = stream.ReadArray<UAVParameter>();

			if(IsReadSamplers)
			{
				m_samplers = stream.ReadArray<SamplerParameter>();
			}
			if(IsReadShaderRequirements)
			{
				ShaderRequirements = stream.ReadInt32();
			}
		}

		public uint BlobIndex { get; private set; }
		public ParserBindChannels Channels { get; } = new ParserBindChannels();
		public IReadOnlyList<ushort> KeywordIndices => m_keywordIndices;
		public byte ShaderHardwareTier { get; private set; }
		public byte GpuProgramType { get; private set; }
		public IReadOnlyList<VectorParameter> VectorParams => m_vectorParams;
		public IReadOnlyList<MatrixParameter> MatrixParams => m_matrixParams;
		public IReadOnlyList<TextureParameter> TextureParams => m_textureParams;
		public IReadOnlyList<BufferBinding> BufferParams => m_bufferParams;
		public IReadOnlyList<ConstantBuffer> ConstantBuffers => m_constantBuffers;
		public IReadOnlyList<BufferBinding> ConstantBufferBindings => m_constantBufferBindings;
		public IReadOnlyList<UAVParameter> UAVParams => m_UAVParams;
		public IReadOnlyList<SamplerParameter> Samplers => m_samplers;
		public int ShaderRequirements { get; private set; }
		
		/// <summary>
		/// 2017.1 and greater
		/// </summary>
		public bool IsReadSamplers => Version.IsGreaterEqual(2017, 1);
		/// <summary>
		/// 2017.2 and greater
		/// </summary>
		public bool IsReadShaderRequirements => Version.IsGreaterEqual(2017, 2);

		/// <summary>
		/// 2017.1 and greater
		/// </summary>
		private bool IsAlignKeywordIndices => Version.IsGreaterEqual(2017, 1);

		private Version Version => m_assetsFile.Version;

		private readonly IAssetsFile m_assetsFile;

		private ushort[] m_keywordIndices;
		private VectorParameter[] m_vectorParams;
		private MatrixParameter[] m_matrixParams;
		private TextureParameter[] m_textureParams;
		private BufferBinding[] m_bufferParams;
		private ConstantBuffer[] m_constantBuffers;
		private BufferBinding[] m_constantBufferBindings;
		private UAVParameter[] m_UAVParams;
		private SamplerParameter[] m_samplers;
	}
}
