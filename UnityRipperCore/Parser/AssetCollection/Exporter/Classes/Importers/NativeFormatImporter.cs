using System;
using UnityRipper.Exporter.YAML;

using Object = UnityRipper.Classes.Object;

namespace UnityRipper.AssetExporters.Classes
{
	public class NativeFormatImporter : IYAMLExportable
	{
		public NativeFormatImporter(Object mainObject)
		{
			if(mainObject == null)
			{
				throw new ArgumentNullException(nameof(mainObject));
			}
			m_mainObject = mainObject;
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("externalObjects", YAMLMappingNode.Empty);
			node.Add("mainObjectFileID", m_mainObject.ExportID);
			node.Add("userData", string.Empty);
			node.Add("assetBundleName", string.Empty);
			node.Add("assetBundleVariant", string.Empty);
			return node;
		}

		private readonly Object m_mainObject;
	}
}
