namespace UnityRipper.Classes
{
	public sealed class String : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			Value = stream.ReadStringAligned();
		}

		public string Value { get; set; }
	}
}
