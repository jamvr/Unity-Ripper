using System;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.AssetExporters.Classes
{
	public class Meta : IYAMLDocExportable
	{
		public Meta(IExportCollection collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException(nameof(collection));
			}
			m_collection = collection;
		}

		public YAMLDocument ExportYAMLDocument()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("fileFormatVersion", FileFormatVersion);
			node.Add("guid", m_collection.GUID.ExportYAML());
			long cplusTick = (DateTime.Now.Ticks - 0x089f7ff5f7b58000) / 10000000;
			node.Add("timeCreated", cplusTick);
			node.Add("licenseType", "Free");			
			node.Add("NativeFormatImporter", m_collection.MetaImporter.ExportYAML());

			YAMLDocument document = new YAMLDocument(node);
			return document;
		}

		private int FileFormatVersion
		{
#warning TODO: export according to AssetsFile version?
			get { return 2; }
		}

		private readonly IExportCollection m_collection;
	}
}
