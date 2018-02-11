using System;
using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class SelectorStateConstant : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			m_transitionConstantArray = stream.ReadArray(() => new OffsetPtr<SelectorTransitionConstant>(new SelectorTransitionConstant()));
			FullPathID = stream.ReadUInt32();
			IsEntry = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);
		}

		public YAMLNode ExportYAML()
		{
			throw new NotSupportedException();
		}

		public IReadOnlyList<OffsetPtr<SelectorTransitionConstant>> TransitionConstantArray => m_transitionConstantArray;
		public uint FullPathID { get; private set; }
		public bool IsEntry { get; private set; }

		private OffsetPtr<SelectorTransitionConstant>[] m_transitionConstantArray;
	}
}
