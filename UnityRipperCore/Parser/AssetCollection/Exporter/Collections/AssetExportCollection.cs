using System;
using System.Collections.Generic;
using UnityRipper.AssetExporters.Classes;
using UnityRipper.Classes;
using UnityRipper.Exporter.YAML;

using Object = UnityRipper.Classes.Object;

namespace UnityRipper.AssetExporters
{
	public class AssetExportCollection : IExportCollection
	{
		public AssetExportCollection(IAssetExporter assetExporter, Object asset)
		{
			if (assetExporter == null)
			{
				throw new ArgumentNullException(nameof(assetExporter));
			}
			if (asset == null)
			{
				throw new ArgumentNullException(nameof(asset));
			}
			AssetExporter = assetExporter;
			Asset = asset;
			MetaImporter = new NativeFormatImporter(asset);
		}

		public static string GetMainExportID(Object @object)
		{
			return $"{(int)@object.ClassID}00000";
		}

		public virtual string GetExportID(Object @object)
		{
			if(@object == Asset)
			{
				return GetMainExportID(Asset);
			}
			throw new ArgumentException(nameof(@object));
		}

		public ExportPointer CreateExportPointer(Object @object, bool isLocal)
		{
			string exportID = GetExportID(@object);
			return isLocal ?
				new ExportPointer(exportID) :
				new ExportPointer(exportID, @object.GUID, AssetExporter.ToExportType(@object.ClassID));
		}

		public IAssetExporter AssetExporter { get; }
		public virtual IEnumerable<Object> Objects
		{
			get { yield return Asset; }
		}
		public string Name => Asset.ToString();
		public UnityGUID GUID => Asset.GUID;
		public Object Asset { get; }
		public IYAMLExportable MetaImporter { get; }
	}
}
