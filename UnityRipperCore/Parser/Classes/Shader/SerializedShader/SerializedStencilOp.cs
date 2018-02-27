using System.Text;

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

		public StringBuilder ToString(StringBuilder sb, StencilType type)
		{
			sb.AppendIntent(4).Append("Comp").Append(type.ToSuffixString()).Append(' ').Append(CompValue).Append('\n');
			sb.AppendIntent(4).Append("Pass").Append(type.ToSuffixString()).Append(' ').Append(PassValue).Append('\n');
			sb.AppendIntent(4).Append("Fail").Append(type.ToSuffixString()).Append(' ').Append(FailValue).Append('\n');
			sb.AppendIntent(4).Append("ZFail").Append(type.ToSuffixString()).Append(' ').Append(ZFailValue).Append('\n');
			return sb;
		}

		public bool IsDefault => PassValue.IsKeep() && FailValue.IsKeep() && ZFailValue.IsKeep() && CompValue.IsAlways();

		public SerializedShaderFloatValue Pass { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue Fail { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue ZFail { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue Comp { get; } = new SerializedShaderFloatValue();

		private StencilOp PassValue => (StencilOp)Pass.Val;
		private StencilOp FailValue => (StencilOp)Fail.Val;
		private StencilOp ZFailValue => (StencilOp)ZFail.Val;
		private StencilComp CompValue => (StencilComp)Comp.Val;
	}
}
