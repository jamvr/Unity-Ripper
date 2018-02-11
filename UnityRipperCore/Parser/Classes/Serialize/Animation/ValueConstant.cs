using System;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class ValueConstant : IStreamReadable, IYAMLExportable
	{
		public ValueConstant(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}

			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			ID = stream.ReadUInt32();
			TypeID = stream.ReadUInt32();
			if(IsReadType)
			{
				Type = stream.ReadUInt32();
			}

			Index = stream.ReadUInt32();
		}

		public YAMLNode ExportYAML()
		{
#warning TODO: ExportName
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_ID", ID);
			node.Add("m_TypeID", TypeID);
			node.Add("m_Type", Type);
			node.Add("m_Index", Index);
			return node;
		}

		public uint ID { get; private set; }
		public uint TypeID { get; private set; }
		public uint Type { get; private set; }
		public uint Index { get; private set; }

		/// <summary>
		/// Less than 5.5.0
		/// </summary>
		public bool IsReadType => Version.IsLess(5, 5);

		private Version Version => m_assetsFile.Version;

		private readonly IAssetsFile m_assetsFile;
	}
}
