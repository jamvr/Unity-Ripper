using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class Texture : NamedObject
	{
		public Texture(AssetInfo assetInfo):
			base(assetInfo)
		{
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			throw new System.NotImplementedException();
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();

			throw new System.NotImplementedException();
		}
	}
}
