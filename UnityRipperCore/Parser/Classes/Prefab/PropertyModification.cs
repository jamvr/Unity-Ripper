using System;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Prefabs
{
	public sealed class PropertyModification : IStreamReadable, IYAMLExportable
	{
		public PropertyModification(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}

			Target = new PPtr<Object>(assetsFile);
			ObjectReference = new PPtr<Object>(assetsFile);
		}

		public void Read(EndianStream stream)
		{
			Target.Read(stream);
			PropertyPath = stream.ReadStringAligned();
			Value = stream.ReadStringAligned();
			ObjectReference.Read(stream);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("target", Target.ExportYAML());
			node.Add("propertyPath", PropertyPath);
			node.Add("value", Value);
			node.Add("objectReference", ObjectReference.ExportYAML());
			return node;
		}

		public PPtr<Object> Target { get; }
		public string PropertyPath { get; private set; }
		public string Value { get; private set; }
		public PPtr<Object> ObjectReference { get; }
	}
}
