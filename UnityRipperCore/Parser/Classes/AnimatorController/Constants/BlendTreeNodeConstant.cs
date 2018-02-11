using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class BlendTreeNodeConstant : IStreamReadable, IYAMLExportable
	{
		public BlendTreeNodeConstant(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;

			if(IsReadBlendData)
			{
				BlendDirectData = new OffsetPtr<BlendDirectDataConstant>(new BlendDirectDataConstant());
			}
		}

		public void Read(EndianStream stream)
		{
			BlendType = stream.ReadUInt32();
			BlendEventID = stream.ReadUInt32();
			BlendEventYID = stream.ReadUInt32();
			m_childIndices = stream.ReadUInt32Array();
			Blend1dData.Read(stream);
			Blend2dData.Read(stream);
			if(IsReadBlendData)
			{
				BlendDirectData.Read(stream);
			}

			ClipID = stream.ReadUInt32();
			if(IsReadClipIndex)
			{
				ClipIndex = stream.ReadUInt32();
			}

			Duration = stream.ReadSingle();
			CycleOffset = stream.ReadSingle();
			Mirror = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);
		}

		public YAMLNode ExportYAML()
		{
			throw new NotSupportedException();
		}

		public uint BlendType { get; private set; }
		public uint BlendEventID { get; private set; }
		public uint BlendEventYID { get; private set; }
		public IReadOnlyList<uint> ChildIndices => m_childIndices;
		public OffsetPtr<Blend1dDataConstant> Blend1dData { get; } = new OffsetPtr<Blend1dDataConstant>(new Blend1dDataConstant());
		public OffsetPtr<Blend2dDataConstant> Blend2dData { get; } = new OffsetPtr<Blend2dDataConstant>(new Blend2dDataConstant());
		public OffsetPtr<BlendDirectDataConstant> BlendDirectData { get; }
		public uint ClipID { get; private set; }
		public uint ClipIndex { get; private set; }
		public float Duration { get; private set; }
		public float CycleOffset { get; private set; }
		public bool Mirror { get; private set; }

		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		public bool IsReadBlendData => Version.IsGreaterEqual(5);
		/// <summary>
		/// Less than 5.0.0
		/// </summary>
		public bool IsReadClipIndex => Version.IsLess(5);

		private Version Version => m_assetsFile.Version;

		private readonly IAssetsFile m_assetsFile;

		private uint[] m_childIndices;

	}
}
