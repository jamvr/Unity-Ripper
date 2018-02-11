using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.AssetExporters.Classes;
using UnityRipper.Classes;

namespace UnityRipper
{
	public interface IAssetCollection
	{
		IAssetsFile GetAssetsFile(AssetsFilePtr ptr);
		IAssetsFile FindAssetsFile(AssetsFilePtr ptr);

		AssetType ToExportType(ClassIDType unityType);
		string GetExportID(Object @object);
		ExportPointer CreateExportPointer(Object @object);

		IEnumerable<Object> FetchObjects();
	}
}
