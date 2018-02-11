using System;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimationClips
{
	public sealed class KeyframeTpl<T> : IStreamReadable, IYAMLExportable
			where T : IStreamReadable, IYAMLExportable, new()
	{
		public KeyframeTpl(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}

			m_assetsFile = assetsFile;
		}

#warning TODO: TCB to Unity's in/out slope
		public KeyframeTpl(IAssetsFile assetsFile, float time, T value):
			this(assetsFile)
		{
			Time = time;
			Value = value;
		}

		public void Read(EndianStream stream)
		{
			Time = stream.ReadSingle();
			Value.Read(stream);
			InSlope.Read(stream);
			OutSlope.Read(stream);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.AddSerializedVersion(SerializedVersion);
			node.Add("time", Time);
			node.Add("value", Value.ExportYAML());
			node.Add("inSlope", InSlope.ExportYAML());
			node.Add("outSlope", OutSlope.ExportYAML());
			node.Add("tangentMode", 0);
			return node;
		}

		public float Time { get; private set; }
		public T Value { get; private set; } = new T();
		public T InSlope { get; } = new T();
		public T OutSlope { get; } = new T();

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
