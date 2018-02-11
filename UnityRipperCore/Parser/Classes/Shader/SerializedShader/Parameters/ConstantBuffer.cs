using System.Collections.Generic;

namespace UnityRipper.Classes.Shaders
{
	public sealed class ConstantBuffer : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			NameIndex = stream.ReadInt32();
			m_matrixParams = stream.ReadArray<MatrixParameter>();
			stream.AlignStream(AlignType.Align4);
			m_vectorParams = stream.ReadArray<VectorParameter>();
			stream.AlignStream(AlignType.Align4);
			Size = stream.ReadInt32();
		}

		public int NameIndex { get; private set; }
		public IReadOnlyList<MatrixParameter> MatrixParams => m_matrixParams;
		public IReadOnlyList<VectorParameter> VectorParams => m_vectorParams;
		public int Size { get; private set; }

		private MatrixParameter[] m_matrixParams;
		private VectorParameter[] m_vectorParams;
	}
}
