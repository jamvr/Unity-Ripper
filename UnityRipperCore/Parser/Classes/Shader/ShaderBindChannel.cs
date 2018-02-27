namespace UnityRipper.Classes.Shaders
{
	public sealed class ShaderBindChannel : IStreamReadable
	{
		public ShaderBindChannel()
		{
		}

		public ShaderBindChannel(ShaderChannel source, VertexComponent target)
		{
			Source = source;
			Target = target;
		}

		public void Read(EndianStream stream)
		{
			Source = (ShaderChannel)stream.ReadByte();
			Target = (VertexComponent)stream.ReadByte();
		}

		public ShaderChannel Source { get; private set; }
		public VertexComponent Target { get; private set; }
	}
}
