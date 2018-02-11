namespace UnityRipper.Classes.Shaders
{
	public sealed class SamplerParameter : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			Sampler = stream.ReadUInt32();
			BindPoint = stream.ReadInt32();
		}

		public uint Sampler { get; private set; }
		public int BindPoint { get; private set; }
	}
}
