namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedShaderFloatValue : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			Val = stream.ReadSingle();
			Name = stream.ReadStringAligned();
		}

		public float Val { get; private set; }
		public string Name { get; private set; }
	}
}
