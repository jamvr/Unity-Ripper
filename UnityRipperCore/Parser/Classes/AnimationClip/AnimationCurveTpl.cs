using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimationClips
{
	public sealed class AnimationCurveTpl<T> : IStreamReadable, IYAMLExportable
				where T : IStreamReadable, IYAMLExportable, new()
	{
		public AnimationCurveTpl(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			Reset();

			Curve.Read(stream, () => new KeyframeTpl<T>(m_assetsFile));
			stream.AlignStream(AlignType.Align4);

			PreInfinity = stream.ReadInt32();
			PostInfinity = stream.ReadInt32();
			if (IsReadRotationOrder)
			{
				RotationOrder = stream.ReadInt32();
			}
		}

		public void Reset()
		{
			Curve.Clear();
		}
		
		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.AddSerializedVersion(SerializedVersion);
			node.Add("m_Curve", Curve.ExportYAML());
			node.Add("m_PreInfinity", PreInfinity);
			node.Add("m_PostInfinity", PostInfinity);
#warning TODO: SerializedVersion
			if (IsReadRotationOrder)
			{
				node.Add("m_RotationOrder", RotationOrder);
			}
			else
			{
				node.Add("m_RotationOrder", 0);
			}

			return node;
		}

		public List<KeyframeTpl<T>> Curve { get; } = new List<KeyframeTpl<T>>();
		public int PreInfinity { get; set; }
		public int PostInfinity { get; set; }
		public int RotationOrder { get; set; }
		
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		public bool IsReadRotationOrder => Version.IsGreaterEqual(5);

		private Version Version => m_assetsFile.Version;

		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 2;
			}
		}

		private readonly IAssetsFile m_assetsFile;
	}
}
