using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public abstract class GlobalGameManager : Object
	{
		protected GlobalGameManager(AssetInfo assetInfo):
			base(assetInfo)
		{
		}

		public override void Read(EndianStream stream)
		{
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			return node;
		}
	}
}
