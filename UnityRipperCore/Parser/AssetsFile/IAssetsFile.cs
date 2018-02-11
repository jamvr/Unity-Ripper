using System.Collections.Generic;
using UnityRipper.Classes;
using UnityRipper.AssetExporters.Classes;

namespace UnityRipper.AssetsFiles
{
	public interface IAssetsFile
	{
		/// <summary>
		/// Get object in assets file with specified file index
		/// </summary>
		/// <param name="fileIndex">Dependency index</param>
		/// <param name="pathID">Path ID for searching object</param>
		/// <returns></returns>
		Object GetObject(int fileIndex, long pathID);
		/// <summary>
		/// Get object from current assets file
		/// </summary>
		/// <param name="fileIndex">Path ID for searching object</param>
		/// <returns>Founded object</returns>
		Object GetObject(long pathID);
		/// <summary>
		/// Try to find object in assets file with specified file index
		/// </summary>
		/// <param name="fileIndex">Dependency index</param>
		/// <param name="pathID">Path ID for searching object</param>
		/// <returns></returns>
		Object FindObject(int fileIndex, long pathID);
		/// <summary>
		/// Try to find object from current assets file
		/// </summary>
		/// <param name="pathID">Path ID for searching object</param>
		/// <returns>Founded object or null</returns>
		Object FindObject(long pathID);
		AssetInfo GetAssetInfo(long pathID);

		AssetType ToExportType(ClassIDType unityType);
		string GetExportID(Object @object);
		ExportPointer CreateExportPointer(Object @object);

		IEnumerable<Object> FetchObjects();

		string Name { get; }
		Platform Platform { get; }
		Version Version { get; }
		IReadOnlyList<AssetsFilePtr> Dependencies { get; }

		IAssetCollection Collection { get; }
	}
}
