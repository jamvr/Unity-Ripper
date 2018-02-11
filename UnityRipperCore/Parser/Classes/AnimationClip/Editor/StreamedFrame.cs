using System.Collections.Generic;

namespace UnityRipper.Classes.AnimationClips.Editor
{
	public class StreamedFrame : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			Time = stream.ReadSingle();
			m_curves = stream.ReadArray<StreamedCurveKey>();
		}

		public float Time { get; set; }
		public IReadOnlyList<StreamedCurveKey> Curves => m_curves;

		private StreamedCurveKey[] m_curves;
	}
}
