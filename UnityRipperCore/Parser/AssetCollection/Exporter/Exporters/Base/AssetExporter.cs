using UnityRipper.AssetExporters.Classes;
using UnityRipper.Classes;
using UnityRipper.Exporter.YAML;

using Object = UnityRipper.Classes.Object;

namespace UnityRipper.AssetExporters
{
	public abstract class AssetExporter : IAssetExporter
	{
		public abstract IExportCollection CreateCollection(Object @object);
		public abstract bool Export(IExportCollection collection, string dirPath);
		public abstract AssetType ToExportType(ClassIDType classID);

		protected string GetUniqueFileName(Object @object, string dirPath)
		{
			string fileName;
			NamedObject named = @object as NamedObject;
			if (named == null)
			{
				Prefab prefab = @object as Prefab;
				if(prefab != null)
				{
					fileName = prefab.Name;
				}
				else
				{
					fileName = @object.GetType().Name;
				}
			}
			else
			{
				fileName = named.Name;
			}
			fileName = FixName(fileName);

			fileName = DirectoryUtils.GetMaxIndexName(dirPath, fileName);
			fileName = $"{fileName}.{@object.ExportExtension}";
			return fileName;
		}

		protected void ExportMeta(IExportCollection collection, string filePath)
		{
			Meta meta = new Meta(collection);
			string metaPath = $"{filePath}.meta";
			YAMLExporter.Export(meta, metaPath, false);
		}

		protected string FixName(string path)
		{
			string fixedPath = path.Replace(':', '_');
			return fixedPath;
		}
	}
}
