using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class StateConstant : IStreamReadable, IYAMLExportable
	{
		public StateConstant(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			m_transitionConstantArray = stream.ReadArray(() => new OffsetPtr<TransitionConstant>(new TransitionConstant(m_assetsFile)));
			m_blendTreeConstantIndexArray = stream.ReadInt32Array();
			if(IsReadLeafInfo)
			{
				m_leafInfoArray = stream.ReadArray<LeafInfoConstant>();
			}

			m_blendTreeConstantArray = stream.ReadArray(() => new OffsetPtr<BlendTreeConstant>(new BlendTreeConstant(m_assetsFile)));
			NameID = stream.ReadUInt32();
			PathID = stream.ReadUInt32();
			if(IsReadFullPathID)
			{
				FullPathID = stream.ReadUInt32();
			}

			TagID = stream.ReadUInt32();
			if(IsReadSpeedParam)
			{
				SpeedParamID = stream.ReadUInt32();
				MirrorParamID = stream.ReadUInt32();
				CycleOffsetParamID = stream.ReadUInt32();
			}
			if (IsReadTimeParam)
			{
				TimeParamID = stream.ReadUInt32();
			}

			Speed = stream.ReadSingle();
			CycleOffset = stream.ReadSingle();
			IKOnFeet = stream.ReadBoolean();
			if (IsReadDefaultValues)
			{
				WriteDefaultValues = stream.ReadBoolean();
			}

			Loop = stream.ReadBoolean();
			Mirror = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);
		}

		public YAMLNode ExportYAML()
		{
			throw new NotSupportedException();
		}

		public IReadOnlyList<OffsetPtr<TransitionConstant>> TransitionConstantArray => m_transitionConstantArray;
		public IReadOnlyList<int> BlendTreeConstantIndexArray => m_blendTreeConstantIndexArray;
		public IReadOnlyList<LeafInfoConstant> LeafInfoArray => m_leafInfoArray;
		public IReadOnlyList<OffsetPtr<BlendTreeConstant>> BlendTreeConstantArray => m_blendTreeConstantArray;
		public uint NameID { get; private set; }
		public uint PathID { get; private set; }
		public uint FullPathID { get; private set; }
		public uint TagID { get; private set; }
		public uint SpeedParamID { get; private set; }
		public uint MirrorParamID { get; private set; }
		public uint CycleOffsetParamID { get; private set; }
		public uint TimeParamID { get; private set; }
		public float Speed { get; private set; }
		public float CycleOffset { get; private set; }
		public bool IKOnFeet { get; private set; }
		public bool WriteDefaultValues { get; private set; }
		public bool Loop { get; private set; }
		public bool Mirror { get; private set; }
		
		/// <summary>
		/// Less than 5.0.0
		/// </summary>
		public bool IsReadLeafInfo => Version.IsLess(5);
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		public bool IsReadFullPathID => Version.IsGreaterEqual(5);
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		public bool IsReadSpeedParam => Version.IsGreaterEqual(5);
		/// <summary>
		/// 2017.2 and greater
		/// </summary>
		public bool IsReadTimeParam => Version.IsGreaterEqual(2017, 2);
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		public bool IsReadDefaultValues => Version.IsGreaterEqual(5);

		private Version Version => m_assetsFile.Version;

		private readonly IAssetsFile m_assetsFile;

		private OffsetPtr<TransitionConstant>[] m_transitionConstantArray;
		private int[] m_blendTreeConstantIndexArray;
		private LeafInfoConstant[] m_leafInfoArray;
		private OffsetPtr<BlendTreeConstant>[] m_blendTreeConstantArray;
		
	}
}
