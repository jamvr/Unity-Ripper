using System.Text;

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

		public StringBuilder ToString(StringBuilder sb, int index)
		{
			if (!SrcBlendValue.IsOne() || !DestBlendValue.IsZero() || !SrcBlendAlphaValue.IsOne() || !DestBlendAlphaValue.IsZero())
			{
				sb.Append("\t\t\t").Append("Blend").Append(' ');
				if(index != -1)
				{
					sb.Append(index).Append(' ');
				}
				sb.Append(SrcBlendValue).Append(' ').Append(DestBlendValue);
				if(!SrcBlendAlphaValue.IsOne() || !DestBlendAlphaValue.IsZero())
				{
					sb.Append(',').Append(' ').Append(SrcBlendAlphaValue).Append(' ').Append(DestBlendAlphaValue);
				}
				sb.Append('\n');
			}

			if(!BlendOpValue.IsAdd() || !BlendOpAlphaValue.IsAdd())
			{
				sb.AppendIntent(3).Append("BlendOp").Append(' ');
				if(index != -1)
				{
					sb.Append(index).Append(' ');
				}
				sb.Append(BlendOpValue);
				if (!BlendOpAlphaValue.IsAdd())
				{
					sb.Append(',').Append(' ').Append(BlendOpAlphaValue);
				}
				sb.Append('\n');
			}
			
			if(!ColMaskValue.IsRBGA())
			{
				sb.AppendIntent(3).Append("ColorMask").Append(' ');
				if (ColMaskValue.IsNone())
				{
					sb.Append(0);
				}
				else
				{
					if (ColMaskValue.IsRed())
					{
						sb.Append(ColorMask.R.ToString());
					}
					if (ColMaskValue.IsGreen())
					{
						sb.Append(ColorMask.G.ToString());
					}
					if (ColMaskValue.IsBlue())
					{
						sb.Append(ColorMask.B.ToString());
					}
					if (ColMaskValue.IsAlpha())
					{
						sb.Append(ColorMask.A.ToString());
					}
				}
				sb.Append(' ').Append(index).Append('\n');
			}

			return sb;
		}

		public SerializedShaderFloatValue SrcBlend { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue DestBlend { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue SrcBlendAlpha { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue DestBlendAlpha { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue BlendOp { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue BlendOpAlpha { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue ColMask { get; } = new SerializedShaderFloatValue();

		private BlendFactor SrcBlendValue => (BlendFactor)SrcBlend.Val;
		private BlendFactor DestBlendValue => (BlendFactor)DestBlend.Val;
		private BlendFactor SrcBlendAlphaValue => (BlendFactor)SrcBlendAlpha.Val;
		private BlendFactor DestBlendAlphaValue => (BlendFactor)DestBlendAlpha.Val;
		private BlendOp BlendOpValue => (BlendOp)BlendOp.Val;
		private BlendOp BlendOpAlphaValue => (BlendOp)BlendOpAlpha.Val;
		private ColorMask ColMaskValue => (ColorMask)ColMask.Val;
	}
}
