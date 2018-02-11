using System;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class ConditionConstant : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			ConditionMode = stream.ReadUInt32();
			EventID = stream.ReadUInt32();
			EventThreshold = stream.ReadSingle();
			ExitTime = stream.ReadSingle();
		}

		public YAMLNode ExportYAML()
		{
			throw new NotSupportedException();
		}

		public uint ConditionMode { get; private set; }
		public uint EventID { get; private set; }
		public float EventThreshold { get; private set; }
		public float ExitTime { get; private set; }
	}
}
