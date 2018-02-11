using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public abstract class RuntimeAnimatorController : NamedObject
	{
		public RuntimeAnimatorController(AssetInfo assetInfo) :
			base(assetInfo)
		{
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			return base.ExportYAMLRoot();
		}
	}
}
