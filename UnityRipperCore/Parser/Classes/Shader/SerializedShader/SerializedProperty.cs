using System.Collections.Generic;

namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedProperty : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			Name = stream.ReadStringAligned();
			Description = stream.ReadStringAligned();
			m_attributes = stream.ReadStringArray();
			Type = stream.ReadInt32();
			Flags = stream.ReadUInt32();
			stream.Read(m_defValue, 0, m_defValue.Length);
			DefTexture.Read(stream);
		}

		public string Name { get; private set; }
		public string Description { get; private set; }
		public IReadOnlyList<string> Attributes => m_attributes;
		public int Type { get; private set; }
		public uint Flags { get; private set; }
		public IReadOnlyList<float> DefValue => m_defValue;
		public SerializedTextureProperty DefTexture { get; } = new SerializedTextureProperty();

		private string[] m_attributes;
		private float[] m_defValue = new float[4];
	}
}
