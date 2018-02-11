namespace UnityRipper.Classes.Shaders
{
	public enum ShaderChannel : uint
	{
		None = 0xFFFFFFFF,
		Vertex = 0x0,
		Normal = 0x1,
		Color = 0x2,
		TexCoord0 = 0x3,
		TexCoord1 = 0x4,
		TexCoord2 = 0x5,
		TexCoord3 = 0x6,
		Tangent = 0x7,
		Count = 0x8,
	}
}
