using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.Shaders
{
	public sealed class ConstantBuffer : IStreamReadable
	{
		public ConstantBuffer(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new Exception(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public ConstantBuffer(IAssetsFile assetsFile, string name, MatrixParameter[] matrices, VectorParameter[] vectors, int usedSize):
			this(assetsFile)
		{
			Name = name;
			NameIndex = -1;
			m_matrixParams = matrices;
			m_vectorParams = vectors;
			if(IsReadStructParams)
			{
				m_structParams = new StructParameter[0];
			}
			Size = usedSize;
		}

		public void Read(EndianStream stream)
		{
			NameIndex = stream.ReadInt32();
			m_matrixParams = stream.ReadArray<MatrixParameter>();
			m_vectorParams = stream.ReadArray<VectorParameter>();
			if(IsReadStructParams)
			{
				m_structParams = stream.ReadArray<StructParameter>();
			}
			Size = stream.ReadInt32();
		}

		public string Name { get; private set; }
		public int NameIndex { get; private set; }
		public IReadOnlyList<MatrixParameter> MatrixParams => m_matrixParams;
		public IReadOnlyList<VectorParameter> VectorParams => m_vectorParams;
		public IReadOnlyList<StructParameter> StructParams => m_structParams;
		public int Size { get; private set; }

		/// <summary>
		/// 2017.3 and greater
		/// </summary>
		public bool IsReadStructParams => Version.IsGreaterEqual(2017, 3);

		private Version Version => m_assetsFile.Version;

		private readonly IAssetsFile m_assetsFile;

		private MatrixParameter[] m_matrixParams;
		private VectorParameter[] m_vectorParams;
		private StructParameter[] m_structParams;
	}
}
