namespace UnityRipper.Classes.Shaders
{
	public sealed class ShaderBindChannel : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			Source = stream.ReadByte();
			Target = stream.ReadByte();
		}

		public byte Source { get; private set; }
		public byte Target { get; private set; }
	}
}
