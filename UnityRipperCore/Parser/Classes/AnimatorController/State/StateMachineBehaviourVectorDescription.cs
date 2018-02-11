using System.Collections.Generic;

namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class StateMachineBehaviourVectorDescription : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			m_stateMachineBehaviourRanges.Read(stream);
			m_stateMachineBehaviourIndices = stream.ReadUInt32Array();
		}
		
		public IReadOnlyDictionary<StateKey, StateRange> StateMachineBehaviourRanges => m_stateMachineBehaviourRanges;
		public IReadOnlyList<uint> StateMachineBehaviourIndices => m_stateMachineBehaviourIndices;

		private readonly Dictionary<StateKey, StateRange> m_stateMachineBehaviourRanges = new Dictionary<StateKey, StateRange>();

		private uint[] m_stateMachineBehaviourIndices;
	}
}
