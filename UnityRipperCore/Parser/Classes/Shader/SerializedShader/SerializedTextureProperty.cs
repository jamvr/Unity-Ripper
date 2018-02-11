namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedTextureProperty : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			DefaultName = stream.ReadStringAligned();
			TexDim = stream.ReadInt32();
		}

		public string DefaultName { get; private set; }
		public int TexDim { get; private set; }
	}
}
