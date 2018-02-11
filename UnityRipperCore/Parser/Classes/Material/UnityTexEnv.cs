using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Materials
{
	public sealed class UnityTexEnv : IStreamReadable, IYAMLExportable
	{
		public UnityTexEnv(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}

			Texture = new PPtr<Texture>(assetsFile);
		}

		public void Read(EndianStream stream)
		{
			Texture.Read(stream);
			Scale.Read(stream);
			Offset.Read(stream);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_Texture", Texture.ExportYAML());
			node.Add("m_Scale", Scale.ExportYAML());
			node.Add("m_Offset", Offset.ExportYAML());
			return node;
		}

		public IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			Texture texture = Texture.FindObject();
			if (texture != null)
			{
				yield return texture;
			}
		}

		public PPtr<Texture> Texture { get; }
		public Vector2f Scale { get; } = new Vector2f();
		public Vector2f Offset { get; } = new Vector2f();
	}
}
