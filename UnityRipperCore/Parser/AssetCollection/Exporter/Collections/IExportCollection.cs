using System.Collections.Generic;
using UnityRipper.AssetExporters.Classes;
using UnityRipper.Classes;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.AssetExporters
{
	public interface IExportCollection
	{
		string GetExportID(Object @object);
		ExportPointer CreateExportPointer(Object @object, bool isLocal);

		IAssetExporter AssetExporter { get; }
		IEnumerable<Object> Objects { get; }
		string Name { get; }
		UnityGUID GUID { get; }
		IYAMLExportable MetaImporter { get; }
	}
}
