using System;
using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class SelectorTransitionConstant : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			Destination = stream.ReadUInt32();
			m_conditionConstantArray = stream.ReadArray(() => new OffsetPtr<ConditionConstant>(new ConditionConstant()));
		}

		public YAMLNode ExportYAML()
		{
			throw new NotSupportedException();
		}

		public uint Destination { get; private set; }
		public IReadOnlyList<OffsetPtr<ConditionConstant>> ConditionConstantArray => m_conditionConstantArray;
		
		private OffsetPtr<ConditionConstant>[] m_conditionConstantArray;
	}
}
