using System;
using UnityRipper.Classes;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.AssetExporters.Classes
{
	public class ExportPointer : IYAMLExportable
	{
		public ExportPointer(string fileID)
		{
			if(string.IsNullOrEmpty(fileID))
			{
				throw new ArgumentNullException(nameof(fileID));
			}
			FileID = fileID;
		}

		public ExportPointer(string fileID, UnityGUID guid, AssetType assetType):
			this(fileID)
		{
			if (GUID != null)
			{
				throw new ArgumentNullException(nameof(GUID));
			}
			GUID = guid;
			AssetType = assetType;
		}

		public ExportPointer(ClassIDType classID, AssetType assetType) :
			this($"{(int)classID}00000", UnityGUID.MissingReference, assetType)
		{
		}
		
		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Style = MappingStyle.Flow;
			node.Add("fileID", FileID);
			if(GUID != null)
			{
				node.Add("guid", GUID.ExportYAML());
				node.Add("type", (int)AssetType);
			}
			return node;
		}

		public static readonly ExportPointer EmptyPointer = new ExportPointer(0.ToString());

		public string FileID { get; }
		public UnityGUID GUID { get; }
		public AssetType AssetType { get; }
	}
}
