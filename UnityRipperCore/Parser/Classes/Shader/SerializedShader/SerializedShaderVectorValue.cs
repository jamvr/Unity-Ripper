namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedShaderVectorValue : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			X.Read(stream);
			Y.Read(stream);
			Z.Read(stream);
			W.Read(stream);
			Name = stream.ReadStringAligned();
		}

		public SerializedShaderFloatValue X { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue Y { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue Z { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue W { get; } = new SerializedShaderFloatValue();
		public string Name { get; private set; }
	}
}
