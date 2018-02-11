using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public class MonoBehaviour : Behaviour
	{
		public MonoBehaviour(AssetInfo assetInfo) :
			base(assetInfo)
		{
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			throw new System.NotImplementedException();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			throw new System.NotImplementedException();
		}
	}
}
