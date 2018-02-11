using System.Collections.Generic;

namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedPass : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			m_nameIndices.Read(stream);
			Type = stream.ReadInt32();
			State.Read(stream);
			ProgramMask = stream.ReadUInt32();
			ProgVertex.Read(stream);
			ProgFragment.Read(stream);
			ProgGeometry.Read(stream);
			ProgHull.Read(stream);
			progDomain.Read(stream);
			HasInstancingVariant = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);

			UseName = stream.ReadStringAligned();
			Name = stream.ReadStringAligned();
			TextureName = stream.ReadStringAligned();
			Tags.Read(stream);
		}
		
		public IDictionary<string, int> NameIndices => m_nameIndices;
		public int Type { get; private set; }
		public SerializedShaderState State { get; } = new SerializedShaderState();
		public uint ProgramMask { get; private set; }
		public SerializedProgram ProgVertex { get; } = new SerializedProgram();
		public SerializedProgram ProgFragment { get; } = new SerializedProgram();
		public SerializedProgram ProgGeometry { get; } = new SerializedProgram();
		public SerializedProgram ProgHull { get; } = new SerializedProgram();
		public SerializedProgram progDomain { get; } = new SerializedProgram();
		public bool HasInstancingVariant { get; private set; }
		public string UseName { get; private set; }
		public string Name { get; private set; }
		public string TextureName { get; private set; }
		public SerializedTagMap Tags { get; } = new SerializedTagMap();

		private readonly Dictionary<string, int> m_nameIndices = new Dictionary<string, int>();
	}
}
