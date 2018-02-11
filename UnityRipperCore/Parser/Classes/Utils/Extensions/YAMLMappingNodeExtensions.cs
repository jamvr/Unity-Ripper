using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public static class YAMLMappingNodeExtensions
	{
		public static void AddSerializedVersion(this YAMLMappingNode _this, int version)
		{
			if(version > 1)
			{
				_this.Add("serializedVersion", version);
			}
		}
	}
}
