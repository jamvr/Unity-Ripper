using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Materials
{
	public sealed class UnityPropertySheet : IStreamReadable, IYAMLExportable
	{
		public UnityPropertySheet(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			m_texEnvs.Read(stream, () => new FastPropertyName(m_assetsFile), () => new UnityTexEnv(m_assetsFile));
			m_floats.Read(stream, () => new FastPropertyName(m_assetsFile));
			m_colors.Read(stream, () => new FastPropertyName(m_assetsFile));
		}

		public void Reset()
		{
			m_texEnvs.Clear();
			m_floats.Clear();
			m_colors.Clear();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.AddSerializedVersion(SerializedVersion);
			node.Add("m_TexEnvs", m_texEnvs.ExportYAML());
			node.Add("m_Floats", m_floats.ExportYAML());
			node.Add("m_Colors", m_colors.ExportYAML());
			return node;
		}

		public IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach(UnityTexEnv env in m_texEnvs.Values)
			{
				foreach(Object @object in env.FetchDependencies(isLog))
				{
					yield return @object;
				}
			}
		}

		public IReadOnlyDictionary<FastPropertyName, UnityTexEnv> TexEnvs => m_texEnvs;
		public IReadOnlyDictionary<FastPropertyName, float> Floats => m_floats;
		public IReadOnlyDictionary<FastPropertyName, Color> Colors => m_colors;

		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 3;
				/*if(m_version.IsGreaterEqual(2017, 3))
				{
					return 3;
				}
				else
				{
					return 2;
				}*/
			}
		}
		
		private readonly Dictionary<FastPropertyName, UnityTexEnv> m_texEnvs = new Dictionary<FastPropertyName, UnityTexEnv>();
		private readonly Dictionary<FastPropertyName, float> m_floats = new Dictionary<FastPropertyName, float>();
		private readonly Dictionary<FastPropertyName, Color> m_colors = new Dictionary<FastPropertyName, Color>();

		private readonly IAssetsFile m_assetsFile;
	}
}
