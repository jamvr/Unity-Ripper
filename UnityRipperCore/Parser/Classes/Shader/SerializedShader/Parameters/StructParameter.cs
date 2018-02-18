using System;
using System.Collections.Generic;

namespace UnityRipper.Classes.Shaders
{
	public class StructParameter : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			NameIndex = stream.ReadInt32();
			Index = stream.ReadInt32();
			ArraySize = stream.ReadInt32();
			StructSize = stream.ReadInt32();
			m_vectorMembers = stream.ReadArray<VectorParameter>();
			stream.AlignStream(AlignType.Align4);
			m_matrixMembers = stream.ReadArray<MatrixParameter>();
			stream.AlignStream(AlignType.Align4);
		}

		public int NameIndex { get; set; }
		public int Index { get; set; }
		public int ArraySize { get; set; }
		public int StructSize { get; set; }
		public IReadOnlyList<VectorParameter> VectorMembers => m_vectorMembers;
		public IReadOnlyList<MatrixParameter> MatrixMembers => m_matrixMembers;

		private VectorParameter[] m_vectorMembers;
		private MatrixParameter[] m_matrixMembers;
	}
}
