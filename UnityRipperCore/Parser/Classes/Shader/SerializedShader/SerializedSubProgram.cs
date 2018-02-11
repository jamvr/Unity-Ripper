using System.Collections.Generic;

namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedSubProgram : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			BlobIndex = stream.ReadUInt32();
			Channels.Read(stream);
			m_keywordIndices = stream.ReadUInt16Array();
			stream.AlignStream(AlignType.Align4);

			ShaderHardwareTier = stream.ReadByte();
			GpuProgramType = stream.ReadByte();
			stream.AlignStream(AlignType.Align4);

			m_vectorParams = stream.ReadArray<VectorParameter>();
			m_matrixParams = stream.ReadArray<MatrixParameter>();
			m_textureParams = stream.ReadArray<TextureParameter>();
			m_bufferParams = stream.ReadArray<BufferBinding>();
			m_constantBuffers = stream.ReadArray<ConstantBuffer>();
			m_constantBufferBindings = stream.ReadArray<BufferBinding>();
			m_UAVParams = stream.ReadArray<UAVParameter>();
			m_samplers = stream.ReadArray<SamplerParameter>();
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
