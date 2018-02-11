using System;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Materials
{
	public sealed class FastPropertyName : IStreamReadable, IYAMLExportable
	{
		public FastPropertyName(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			Value = stream.ReadStringAligned();
		}

		public YAMLNode ExportYAML()
		{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
			//if(IsPlainString)
			{
				return new YAMLScalarNode(Value);
			}
			/*else
			{
				YAMLMappingNode node = new YAMLMappingNode();
				node.Add("name", Value);
				return node;
			}*/
		}

		public string Value { get; private set; }

		private Version Version => m_assetsFile.Version;

		/// <summary>
		/// 2017.3 and greater
		/// </summary>
		private bool IsPlainString => Version.IsGreaterEqual(2017, 3);

		private readonly IAssetsFile m_assetsFile;
	}
}
