/*namespace UnityRipper.Classes.Shaders
{
	public class ShaderTextureParameter
	{
		public ShaderTextureParameter(string name, int index, int dimension)
		{
			Name = name;
			Index = index;
			TextureDimension = dimension;
			Sampler = dimension >> 8;
			if (Sampler == 0xFFFFFF)
			{
				Sampler = -1;
			}
		}

		public string Name { get; private set; }
		public int Index { get; private set; }
		/// <summary>
		/// TextureDimension enum
		/// </summary>
		public int TextureDimension { get; private set; }
		public int Sampler { get; private set; }
	}
}
*/