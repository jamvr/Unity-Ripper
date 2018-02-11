using System;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Avatars
{
	public sealed class Handle : IStreamReadable, IYAMLExportable
	{
		public Handle(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			X = new XForm(assetsFile);
		}

		public void Read(EndianStream stream)
		{
			X.Read(stream);
			ParentHumanIndex = stream.ReadUInt32();
			ID = stream.ReadUInt32();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_X", X.ExportYAML());
			node.Add("m_ParentHumanIndex", ParentHumanIndex);
			node.Add("m_ID", ID);
			return node;
		}

		public XForm X { get; }
		public uint ParentHumanIndex { get; private set; }
		public uint ID { get; private set; }
	}
}
