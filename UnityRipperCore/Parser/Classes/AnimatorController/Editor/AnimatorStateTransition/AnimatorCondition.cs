using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers.Editor
{
	public sealed class AnimatorCondition : IYAMLExportable
	{
		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_ConditionMode", ConditionMode);
			node.Add("m_ConditionEvent", ConditionEvent);
			node.Add("m_EventTreshold", EventTreshold);
			return node;
		}

		public int ConditionMode { get; private set; }
		public string ConditionEvent { get; private set; }
		public float EventTreshold { get; private set; }
	}
}
