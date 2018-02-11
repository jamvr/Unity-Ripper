using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityRipper.AssetsFiles;
using UnityRipper.Classes.Prefabs;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class Prefab : Object
	{
		public Prefab(AssetInfo assetInfo):
			base(assetInfo)
		{
			Modification = new PrefabModification(AssetsFile);
			ParentPrefab = new PPtr<Prefab>(AssetsFile);
			RootGameObject = new PPtr<GameObject>(AssetsFile);
			ThisPrefab = new InnerPPtr<Prefab>(this);
		}

		public Prefab(GameObject root) :
			base(CreateAssetsInfo(root))
		{
			Modification = new PrefabModification(AssetsFile);
			ParentPrefab = new PPtr<Prefab>(AssetsFile);
			RootGameObject = new PPtr<GameObject>(root);
			ThisPrefab = new InnerPPtr<Prefab>(this);
			IsPrefabParent = true;
			ObjectHideFlags = 1;

			if(Config.IsGenerateGUIDByContent)
			{
				CalculatePrefabHash();
			}
		}

		private static AssetInfo CreateAssetsInfo(GameObject root)
		{
			int classID = (int)ClassIDType.Prefab;
			ClassIDMap classIDMap = new ClassIDMap(classID, classID);
			AssetInfo assetInfo = new AssetInfo(root.AssetsFile, 0, classIDMap);
			return assetInfo;
		}
		
		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			ParentPrefab.Read(stream);
			RootGameObject.Read(stream);
			IsPrefabParent = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align64);
		}

		public IEnumerable<EditorExtension> FetchObjects(bool isLog = false)
		{
			GameObject root = RootGameObject.GetObject();
			List<Object> dependencies = ObjectUtils.CollectDependencies(root, isLog);
			IEnumerable<Object> deps = dependencies.Where(t => !t.IsAsset);
#warning dependency of other prefab's component?

			IEnumerable<Object> gos = deps.Where(t => t.ClassID == ClassIDType.GameObject);
			foreach (GameObject go in gos)
			{
				yield return go;
			}
			IEnumerable<Object> comps = deps.Where(t => t.ClassID != ClassIDType.GameObject).OrderBy(t => t.ClassID);
			foreach (Component comp in comps)
			{
				yield return comp;
			}
		}

		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach (Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}

			if(!ParentPrefab.IsNull)
			{
				yield return ParentPrefab.GetObject();
			}
			yield return RootGameObject.GetObject();
		}

		public override string ToString()
		{
			return $"{Name}(Prefab)";
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.AddSerializedVersion(SerializedVersion);
			node.Add("m_Modification", Modification.ExportYAML());
			node.Add("m_ParentPrefab", ParentPrefab.ExportYAML());
			node.Add("m_RootGameObject", RootGameObject.ExportYAML());
			node.Add("m_IsPrefabParent", IsPrefabParent);
			return node;
		}

		private void CalculatePrefabHash()
		{
			List<uint> hashList = new List<uint>();
			foreach (Object @object in FetchObjects())
			{
				hashList.AddRange(@object.GUID.Data);
			}

			uint[] hashArray = hashList.ToArray();
			byte[] buffer = new byte[hashArray.Length * sizeof(uint)];
			Buffer.BlockCopy(hashArray, 0, buffer, 0, buffer.Length);
			using (MD5 md5 = MD5.Create())
			{
				byte[] hash = md5.ComputeHash(buffer);
				GUID = new UnityGUID(hash);
			}
		}

		public override string ExportExtension => "prefab";
		
		public InnerPPtr<Prefab> ThisPrefab { get; }
		public string Name => RootGameObject.GetObject().Name;

		public PrefabModification Modification { get; }
		public PPtr<Prefab> ParentPrefab { get; }
		public PPtr<GameObject> RootGameObject { get; private set; }
		public bool IsPrefabParent { get; private set; }

		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 2;
			}
		}

	}
}
