namespace UnityRipper.Classes.Shaders
{
	public sealed class MatrixParameter : IStreamReadable
	{
		public MatrixParameter()
		{
		}

		public MatrixParameter(string name, ShaderParamType type, int index, int rowCount)
		{
			Name = name;
			NameIndex = -1;
			Type = type;
			Index = index;
			RowCount = (byte)rowCount;
		}

		public MatrixParameter(string name, ShaderParamType type, int index, int arraySize, int rowCount) :
			this(name, type, index, rowCount)
		{
			ArraySize = arraySize;
		}

		public void Read(EndianStream stream)
		{
			NameIndex = stream.ReadInt32();
			Index = stream.ReadInt32();
			ArraySize = stream.ReadInt32();
			Type = (ShaderParamType)stream.ReadByte();
			RowCount = stream.ReadByte();
			stream.AlignStream(AlignType.Align4);
		}

		public string Name { get; private set; }
		public int NameIndex { get; private set; }
		public int Index { get; private set; }
		public int ArraySize { get; private set; }
		public ShaderParamType Type { get; private set; }
		public byte RowCount { get; private set; }
	}
}
