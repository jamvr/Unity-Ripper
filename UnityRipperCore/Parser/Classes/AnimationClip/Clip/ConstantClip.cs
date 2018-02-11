using System.Collections.Generic;

namespace UnityRipper.Classes.AnimationClips
{
	public sealed class ConstantClip
	{
		public void Read(EndianStream stream)
		{
			m_constants = stream.ReadSingleArray();
		}
		
		public bool IsValid => Constants.Count > 0;

		public IReadOnlyList<float> Constants => m_constants;

		private float[] m_constants;
	}
}
