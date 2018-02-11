namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedShaderDependency : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			From = stream.ReadStringAligned();
			To = stream.ReadStringAligned();
		}

		public string From { get; private set; }
		public string To { get; private set; }
	}
}
