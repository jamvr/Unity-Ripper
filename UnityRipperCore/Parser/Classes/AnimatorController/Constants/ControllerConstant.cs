using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class ControllerConstant : IStreamReadable
	{
		public ControllerConstant(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;

			Values = new OffsetPtr<ValueArrayConstant>(new ValueArrayConstant(m_assetsFile));
			DefaultValues = new OffsetPtr<ValueArray>(new ValueArray(m_assetsFile));
		}
		
		public void Read(EndianStream stream)
		{
			m_layerArray = stream.ReadArray(() => new OffsetPtr<LayerConstant>(new LayerConstant(m_assetsFile)));
			m_stateMachineArray = stream.ReadArray(() => new OffsetPtr<StateMachineConstant>(new StateMachineConstant(m_assetsFile)));
			Values.Read(stream);
			DefaultValues.Read(stream);
		}

		public IReadOnlyList<OffsetPtr<LayerConstant>> LayerArray => m_layerArray;
		public IReadOnlyList<OffsetPtr<StateMachineConstant>> StateMachineArray => m_stateMachineArray;
		public OffsetPtr<ValueArrayConstant> Values { get; }
		public OffsetPtr<ValueArray> DefaultValues { get; }

		private OffsetPtr<LayerConstant>[] m_layerArray;
		private OffsetPtr<StateMachineConstant>[] m_stateMachineArray;

		private readonly IAssetsFile m_assetsFile;
	}
}
