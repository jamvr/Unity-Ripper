using System;
using System.IO;

using Object = UnityRipper.Classes.Object;

namespace UnityRipper.AssetExporters
{
	public class BinaryAssetExporter : AssetExporter
	{
		public override IExportCollection CreateCollection(Object @object)
		{
			AssetExportCollection collection = new AssetExportCollection(this, @object);
			return collection;
		}

		public override bool Export(IExportCollection collection, string dirPath)
		{
			AssetExportCollection asset = (AssetExportCollection)collection;
			byte[] data = asset.Asset.ExportBinary();
			if (data == null)
			{
				return false;
			}

			string subFolder = asset.Asset.ClassID.ToString();
			string subPath = Path.Combine(dirPath, subFolder);
			string fileName = GetUniqueFileName(asset.Asset, subPath);
			string filePath = Path.Combine(subPath, fileName);

			if(!Directory.Exists(subPath))
			{
				Directory.CreateDirectory(subPath);
			}
			using (FileStream fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
			{
				using (BinaryWriter writer = new BinaryWriter(fileStream))
				{
					writer.Write(data);
				}
			}
			ExportMeta(asset, filePath);
			return true;
		}

		public override AssetType ToExportType(ClassIDType classID)
		{
			switch (classID)
			{
				case ClassIDType.Shader:
					return AssetType.Meta;

				case ClassIDType.Texture2D:
					return AssetType.Meta;

				default:
					throw new NotSupportedException(classID.ToString());
			}
		}
	}
}
