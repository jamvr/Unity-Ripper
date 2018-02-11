using System;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Avatars
{
	public sealed class Collider : IStreamReadable, IYAMLExportable
	{
		public Collider(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			X = new XForm(assetsFile);
		}

		public void Read(EndianStream stream)
		{
			X.Read(stream);
			Type = stream.ReadUInt32();
			XMotionType = stream.ReadUInt32();
			YMotionType = stream.ReadUInt32();
			ZMotionType = stream.ReadUInt32();
			MinLimitX = stream.ReadSingle();
			MaxLimitX = stream.ReadSingle();
			MaxLimitY = stream.ReadSingle();
			MaxLimitZ = stream.ReadSingle();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_X", X.ExportYAML());
			node.Add("m_Type", Type);
			node.Add("m_XMotionType", XMotionType);
			node.Add("m_YMotionType", YMotionType);
			node.Add("m_ZMotionType", ZMotionType);
			node.Add("m_MinLimitX", MinLimitX);
			node.Add("m_MaxLimitX", MaxLimitX);
			node.Add("m_MaxLimitY", MaxLimitY);
			node.Add("m_MaxLimitZ", MaxLimitZ);
			return node;
		}

		public XForm X { get; }
		public uint Type { get; private set; }
		public uint XMotionType { get; private set; }
		public uint YMotionType { get; private set; }
		public uint ZMotionType { get; private set; }
		public float MinLimitX { get; private set; }
		public float MaxLimitX { get; private set; }
		public float MaxLimitY { get; private set; }
		public float MaxLimitZ { get; private set; }
	}
}
