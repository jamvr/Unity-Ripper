using System;
using System.Collections.Generic;
using UnityRipper.Classes;
using UnityRipper.AssetExporters.Classes;

using Object = UnityRipper.Classes.Object;

namespace UnityRipper.AssetExporters
{
	public class AssetsExporter
	{
		public AssetsExporter()
		{
			DummyAssetExporter dummyExporter = new DummyAssetExporter();
			OverrideExporter(ClassIDType.AnimatorController, dummyExporter);
			OverrideExporter(ClassIDType.MonoScript, dummyExporter);
			OverrideExporter(ClassIDType.AssetBundle, dummyExporter);

			YAMLAssetExporter yamlExporter = new YAMLAssetExporter();
			OverrideExporter(ClassIDType.Prefab, yamlExporter);
			OverrideExporter(ClassIDType.Component, yamlExporter);
			OverrideExporter(ClassIDType.GameObject, yamlExporter);
			OverrideExporter(ClassIDType.Material, yamlExporter);
			OverrideExporter(ClassIDType.Mesh, yamlExporter);
			OverrideExporter(ClassIDType.AnimationClip, yamlExporter);
			OverrideExporter(ClassIDType.Avatar, yamlExporter);
			OverrideExporter(ClassIDType.AnimatorOverrideController, yamlExporter);
			OverrideExporter(ClassIDType.AvatarMask, yamlExporter);

			BinaryAssetExporter binExporter = new BinaryAssetExporter();
			OverrideExporter(ClassIDType.Shader, binExporter);
			OverrideExporter(ClassIDType.TextAsset, binExporter);
		}

		public void OverrideExporter(ClassIDType classType, IAssetExporter exporter)
		{
			if (exporter == null)
			{
				throw new ArgumentNullException(nameof(exporter));
			}
			m_exporters[classType] = exporter;
		}
				
		public void Export(string path, Object @object)
		{
			Export(path, ToIEnumerable(@object));
		}
		
		public void Export(string path, IEnumerable<Object> objects)
		{
			if(IsExporting)
			{
				throw new InvalidOperationException("Unable to start a new export process until the old one is completed");
			}

			List<Object> deps = new List<Object>();
			deps.AddRange(objects);
			for (int i = 0; i < deps.Count; i++)
			{
				Object current = deps[i];

				if(!IsPresentInCollection(current))
				{
					ClassIDType exportID = current.IsAsset ? current.ClassID : ClassIDType.Component;
					IAssetExporter exporter = m_exporters[exportID];
					IExportCollection collection = exporter.CreateCollection(current);
					m_collections.Add(collection);
				}

#warning TODO: if IsGenerateGUIDByContent set it should build collections and write actual references with persistent GUIS, but skip dependencies
				if (Config.IsExportDependencies)
				{
					foreach (Object dep in current.FetchDependencies(true))
					{
						if (!deps.Contains(dep))
						{
							deps.Add(dep);
						}
					}
				}
			}

			foreach(IExportCollection collection in m_collections)
			{
				m_currentCollection = collection;
				collection.AssetExporter.Export(collection, path);
				Logger.Log(LogType.Info, LogCategory.Export, $"'{collection.Name}' exported");
			}

			m_currentCollection = null;
			m_collections.Clear();
		}
		
		public AssetType ToExportType(ClassIDType classID)
		{
			switch (classID)
			{
				// abstract objects
				case ClassIDType.Object:
					return AssetType.Meta;

				case ClassIDType.RuntimeAnimatorController:
					return AssetType.Serialized;

				// not supported yet
				case ClassIDType.Texture:
					return AssetType.Meta;
			}

			if(!m_exporters.ContainsKey(classID))
			{
				throw new NotImplementedException($"Export type for class {classID} is undefined");
			}

			return m_exporters[classID].ToExportType(classID);
		}

		public string GetExportID(Object @object)
		{
			if (!IsExporting)
			{
				throw new InvalidOperationException("Export pointer can only be used during export process");
			}

			foreach (IExportCollection collection in m_collections)
			{
				foreach (Object element in collection.Objects)
				{
					if (element == @object)
					{
						return collection.GetExportID(@object);
					}
				}
			}

			if(Config.IsExportDependencies)
			{
				throw new InvalidOperationException($"Object {@object} wasn't found in any export collection");
			}
			else
			{
				return AssetExportCollection.GetMainExportID(@object);
			}
		}

		public ExportPointer CreateExportPointer(Object @object)
		{
			if(!IsExporting)
			{
				throw new InvalidOperationException("Export pointer could be used only during export process");
			}

			foreach(IExportCollection collection in m_collections)
			{
				foreach(Object element in collection.Objects)
				{
					if(element == @object)
					{
						return collection.CreateExportPointer(element, collection == m_currentCollection);
					}
				}
			}

			if (Config.IsExportDependencies)
			{
				throw new InvalidOperationException($"Object {@object} wasn't found in any export collection");
			}
			else
			{
				string exportID = AssetExportCollection.GetMainExportID(@object);
				return new ExportPointer(exportID, UnityGUID.MissingReference, AssetType.Meta);
			}
		}

		private IEnumerable<Object> ToIEnumerable(Object @object)
		{
			yield return @object;
		}

		private bool IsPresentInCollection(Object @object)
		{
			foreach(IExportCollection collection in m_collections)
			{
				foreach(Object element in collection.Objects)
				{
					if(element == @object)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool IsExporting => m_currentCollection != null;

		private readonly Dictionary<ClassIDType, IAssetExporter> m_exporters = new Dictionary<ClassIDType, IAssetExporter>();
		private readonly List<IExportCollection> m_collections = new List<IExportCollection>();

		private IExportCollection m_currentCollection = null;
	}
}
