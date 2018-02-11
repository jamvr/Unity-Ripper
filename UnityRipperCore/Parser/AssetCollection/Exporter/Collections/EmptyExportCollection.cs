using System;
using System.Collections.Generic;
using UnityRipper.AssetExporters.Classes;
using UnityRipper.Classes;
using UnityRipper.Exporter.YAML;

using Object = UnityRipper.Classes.Object;

namespace UnityRipper.AssetExporters
{
	internal class EmptyExportCollection : IExportCollection
	{
		public EmptyExportCollection(DummyAssetExporter assetExporter, string name)
		{
			if (assetExporter == null)
			{
				throw new ArgumentNullException(nameof(assetExporter));
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}
			AssetExporter = assetExporter;
			Name = name;
		}

		public string GetExportID(Object @object)
		{
			throw new NotSupportedException();
		}

		public ExportPointer CreateExportPointer(Object @object, bool isLocal)
		{
			throw new NotSupportedException();
		}

		public IAssetExporter AssetExporter { get; }
		public IEnumerable<Object> Objects
		{
			get { yield break; }
		}
		public string Name { get; }
		public UnityGUID GUID => throw new NotSupportedException();
		public IYAMLExportable MetaImporter => throw new NotSupportedException();
	}
}
