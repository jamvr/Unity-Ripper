using System;
using System.Collections.Generic;
using UnityRipper.AssetExporters.Classes;
using UnityRipper.Classes;
using UnityRipper.Exporter.YAML;

using Object = UnityRipper.Classes.Object;

namespace UnityRipper.AssetExporters
{
	internal class SkipExportCollection : IExportCollection
	{
		public SkipExportCollection(DummyAssetExporter assetExporter, NamedObject asset):
			this(assetExporter, asset, asset.Name)
		{
		}

		public SkipExportCollection(DummyAssetExporter assetExporter, NamedObject asset, string name)
		{
			if (assetExporter == null)
			{
				throw new ArgumentNullException(nameof(assetExporter));
			}
			if (asset == null)
			{
				throw new ArgumentNullException(nameof(asset));
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}

			AssetExporter = assetExporter;
			Name = name;
			m_asset = asset;
		}

		public string GetExportID(Object @object)
		{
			if (@object == m_asset)
			{
				return $"{(int)m_asset.ClassID}00000";
			}
			throw new ArgumentException(nameof(@object));
		}

		public ExportPointer CreateExportPointer(Object @object, bool isLocal)
		{
			if(isLocal)
			{
				throw new ArgumentException(nameof(isLocal));
			}
			return new ExportPointer(GetExportID(@object), UnityGUID.MissingReference, AssetExporter.ToExportType(@object.ClassID));
		}

		public IAssetExporter AssetExporter { get; }
		public IEnumerable<Object> Objects
		{
			get { yield return m_asset; }
		}
		public string Name { get; }
		public UnityGUID GUID => throw new NotSupportedException();
		public IYAMLExportable MetaImporter => throw new NotSupportedException();

		private Object m_asset;
	}
}
