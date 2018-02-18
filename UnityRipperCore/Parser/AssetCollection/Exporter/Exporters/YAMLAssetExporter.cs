using System;
using System.Collections.Generic;
using System.IO;
using UnityRipper.Classes;
using UnityRipper.Exporter.YAML;

using Object = UnityRipper.Classes.Object;

namespace UnityRipper.AssetExporters
{
	public class YAMLAssetExporter : AssetExporter
	{
		public override IExportCollection CreateCollection(Object @object)
		{
			Component comp = @object as Component;
			if (comp != null)
			{
				@object = comp.GameObject.GetObject();
			}
			if (@object.ClassID == ClassIDType.GameObject)
			{
				GameObject go = (GameObject)@object;
				@object = go.GetRoot();
			}

			if (@object.ClassID == ClassIDType.GameObject)
			{
				GameObject go = (GameObject)@object;
				Prefab prefab = new Prefab(go);
				IEnumerable<Object> prefabContent = EnumeratePrefabContent(prefab);
				PrefabExportCollection collection = new PrefabExportCollection(this, prefab, prefabContent);
				return collection;
			}
			else
			{
				if (!@object.IsAsset)
				{
					throw new ArgumentException($"Unsupported export object type {@object.ClassID}", "@object");
				}
				
				AssetExportCollection collection = new AssetExportCollection(this, @object);
				return collection;
			}
		}

		public override bool Export(IExportCollection collection, string dirPath)
		{
			AssetExportCollection asset = (AssetExportCollection)collection;
			string subFolder = asset.Asset.ClassID.ToString();
			string subPath = Path.Combine(dirPath, subFolder);
			string fileName = GetUniqueFileName(asset.Asset, subPath);
			string filePath = Path.Combine(subPath, fileName);

			PrefabExportCollection prefab = collection as PrefabExportCollection;
			if(prefab == null)
			{
				YAMLExporter.Export(asset.Asset, filePath);
			}
			else
			{
				YAMLExporter.Export(prefab.Objects, filePath);
			}
			ExportMeta(collection, filePath);
			return true;
		}

		public override AssetType ToExportType(ClassIDType classID)
		{
			switch (classID)
			{
				case ClassIDType.Material:
				case ClassIDType.Mesh:
				case ClassIDType.AnimationClip:
				case ClassIDType.Avatar:
				case ClassIDType.AnimatorOverrideController:
					return AssetType.Serialized;

				default:
					throw new NotSupportedException();
			}
		}

		private IEnumerable<Object> EnumeratePrefabContent(Prefab prefab)
		{
			foreach(EditorExtension @object in prefab.FetchObjects())
			{
				if(@object.ClassID == ClassIDType.GameObject)
				{
					GameObject go = (GameObject)@object;
					int depth = go.GetRootDepth();
					@object.ObjectHideFlags = depth > 1 ? 1u : 0u;
				}
				else
				{
					@object.ObjectHideFlags = 1;
				}
				@object.PrefabInternal = prefab.ThisPrefab;
				yield return @object;
			}
		}
	}
}
