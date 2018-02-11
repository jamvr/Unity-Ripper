using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class StateMachineConstant : IStreamReadable, IYAMLExportable
	{
		public StateMachineConstant(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			m_stateConstantArray = stream.ReadArray(() => new OffsetPtr<StateConstant>(new StateConstant(m_assetsFile)));
			m_anyStateTransitionConstantArray = stream.ReadArray(() => new OffsetPtr<TransitionConstant>(new TransitionConstant(m_assetsFile)));
			if(IsReadConstantArray)
			{
				m_selectorStateConstantArray = stream.ReadArray(() => new OffsetPtr<SelectorStateConstant>(new SelectorStateConstant()));
			}

			DefaultState = stream.ReadUInt32();
			MotionSetCount = stream.ReadUInt32();
		}

		public YAMLNode ExportYAML()
		{
			throw new NotSupportedException();
		}

		public IReadOnlyList<OffsetPtr<StateConstant>> StateConstantArray => m_stateConstantArray;
		public IReadOnlyList<OffsetPtr<TransitionConstant>> AnyStateTransitionConstantArray => m_anyStateTransitionConstantArray;
		public IReadOnlyList<OffsetPtr<SelectorStateConstant>> SelectorStateConstantArray => m_selectorStateConstantArray;
		public uint DefaultState { get; private set; }
		public uint MotionSetCount { get; private set; }
		
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		public bool IsReadConstantArray => Version.IsGreaterEqual(5);

		private Version Version => m_assetsFile.Version;

		private OffsetPtr<StateConstant>[] m_stateConstantArray;
		private OffsetPtr<TransitionConstant>[] m_anyStateTransitionConstantArray;
		private OffsetPtr<SelectorStateConstant>[] m_selectorStateConstantArray;

		private readonly IAssetsFile m_assetsFile;
	}
}
