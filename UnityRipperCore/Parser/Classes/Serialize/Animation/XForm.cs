using System;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class XForm : IStreamReadable, IYAMLExportable
	{
		public XForm(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;

			if(IsVector3)
			{
				S3 = new Vector3f();
				T3 = new Vector3f();
			}
			else
			{
				S4 = new Vector4f();
				T4 = new Vector4f();
			}
		}

		public void Read(EndianStream stream)
		{
			if(IsVector3)
			{
				T3.Read(stream);
			}
			else
			{
				T4.Read(stream);
			}
			Q.Read(stream);
			if (IsVector3)
			{
				S3.Read(stream);
			}
			else
			{
				S4.Read(stream);
			}
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("t", IsVector3 ? T3.ExportYAML() : T4.ExportYAML());
			node.Add("q", Q.ExportYAML());
			node.Add("s", IsVector3 ? S3.ExportYAML() : S4.ExportYAML());
			return node;
		}

		public Vector4f T4 { get; }
		public Vector3f T3 { get; }
		public Vector4f Q { get; } = new Vector4f();
		public Vector4f S4 { get; }
		public Vector3f S3 { get; }

		private Version Version => m_assetsFile.Version;

		/// <summary>
		/// 5.4.0 and greater
		/// </summary>
		public bool IsVector3 => Version.IsGreaterEqual(5, 4);

		private readonly IAssetsFile m_assetsFile;
	}
}
