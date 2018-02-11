using System;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Avatars
{
	public sealed class Limit : IStreamReadable, IYAMLExportable
	{
		public Limit(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;

			if(IsVector3)
			{
				Min3 = new Vector3f();
				Max3 = new Vector3f();
			}
			else
			{
				Min4 = new Vector4f();
				Max4 = new Vector4f();
			}
		}

		public void Read(EndianStream stream)
		{
			if(IsVector3)
			{
				Min3.Read(stream);
				Max3.Read(stream);
			}
			else
			{
				Min4.Read(stream);
				Max4.Read(stream);
			}
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_Min", IsVector3 ? Min3.ExportYAML() : Min4.ExportYAML3());
			node.Add("m_Max", IsVector3 ? Max3.ExportYAML() : Max4.ExportYAML3());
			return node;
		}

		public Vector3f Min3 { get; }
		public Vector4f Min4 { get; }
		public Vector3f Max3 { get; }
		public Vector4f Max4 { get; }

		/// <summary>
		/// 5.4.0 and greater
		/// </summary>
		public bool IsVector3 => Version.IsGreaterEqual(5, 4);

		private Version Version => m_assetsFile.Version;

		private readonly IAssetsFile m_assetsFile;
	}
}
