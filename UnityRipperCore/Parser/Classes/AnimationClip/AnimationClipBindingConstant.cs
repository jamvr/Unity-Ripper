using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimationClips
{
	public sealed class AnimationClipBindingConstant : IStreamReadable, IYAMLExportable
	{
		public AnimationClipBindingConstant(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			m_genericBindings = stream.ReadArray(() => new GenericBinding(m_assetsFile));
			stream.AlignStream(AlignType.Align4);

			m_pptrCurveMapping = stream.ReadArray(() => new PPtr<Object>(m_assetsFile));
			stream.AlignStream(AlignType.Align4);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("genericBindings", GenericBindings.ExportYAML());
			node.Add("pptrCurveMapping", PptrCurveMapping.ExportYAML());
			return node;
		}

		public IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach (PPtr<Object> ptr in m_pptrCurveMapping)
			{
				Object @object = ptr.FindObject();
				if(@object == null)
				{
					if(isLog)
					{
						Logger.Log(LogType.Warning, LogCategory.Export, $"AnimationClipBindingConstant's pptrCurveMapping {ptr.ToLogString()} wasn't found ");
					}
				}
				else
				{
					yield return @object;
				}
			}
		}

		public GenericBinding FindBinding(int index)
		{
			int curves = 0;
			for (int i = 0; i < GenericBindings.Count; i++)
			{
				GenericBinding gb = GenericBindings[i];
				if(gb.Attribute == 2) // Quaternion
				{
					curves += 4;
				}
				else if(gb.Attribute <= 4) // Vector3
				{
					curves += 3;
				}
				else // float
				{
					curves++;
				}
				if (curves > index)
				{
					return gb;
				}
			}

			return null;
		}

		public IReadOnlyList<GenericBinding> GenericBindings => m_genericBindings;
		public IReadOnlyList<PPtr<Object>> PptrCurveMapping => m_pptrCurveMapping;
		
		private readonly IAssetsFile m_assetsFile;

		private GenericBinding[] m_genericBindings;
		private PPtr<Object>[] m_pptrCurveMapping;
	}
}
