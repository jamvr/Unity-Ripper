using System;
using System.Collections.Generic;
using System.Text;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.Shaders
{
	public sealed class ShaderSubProgram : IStreamReadable
	{
		public ShaderSubProgram(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			int magic = stream.ReadInt32();
			if (magic != MagicNumber)
			{
				throw new Exception($"Magic number {magic} doesn't match");
			}

			ProgramType = (ShaderGpuProgramType)stream.ReadInt32();
			int unknown1 = stream.ReadInt32();
			int unknown2 = stream.ReadInt32();
			int unknown3 = stream.ReadInt32();
			if(IsReadUnknown4)
			{
				int unknown4 = stream.ReadInt32();
			}

			m_keywords = stream.ReadStringArray();
			m_programData = stream.ReadByteArray();
			stream.AlignStream(AlignType.Align4);

			int sourceMap = stream.ReadInt32();
			int bindCount = stream.ReadInt32();
			ShaderBindChannel[] channels = new ShaderBindChannel[bindCount];
			for (int i = 0; i < bindCount; i++)
			{
				ShaderChannel source = (ShaderChannel)stream.ReadUInt32();
				VertexComponent target = (VertexComponent)stream.ReadUInt32();
				ShaderBindChannel channel = new ShaderBindChannel(source, target);
				channels[i] = channel;
				sourceMap |= 1 << (int)source;
			}
			m_bindChannels = new ParserBindChannels(channels, sourceMap);

			List<VectorParameter> vectors = new List<VectorParameter>();
			List<MatrixParameter> matrices = new List<MatrixParameter>();
			List<TextureParameter> textures = new List<TextureParameter>();
			List<BufferBinding> buffers = new List<BufferBinding>();
			List<UAVParameter> uavs = IsReadUAVParameters ? new List<UAVParameter>() : null;
			List<SamplerParameter> samplers = IsReadSamplerParameters ? new List<SamplerParameter>() : null;
			List<BufferBinding> constBindings = new List<BufferBinding>();
			List<StructParameter> structs = IsReadStructParameters ? new List<StructParameter>() : null;

			int paramGroupCount = stream.ReadInt32();
			m_constantBuffers = new ConstantBuffer[paramGroupCount - 1];
			for (int i = 0; i < paramGroupCount; i++)
			{
				vectors.Clear();
				matrices.Clear();

				string name = stream.ReadStringAligned();
				int usedSize = stream.ReadInt32();
				int paramCount = stream.ReadInt32();
				for (int j = 0; j < paramCount; j++)
				{
					string paramName = stream.ReadStringAligned();
					ShaderParamType paramType = (ShaderParamType)stream.ReadInt32();
					int rows = stream.ReadInt32();
					int dimension = stream.ReadInt32();
					bool isMatrix = stream.ReadInt32() > 0;
					int arraySize = stream.ReadInt32();
					int index = stream.ReadInt32();

					if (isMatrix)
					{
						MatrixParameter matrix = IsAllParamArgs ?
							new MatrixParameter(paramName, paramType, index, arraySize, rows) :
							new MatrixParameter(paramName, paramType, index, rows);
						matrices.Add(matrix);
					}
					else
					{
						VectorParameter vector = IsAllParamArgs ?
							new VectorParameter(paramName, paramType, index, arraySize, dimension) :
							new VectorParameter(paramName, paramType, index, dimension);
						vectors.Add(vector);
					}
				}

				if (i == 0)
				{
					m_vectorParameters = vectors.ToArray();
					m_matrixParameters = matrices.ToArray();
				}
				else
				{
					ConstantBuffer constBuffer = new ConstantBuffer(m_assetsFile, name, matrices.ToArray(), vectors.ToArray(), usedSize);
					m_constantBuffers[i - 1] = constBuffer;
				}

				if (IsReadStructParameters)
				{
					int structCount = stream.ReadInt32();
					for(int j = 0; j < structCount; j++)
					{
						vectors.Clear();
						matrices.Clear();

						string structName = stream.ReadStringAligned();
						int index = stream.ReadInt32();
						int arraySize = stream.ReadInt32();
						int structSize = stream.ReadInt32();

						int strucParamCount = stream.ReadInt32();
						for(int k = 0; k < strucParamCount; k++)
						{
							string paramName = stream.ReadStringAligned();
							paramName = $"{structName}.{paramName}";
							ShaderParamType paramType = (ShaderParamType)stream.ReadInt32();
							int rows = stream.ReadInt32();
							int dimension = stream.ReadInt32();
							bool isMatrix = stream.ReadInt32() > 0;
							int vectorArraySize = stream.ReadInt32();
							int paramIndex = stream.ReadInt32();

							if (isMatrix)
							{
								MatrixParameter matrix = IsAllParamArgs ?
									new MatrixParameter(paramName, paramType, paramIndex, vectorArraySize, rows) :
									new MatrixParameter(paramName, paramType, paramIndex, rows);
								matrices.Add(matrix);
							}
							else
							{
								VectorParameter vector = IsAllParamArgs ?
									new VectorParameter(paramName, paramType, paramIndex, vectorArraySize, dimension) :
									new VectorParameter(paramName, paramType, paramIndex, dimension);
								vectors.Add(vector);
							}
						}

						StructParameter @struct = new StructParameter(structName, index, arraySize, structSize, vectors.ToArray(), matrices.ToArray());
						structs.Add(@struct);
					}
				}
			}

			int paramGroup2Count = stream.ReadInt32();
			for (int i = 0; i < paramGroup2Count; i++)
			{
				string name = stream.ReadStringAligned();
				int type = stream.ReadInt32();
				int index = stream.ReadInt32();
				int extraValue = stream.ReadInt32();

				if (type == 0)
				{
					TextureParameter texture;
					if (IsReadMultiSampled)
					{
						bool isMultiSampled = stream.ReadUInt32() > 0;
						texture = new TextureParameter(m_assetsFile, name, index, isMultiSampled, extraValue);
					}
					else
					{
						texture = new TextureParameter(m_assetsFile, name, index, extraValue);
					}
					textures.Add(texture);
				}
				else if (type == 1)
				{
					BufferBinding binding = new BufferBinding(name, index);
					constBindings.Add(binding);
				}
				else if (type == 2)
				{
					BufferBinding buffer = new BufferBinding(name, index);
					buffers.Add(buffer);
				}
				else if(type == 3 && IsReadUAVParameters)
				{
					UAVParameter uav = new UAVParameter(name, index, extraValue);
					uavs.Add(uav);
				}
				else if(type == 4 && IsReadSamplerParameters)
				{
					SamplerParameter sampler = new SamplerParameter((uint)extraValue, index);
					samplers.Add(sampler);
				}
				else
				{
					throw new Exception($"Unupported parameter type {type}");
				}
			}
			m_textureParameters = textures.ToArray();
			m_bufferParameters = buffers.ToArray();
			if(IsReadUAVParameters)
			{
				m_UAVParameters = uavs.ToArray();
			}
			if(IsReadSamplerParameters)
			{
				m_samplerParameters = samplers.ToArray();
			}
			m_constantBufferBindings = constBindings.ToArray();
			if(IsReadStructParameters)
			{
				m_structParameters = structs.ToArray();
			}
		}
		
		public void ToString(StringBuilder sb)
		{
			if(Keywords.Count > 0)
			{
				sb.Append("Keywords").Append(' ').Append('{').Append(' ');
				foreach(string keyword in Keywords)
				{
					sb.Append('"').Append(keyword).Append('"').Append(' ');
				}
				sb.Append('}').Append('\n');
				sb.AppendIntent(5);
			}

#warning TODO: D3D listing
			string programData = ProgramType.IsGL() || ProgramType.IsMetal() ?
				Encoding.UTF8.GetString(m_programData) :
				$"/*Can't export program data {ProgramType} as a text*/";
			if (programData.Length != 0)
			{
				sb.Append('"');
				sb.Append("!!").Append(ProgramType.ToString()).Append('\n');
				sb.AppendIntent(5);

				for (int i = 0; i < programData.Length; i++)
				{
					char c = programData[i];
					if (c == '\n')
					{
						sb.Append(c);
						if (i != programData.Length - 1)
						{
							sb.AppendIntent(5);
						}
					}
					else
					{
						sb.Append(c);
					}
				}

				if (sb[sb.Length - 1] == '\n')
				{
					sb.Length--;
				}
				sb.Append('"');
			}
		}
		
		public ShaderGpuProgramType ProgramType { get; private set; }
		public IReadOnlyList<string> Keywords => m_keywords;
		public IReadOnlyList<byte> ProgramData => m_programData;
		public ParserBindChannels BindChannels => m_bindChannels;
		public IReadOnlyList<VectorParameter> VectorParameters => m_vectorParameters;
		public IReadOnlyList<MatrixParameter> MatrixParameters => m_matrixParameters;
		public IReadOnlyList<TextureParameter> TextureParameters => m_textureParameters;
		public IReadOnlyList<BufferBinding> BufferParameters => m_bufferParameters;
		public IReadOnlyList<UAVParameter> UAVParameters => m_UAVParameters;
		public IReadOnlyList<ConstantBuffer> ConstantBuffers => m_constantBuffers;
		public IReadOnlyList<BufferBinding> ConstantBufferBindings => m_constantBufferBindings;
		public IReadOnlyList<StructParameter> StructParameters => m_structParameters;
		
		private int MagicNumber
		{
			get
			{
				if (Version.IsEqual(5, 3))
				{
					return 201509030;
				}
				else if (Version.IsEqual(5, 4))
				{
					return 201510240;
				}
				else if (Version.IsEqual(5, 5))
				{
					return 201608170;
				}
				else if (Version.IsGreaterEqual(5, 6) && Version.IsLess(2017, 3))
				{
					return 201609010;
				}
				else if (Version.IsEqual(2017, 3))
				{
					return 201708220;
				}
				else
				{
					throw new NotSupportedException($"No magic number for version {Version}");
				}
			}
		}

		/// <summary>
		/// 5.5.0 and greater
		/// </summary>
		private bool IsMagicNumber => SShader.IsSerialized(Version);
		/// <summary>
		/// 5.5.0 and greater
		/// </summary>
		private bool IsReadUnknown4 => SShader.IsSerialized(Version);
		/// <summary>
		/// 5.5.0 and greater
		/// </summary>
		private bool IsAllParamArgs => SShader.IsSerialized(Version);
		/// <summary>
		/// 5.5.0 and greater
		/// </summary>
		private bool IsReadUAVParameters => SShader.IsSerialized(Version);
		/// <summary>
		/// 2017.2 and greater
		/// </summary>
		private bool IsReadSamplerParameters => Version.IsGreaterEqual(2017, 2);
		/// <summary>
		/// 2017.3 and greater
		/// </summary>
		private bool IsReadStructParameters => Version.IsGreaterEqual(2017, 3);
		/// <summary>
		/// 2017.3 and greater
		/// </summary>
		public bool IsReadMultiSampled => Version.IsGreaterEqual(2017, 3);

		private Version Version => m_assetsFile.Version;
		
		private readonly IAssetsFile m_assetsFile;

		private string[] m_keywords;
		private byte[] m_programData;
		private ParserBindChannels m_bindChannels;
		private VectorParameter[] m_vectorParameters;
		private MatrixParameter[] m_matrixParameters;
		private TextureParameter[] m_textureParameters;
		private BufferBinding[] m_bufferParameters;
		private UAVParameter[] m_UAVParameters;
		private SamplerParameter[] m_samplerParameters;
		private ConstantBuffer[] m_constantBuffers;
		private BufferBinding[] m_constantBufferBindings;
		private StructParameter[] m_structParameters;
	}
}
