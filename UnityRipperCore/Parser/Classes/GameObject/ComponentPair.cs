using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.GameObjects
{
	public sealed class ComponentPair : IStreamReadable, IYAMLExportable
	{
		public ComponentPair(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;

			Component = new PPtr<Component>(assetsFile);
		}

		public void Read(EndianStream stream)
		{
			if(IsReadClassID)
			{
				ClassID = (ClassIDType)stream.ReadInt32();
			}
			Component.Read(stream);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
#warning TODO: ClassID?
			node.Add("component", Component.ExportYAML());
			return node;
		}

		public IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			Component comp = Component.FindObject();
			if(comp == null)
			{
				if(isLog)
				{
					AssetInfo assetInfo = m_assetsFile.GetAssetInfo(Component.PathID);
					Logger.Log(LogType.Debug, LogCategory.Export, $"GameObject's component {Component}[{assetInfo.ClassMap.IDType}] doesn't implemeneted;");
				}
			}
			else
			{
				yield return comp;
			}
		}
		
		public ClassIDType ClassID { get; private set; }
		public PPtr<Component> Component { get; }

		/// <summary>
		/// Less than 5.5.0
		/// </summary>
		public bool IsReadClassID => Version.IsLess(5, 5);

		private Version Version => m_assetsFile.Version;

		private IAssetsFile m_assetsFile;
	}
}
