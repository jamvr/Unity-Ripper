using System;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Avatars
{
	public sealed class Axes : IStreamReadable, IYAMLExportable
	{
		public Axes(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			Limit = new Limit(assetsFile);
		}

		public void Read(EndianStream stream)
		{
			PreQ.Read(stream);
			PostQ.Read(stream);
			Sgn.Read(stream);
			Limit.Read(stream);
			Length = stream.ReadSingle();
			Type = stream.ReadUInt32();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_PreQ", PreQ.ExportYAML());
			node.Add("m_PostQ", PostQ.ExportYAML());
			node.Add("m_Sgn", Sgn.ExportYAML());
			node.Add("m_Limit", Limit.ExportYAML());
			node.Add("m_Length", Length);
			node.Add("m_Type", Type);
			return node;
		}

		public Vector4f PreQ { get; private set; } = new Vector4f();
		public Vector4f PostQ { get; private set; } = new Vector4f();
		public Vector4f Sgn { get; private set; } = new Vector4f();
		public Limit Limit { get; }
		public float Length { get; private set; }
		public uint Type { get; private set; }
	}
}
