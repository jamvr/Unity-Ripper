using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public abstract class EditorExtension : Object
	{
		protected EditorExtension(AssetInfo assetInfo):
			base(assetInfo)
		{
			PrefabParentObject = new PPtr<EditorExtension>(AssetsFile);
			PrefabInternal = new PPtr<Prefab>(AssetsFile);
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			if (IsReadPrefab)
			{
				PrefabParentObject.Read(stream);
				PrefabInternal.Read(stream);
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			if (IsWritePrefab)
			{
				node.Add("m_PrefabParentObject", PrefabParentObject.ExportYAML());
				node.Add("m_PrefabInternal", PrefabInternal.ExportYAML());
			}
			return node;
		}

		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}
			
			if (!PrefabParentObject.IsNull)
			{
				yield return PrefabParentObject.GetObject();
			}
			if(!PrefabInternal.IsNull)
			{
				yield return PrefabInternal.GetObject();
			}
		}
		
		public PPtr<EditorExtension> PrefabParentObject { get; }
		public IPPtr<Prefab> PrefabInternal { get; set; }

		/// <summary>
		/// Unity Package
		/// </summary>
		public bool IsReadPrefab => Platform == Platform.UnityPackage;
		private bool IsWritePrefab => ClassID != ClassIDType.Prefab;
	}
}
