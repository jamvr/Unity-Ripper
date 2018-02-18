using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedPass : IStreamReadable
	{
		public SerializedPass(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new Exception(nameof(assetsFile));
			}

			State = new SerializedShaderState(assetsFile);
			ProgVertex = new SerializedProgram(assetsFile);
			ProgFragment = new SerializedProgram(assetsFile);
			ProgGeometry = new SerializedProgram(assetsFile);
			ProgHull = new SerializedProgram(assetsFile);
			ProgDomain = new SerializedProgram(assetsFile);
		}

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
			ProgDomain.Read(stream);
			HasInstancingVariant = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);

			UseName = stream.ReadStringAligned();
			Name = stream.ReadStringAligned();
			TextureName = stream.ReadStringAligned();
			Tags.Read(stream);
		}
		
		public IDictionary<string, int> NameIndices => m_nameIndices;
		public int Type { get; private set; }
		public SerializedShaderState State { get; }
		public uint ProgramMask { get; private set; }
		public SerializedProgram ProgVertex { get; }
		public SerializedProgram ProgFragment { get; }
		public SerializedProgram ProgGeometry { get; }
		public SerializedProgram ProgHull { get; }
		public SerializedProgram ProgDomain { get; }
		public bool HasInstancingVariant { get; private set; }
		public string UseName { get; private set; }
		public string Name { get; private set; }
		public string TextureName { get; private set; }
		public SerializedTagMap Tags { get; } = new SerializedTagMap();

		private readonly Dictionary<string, int> m_nameIndices = new Dictionary<string, int>();
	}
}
