using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class TransitionConstant : IStreamReadable, IYAMLExportable
	{
		public TransitionConstant(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			m_conditionConstantArray = stream.ReadArray(() => new OffsetPtr<ConditionConstant>(new ConditionConstant()));
			DestinationState = stream.ReadUInt32();
			if(IsReadPathID)
			{
				FullPathID = stream.ReadUInt32();
			}
			ID = stream.ReadUInt32();
			UserID = stream.ReadUInt32();
			TransitionDuration = stream.ReadSingle();
			TransitionOffset = stream.ReadSingle();
			if(IsReadAtomic)
			{
				Atomic = stream.ReadBoolean();
			}
			else
			{
				ExitTime = stream.ReadSingle();
				HasExitTime = stream.ReadBoolean();
				HasFixedDuration = stream.ReadBoolean();
				stream.AlignStream(AlignType.Align4);

				InterruptionSource = stream.ReadInt32();
				OrderedInterruption = stream.ReadBoolean();
			}

			CanTransitionToSelf = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);
		}

		public YAMLNode ExportYAML()
		{
			throw new NotSupportedException();
		}

		public IReadOnlyList<OffsetPtr<ConditionConstant>> ConditionConstantArray => m_conditionConstantArray;
		public uint DestinationState { get; private set; }
		public uint FullPathID { get; private set; }
		public uint ID { get; private set; }
		public uint UserID { get; private set; }
		public float TransitionDuration { get; private set; }
		public float TransitionOffset { get; private set; }
		public bool Atomic { get; private set; }
		public float ExitTime { get; private set; }
		public bool HasExitTime { get; private set; }
		public bool HasFixedDuration { get; private set; }
		public int InterruptionSource { get; private set; }
		public bool OrderedInterruption { get; private set; }
		public bool CanTransitionToSelf { get; private set; }

		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		public bool IsReadPathID => Version.IsGreaterEqual(5);
		/// <summary>
		/// Less than 5.0.0
		/// </summary>
		public bool IsReadAtomic => Version.IsLess(5);

		private OffsetPtr<ConditionConstant>[] m_conditionConstantArray;

		private Version Version => m_assetsFile.Version;

		private readonly IAssetsFile m_assetsFile;
	}
}
