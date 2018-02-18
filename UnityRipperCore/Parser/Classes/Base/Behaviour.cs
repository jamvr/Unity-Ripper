using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public abstract class Behaviour : Component
	{
		protected Behaviour(AssetInfo assetInfo):
			base(assetInfo)
		{
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			IsEnabled = stream.ReadByte();
			stream.AlignStream(AlignType.Align4);
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.Add("m_Enabled", IsEnabled);
			return node;
		}

		public byte IsEnabled { get; private set; }
	}
}
