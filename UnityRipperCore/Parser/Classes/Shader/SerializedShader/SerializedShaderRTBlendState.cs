namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedShaderRTBlendState : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			SrcBlend.Read(stream);
			DestBlend.Read(stream);
			SrcBlendAlpha.Read(stream);
			DestBlendAlpha.Read(stream);
			BlendOp.Read(stream);
			BlendOpAlpha.Read(stream);
			ColMask.Read(stream);
		}

		public SerializedShaderFloatValue SrcBlend { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue DestBlend { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue SrcBlendAlpha { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue DestBlendAlpha { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue BlendOp { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue BlendOpAlpha { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue ColMask { get; } = new SerializedShaderFloatValue();
	}
}
