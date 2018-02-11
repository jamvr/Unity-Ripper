using System.Collections.Generic;
using UnityRipper.Classes.Avatars;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class Avatar : NamedObject
	{
		public Avatar(AssetInfo assetInfo) :
			base(assetInfo)
		{
			AvatarConstant = new AvatarConstant(AssetsFile);
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			AvatarSize = stream.ReadUInt32();
			AvatarConstant.Read(stream);
			m_TOS.Read(stream);
		}

		public override void Reset()
		{
			base.Reset();

			m_TOS.Clear();
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.Add("m_AvatarSize", AvatarSize);
			node.Add("m_Avatar", AvatarConstant.ExportYAML());
			node.Add("m_TOS", TOS.ExportYAML());
			return node;
		}

		public uint AvatarSize { get; private set; }
		public AvatarConstant AvatarConstant { get; }
		public IReadOnlyDictionary<uint, string> TOS => m_TOS;

		private readonly Dictionary<uint, string> m_TOS = new Dictionary<uint, string>();
	}
}
