using System.Collections.Generic;
using UnityRipper.Classes.AvatarMasks;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class AvatarMask : NamedObject
	{
		public AvatarMask(AssetInfo assetInfo) :
			base(assetInfo)
		{
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			Mask = stream.ReadUInt32Array();
			m_elements = stream.ReadArray<TransformMaskElement>();
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.Add("m_Mask", Mask.ExportYAML(true));
			node.Add("m_Elements", Elements.ExportYAML());
			return node;
		}

		public override string ExportExtension => "mask";

		public uint[] Mask { get; private set; }
		public IReadOnlyList<TransformMaskElement> Elements => m_elements;

		private TransformMaskElement[] m_elements;
	}
}
