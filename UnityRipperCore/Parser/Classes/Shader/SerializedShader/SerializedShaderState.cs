using System;
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

			if(IsReadZClip)
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

			FogMode = stream.ReadInt32();
			GpuProgramID = stream.ReadInt32();
			Tags.Read(stream);
			LOD = stream.ReadInt32();
			Lighting = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);
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
		public int FogMode { get; private set; }
		public int GpuProgramID { get; private set; }
		public SerializedTagMap Tags { get; } = new SerializedTagMap();
		public int LOD { get; private set; }
		public bool Lighting { get; private set; }

		/// <summary>
		/// 2017.2 and greater
		/// </summary>
		public bool IsReadZClip => Version.IsGreaterEqual(2017, 2);

		private Version Version => m_assetsFile.Version;

		private readonly IAssetsFile m_assetsFile;
	}
}
