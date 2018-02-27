namespace UnityRipper.Classes.Shaders
{
	public sealed class SamplerParameter : IStreamReadable
	{
		public SamplerParameter()
		{
		}

		public SamplerParameter(uint sampler, int bindPoint)
		{
			Sampler = sampler;
			BindPoint = bindPoint;
		}

		public void Read(EndianStream stream)
		{
			Sampler = stream.ReadUInt32();
			BindPoint = stream.ReadInt32();
		}

		public uint Sampler { get; private set; }
		public int BindPoint { get; private set; }
	}
}
