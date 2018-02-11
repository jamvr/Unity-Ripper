using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Prefabs
{
	public sealed class PrefabModification : IStreamReadable, IYAMLExportable
	{
		public PrefabModification(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new NullReferenceException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;

			TransformParent = new PPtr<Transform>(assetsFile);
		}

		public void Read(EndianStream stream)
		{
			TransformParent.Read(stream);
			m_modifications = stream.ReadArray(() => new PropertyModification(m_assetsFile));
			m_removedComponents = stream.ReadArray(() => new PPtr<Object>(m_assetsFile));
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_TransformParent", TransformParent.ExportYAML());
			node.Add("m_Modifications", Modifications.ExportYAML());
			node.Add("m_RemovedComponents", RemovedComponents.ExportYAML());
			return node;
		}
		
		public PPtr<Transform> TransformParent { get; }
		public IReadOnlyList<PropertyModification> Modifications => m_modifications;
		public IReadOnlyList<PPtr<Object>> RemovedComponents => m_removedComponents;

		private readonly IAssetsFile m_assetsFile;

		private PropertyModification[] m_modifications = new PropertyModification[0];
		private PPtr<Object>[] m_removedComponents = new PPtr<Object>[0];
	}
}
