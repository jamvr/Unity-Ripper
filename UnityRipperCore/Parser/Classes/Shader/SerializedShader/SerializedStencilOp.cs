namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedStencilOp : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			Pass.Read(stream);
			Fail.Read(stream);
			ZFail.Read(stream);
			Comp.Read(stream);
		}

		public SerializedShaderFloatValue Pass { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue Fail { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue ZFail { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue Comp { get; } = new SerializedShaderFloatValue();
	}
}
