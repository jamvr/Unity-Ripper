using System;
using System.Collections.Generic;
using System.Linq;
using UnityRipper.Classes.AssetBundles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class AssetBundle : NamedObject
	{
		public AssetBundle(AssetInfo assetInfo):
			base(assetInfo)
		{
			MainAsset = new AssetBundles.AssetInfo(AssetsFile);
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			m_preloadTable = stream.ReadArray(() => new PPtr<Object>(AssetsFile));
			m_container = stream.ReadStringKVPArray(() => new AssetBundles.AssetInfo(AssetsFile));
			MainAsset.Read(stream);
			if(IsReadScriptCampatibility)
			{
				m_scriptCampatibility = stream.ReadArray<AssetBundleScriptInfo>();
				m_classCampatibility = stream.ReadInt32KVPUInt32Array();
			}
			RuntimeCompatibility = stream.ReadUInt32();
			if(IsReadAssetBundleName)
			{
				AssetBundleName = stream.ReadStringAligned();
				m_dependencies = stream.ReadStringArray();
				IsStreamedSceneAssetBundle = stream.ReadBoolean();
				stream.AlignStream(AlignType.Align4);

				PathFlags = stream.ReadInt32();
			}
		}

		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}
			foreach (AssetBundles.AssetInfo container in m_container.Select(t => t.Value))
			{
				foreach (Object @object in container.FetchDependencies(isLog))
				{
					yield return @object;
				}
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			throw new NotSupportedException();
		}

		public override string ExportExtension => throw new NotSupportedException();

		public IReadOnlyList<PPtr<Object>> PreloadTable => m_preloadTable;
		public ILookup<string, AssetBundles.AssetInfo> Container => m_container.ToLookup(t => t.Key, t => t.Value);
		public AssetBundles.AssetInfo MainAsset { get; }
		public IReadOnlyList<AssetBundleScriptInfo> ScriptCampatibility => m_scriptCampatibility;
		public IReadOnlyList<KeyValuePair<int, uint>> ClassCampatibility => m_classCampatibility;
		public uint RuntimeCompatibility { get; private set; }
		public string AssetBundleName { get; private set; }
		public IReadOnlyList<string> Dependencies  => m_dependencies;
		public bool IsStreamedSceneAssetBundle { get; private set; }
		public int PathFlags { get; private set; }

		/// <summary>
		/// Less than 5.0.0
		/// </summary>
		public bool IsReadScriptCampatibility => Version.IsLess(5);
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		public bool IsReadAssetBundleName => Version.IsGreaterEqual(5);

		private PPtr<Object>[] m_preloadTable;
		private KeyValuePair<string, AssetBundles.AssetInfo>[] m_container;
		private AssetBundleScriptInfo[] m_scriptCampatibility;
		private KeyValuePair<int, uint>[] m_classCampatibility;
		private string[] m_dependencies;

	}
}
