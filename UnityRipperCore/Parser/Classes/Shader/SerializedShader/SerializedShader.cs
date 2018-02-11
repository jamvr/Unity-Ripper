using System.Collections.Generic;

namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedShader : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			PropInfo.Read(stream);
			m_subShaders = stream.ReadArray<SerializedSubShader>();
			Name = stream.ReadStringAligned();
			CustomEditorName = stream.ReadStringAligned();
			FallbackName = stream.ReadStringAligned();
			m_dependencies = stream.ReadArray<SerializedShaderDependency>();
			DisableNoSubshadersMessage = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);
		}

		public SerializedProperties PropInfo { get; } = new SerializedProperties();
		public IReadOnlyList<SerializedSubShader> SubShaders => m_subShaders;
		public string Name { get; private set; }
		public string CustomEditorName { get; private set; }
		public string FallbackName { get; private set; }
		public IReadOnlyList<SerializedShaderDependency> Dependencies => m_dependencies;
		public bool DisableNoSubshadersMessage { get; private set; }

		private SerializedSubShader[] m_subShaders;
		private SerializedShaderDependency[] m_dependencies;
	}
}
