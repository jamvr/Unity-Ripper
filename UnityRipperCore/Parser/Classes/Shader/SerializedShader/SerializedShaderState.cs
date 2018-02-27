using System;
using System.Globalization;
using System.Text;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedShaderState : IStreamReadable
	{
		public SerializedShaderState(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new Exception(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			Name = stream.ReadStringAligned();
			RtBlend0.Read(stream);
			RtBlend1.Read(stream);
			RtBlend2.Read(stream);
			RtBlend3.Read(stream);
			RtBlend4.Read(stream);
			RtBlend5.Read(stream);
			RtBlend6.Read(stream);
			RtBlend7.Read(stream);
			RtSeparateBlend = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);

			if (IsReadZClip)
			{
				ZClip.Read(stream);
			}
			ZTest.Read(stream);
			ZWrite.Read(stream);
			Culling.Read(stream);
			OffsetFactor.Read(stream);
			OffsetUnits.Read(stream);
			AlphaToMask.Read(stream);
			StencilOp.Read(stream);
			StencilOpFront.Read(stream);
			StencilOpBack.Read(stream);
			StencilReadMask.Read(stream);
			StencilWriteMask.Read(stream);
			StencilRef.Read(stream);
			FogStart.Read(stream);
			FogEnd.Read(stream);
			FogDensity.Read(stream);
			FogColor.Read(stream);

			FogMode = (FogMode)stream.ReadInt32();
			GpuProgramID = stream.ReadInt32();
			Tags.Read(stream);
			LOD = stream.ReadInt32();
			Lighting = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);
		}

		public StringBuilder ToString(StringBuilder sb)
		{
			if (Name != string.Empty)
			{
				sb.AppendIntent(3).Append("Name").Append(' ');
				sb.Append('"').Append(Name).Append('"');
				sb.Append('\n');
			}
			if (LOD != 0)
			{
				sb.AppendIntent(3).Append("LOD").Append(' ').Append(LOD).Append('\n');
			}
			Tags.ToString(sb, 3);
			
			RtBlend0.ToString(sb, RtSeparateBlend ? 0 : -1);
			RtBlend1.ToString(sb, 1);
			RtBlend2.ToString(sb, 2);
			RtBlend3.ToString(sb, 3);
			RtBlend4.ToString(sb, 4);
			RtBlend5.ToString(sb, 5);
			RtBlend6.ToString(sb, 6);
			RtBlend7.ToString(sb, 7);

			if (AlphaToMaskValue)
			{
				sb.AppendIntent(3).Append("AlphaToMask").Append(' ').Append("On").Append('\n');
			}

			if (!ZClipValue.IsOn())
			{
				sb.AppendIntent(3).Append("ZClip").Append(' ').Append(ZClipValue).Append('\n');
			}
			if (!ZTestValue.IsLEqual())
			{
				sb.AppendIntent(3).Append("ZTest").Append(' ').Append(ZTestValue).Append('\n');
			}
			if (!ZWriteValue.IsOn())
			{
				sb.AppendIntent(3).Append("ZWrite").Append(' ').Append(ZWriteValue).Append('\n');
			}
			if (!CullingValue.IsBack())
			{
				sb.AppendIntent(3).Append("Cull").Append(' ').Append(CullingValue).Append('\n');
			}
			if (!OffsetFactor.IsZero || !OffsetUnits.IsZero)
			{
				sb.AppendIntent(3).Append("Offset").Append(' ').Append(OffsetFactor.Val).Append(',').Append(' ').Append(OffsetUnits.Val).Append('\n');
			}

			if (!StencilRef.IsZero || !StencilReadMask.IsMax || !StencilWriteMask.IsMax || !StencilOp.IsDefault || !StencilOpFront.IsDefault || !StencilOpBack.IsDefault)
			{
				sb.AppendIntent(3).Append("Stencil").Append(' ').Append('{').Append('\n');
				if(!StencilRef.IsZero)
				{
					sb.AppendIntent(4).Append("Ref").Append(' ').Append(StencilRef.Val).Append('\n');
				}
				if(!StencilReadMask.IsMax)
				{
					sb.AppendIntent(4).Append("ReadMask").Append(' ').Append(StencilReadMask.Val).Append('\n');
				}
				if(!StencilWriteMask.IsMax)
				{
					sb.AppendIntent(4).Append("WriteMask").Append(' ').Append(StencilWriteMask.Val).Append('\n');
				}
				if(!StencilOp.IsDefault)
				{
					StencilOp.ToString(sb, StencilType.Base);
				}
				if(!StencilOpFront.IsDefault)
				{
					StencilOpFront.ToString(sb, StencilType.Front);
				}
				if(!StencilOpBack.IsDefault)
				{
					StencilOpBack.ToString(sb, StencilType.Back);
				}
				sb.AppendIntent(3).Append('}').Append('\n');
			}
			
			if(!FogMode.IsUnknown() || !FogColor.IsZero || !FogDensity.IsZero || !FogStart.IsZero || !FogEnd.IsZero)
			{
				sb.AppendIntent(3).Append("Fog").Append(' ').Append('{').Append('\n');
				if(!FogMode.IsUnknown())
				{
					sb.AppendIntent(4).Append("Mode").Append(' ').Append(FogMode).Append('\n');
				}
				if (!FogColor.IsZero)
				{
					sb.AppendIntent(4).Append("Color").Append(' ').Append('(');
					sb.Append(FogColor.X.Val.ToString(CultureInfo.InvariantCulture)).Append(',');
					sb.Append(FogColor.Y.Val.ToString(CultureInfo.InvariantCulture)).Append(',');
					sb.Append(FogColor.Z.Val.ToString(CultureInfo.InvariantCulture)).Append(',');
					sb.Append(FogColor.W.Val.ToString(CultureInfo.InvariantCulture));
					sb.Append(')').Append('\n');
				}
				if (!FogDensity.IsZero)
				{
					sb.AppendIntent(4).Append("Density").Append(' ').Append(FogDensity.Val.ToString(CultureInfo.InvariantCulture)).Append('\n');
				}
				if (!FogStart.IsZero ||!FogEnd.IsZero)
				{
					sb.AppendIntent(4).Append("Range").Append(' ').Append(FogStart.Val.ToString(CultureInfo.InvariantCulture)).Append(',');
					sb.Append(' ').Append(FogEnd.Val.ToString(CultureInfo.InvariantCulture)).Append('\n');
				}
				sb.AppendIntent(3).Append('}').Append('\n');
			}

			if(Lighting)
			{
				sb.AppendIntent(3).Append("Lighting").Append(' ').Append(LightingValue).Append('\n');
			}
			sb.AppendIntent(3).Append("GpuProgramID").Append(' ').Append(GpuProgramID).Append('\n');
			return sb;
		}

		public string Name { get; private set; }
		public SerializedShaderRTBlendState RtBlend0 { get; } = new SerializedShaderRTBlendState();
		public SerializedShaderRTBlendState RtBlend1 { get; } = new SerializedShaderRTBlendState();
		public SerializedShaderRTBlendState RtBlend2 { get; } = new SerializedShaderRTBlendState();
		public SerializedShaderRTBlendState RtBlend3 { get; } = new SerializedShaderRTBlendState();
		public SerializedShaderRTBlendState RtBlend4 { get; } = new SerializedShaderRTBlendState();
		public SerializedShaderRTBlendState RtBlend5 { get; } = new SerializedShaderRTBlendState();
		public SerializedShaderRTBlendState RtBlend6 { get; } = new SerializedShaderRTBlendState();
		public SerializedShaderRTBlendState RtBlend7 { get; } = new SerializedShaderRTBlendState();
		public bool RtSeparateBlend { get; private set; }
		public SerializedShaderFloatValue ZClip { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue ZTest { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue ZWrite { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue Culling { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue OffsetFactor { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue OffsetUnits { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue AlphaToMask { get; } = new SerializedShaderFloatValue();
		public SerializedStencilOp StencilOp { get; } = new SerializedStencilOp();
		public SerializedStencilOp StencilOpFront { get; } = new SerializedStencilOp();
		public SerializedStencilOp StencilOpBack { get; } = new SerializedStencilOp();
		public SerializedShaderFloatValue StencilReadMask { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue StencilWriteMask { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue StencilRef { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue FogStart { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue FogEnd { get; } = new SerializedShaderFloatValue();
		public SerializedShaderFloatValue FogDensity { get; } = new SerializedShaderFloatValue();
		public SerializedShaderVectorValue FogColor { get; } = new SerializedShaderVectorValue();
		public FogMode FogMode { get; private set; }
		public int GpuProgramID { get; private set; }
		public SerializedTagMap Tags { get; } = new SerializedTagMap();
		public int LOD { get; private set; }
		public bool Lighting { get; private set; }

		private ZClip ZClipValue => (ZClip)ZClip.Val;
		private ZTest ZTestValue => (ZTest)ZTest.Val;
		private ZWrite ZWriteValue => (ZWrite)ZWrite.Val;
		private Cull CullingValue => (Cull)Culling.Val;
		private bool AlphaToMaskValue => AlphaToMask.Val > 0;
		private string LightingValue => Lighting ? "On" : "Off";

		/// <summary>
		/// 2017.2 and greater
		/// </summary>
		public bool IsReadZClip => Version.IsGreaterEqual(2017, 2);

		private Version Version => m_assetsFile.Version;

		private readonly IAssetsFile m_assetsFile;
	}
}
