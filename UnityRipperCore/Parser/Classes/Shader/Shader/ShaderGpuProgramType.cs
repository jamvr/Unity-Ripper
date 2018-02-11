namespace UnityRipper.Classes.Shaders
{
	public enum ShaderGpuProgramType
	{
		Unknown = 0x0,
		GLLegacy = 0x1,
		GLES31AEP = 0x2,
		GLES31 = 0x3,
		GLES3 = 0x4,
		GLES = 0x5,
		GLCore32 = 0x6,
		GLCore41 = 0x7,
		GLCore43 = 0x8,
		DX9VertexSM20 = 0x9,
		DX9VertexSM30 = 0xA,
		DX9PixelSM20 = 0xB,
		DX9PixelSM30 = 0xC,
		DX10Level9Vertex = 0xD,
		DX10Level9Pixel = 0xE,
		DX11VertexSM40 = 0xF,
		DX11VertexSM50 = 0x10,
		DX11PixelSM40 = 0x11,
		DX11PixelSM50 = 0x12,
		DX11GeometrySM40 = 0x13,
		DX11GeometrySM50 = 0x14,
		DX11HullSM50 = 0x15,
		DX11DomainSM50 = 0x16,
		MetalVS = 0x17,
		MetalFS = 0x18,
		Console = 0x19,
	}
}
