using UnityRipper.Classes;

namespace UnityRipper.AssetExporters
{
	public interface IAssetExporter
	{
		IExportCollection CreateCollection(Object @object);
		void Export(IExportCollection collection, string dirPath);
		AssetType ToExportType(ClassIDType classID);
	}
}
