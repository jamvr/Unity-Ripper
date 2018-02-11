using System;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimationClips
{
	public sealed class GenericBinding : IStreamReadable, IYAMLExportable
	{
		public GenericBinding(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}

			m_assetsFile = assetsFile;
			Script = new PPtr<Object>(assetsFile);
		}

		public void Read(EndianStream stream)
		{
			Path = stream.ReadUInt32();
			Attribute = stream.ReadUInt32();
			Script.Read(stream);

			if(IsIntID)
			{
				ClassID = stream.ReadInt32();
			}
			else
			{
				ClassID = stream.ReadUInt16();
			}

			CustomType = stream.ReadByte();
			IsPPtrCurve = stream.ReadByte();
			stream.AlignStream(AlignType.Align4);
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("path", Path);
			node.Add("attribute", Attribute);
			node.Add("script", Script.ExportYAML());
			node.Add("classID", ClassID);
			node.Add("customType", CustomType);
			node.Add("isPPtrCurve", IsPPtrCurve);
			return node;
		}

		public uint Path { get; private set; }
		public uint Attribute { get; private set; }
		public BindingType BindingType => (BindingType)Attribute;
		public PPtr<Object> Script { get; }
		public int ClassID { get; private set; }
		public byte CustomType { get; private set; }
		public byte IsPPtrCurve { get; private set; }

		private Version Version => m_assetsFile.Version;

		/// <summary>
		/// 5.6.0 an greater
		/// </summary>
		private bool IsIntID => Version.IsGreaterEqual(5, 6);

		private readonly IAssetsFile m_assetsFile;
	}
}
