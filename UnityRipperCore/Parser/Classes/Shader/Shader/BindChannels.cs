namespace UnityRipper.Classes.Shaders
{
	public sealed class BindChannels : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			Channel = (ShaderChannel)stream.ReadUInt32();
			Component = (VertexComponent)stream.ReadUInt32();
		}

		public ShaderChannel Channel { get; private set; }
		public VertexComponent Component { get; private set; }
	}
}
