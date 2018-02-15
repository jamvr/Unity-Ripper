namespace UnityRipper.Classes.Shaders
{
	public sealed class ShaderParameter : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			Name = stream.ReadStringAligned();
			ParamType = (ShaderParamType)stream.ReadInt32();
			Rows = stream.ReadInt32();
			Dimension = stream.ReadInt32();
			IsMatrix = stream.ReadInt32() > 0;
			int unknown = stream.ReadInt32();
			Index = stream.ReadInt32();
		}

		public string Name { get; private set; }
		public ShaderParamType ParamType { get; private set; }
		public int Rows { get; private set; }
		public int Dimension { get; private set; }
		/// <summary>
		/// Matrix, otherwise vector
		/// </summary>
		public bool IsMatrix { get; private set; }
		public int Index { get; private set; }
	}
}
