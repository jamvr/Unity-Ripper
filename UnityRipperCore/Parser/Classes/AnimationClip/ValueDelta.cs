namespace UnityRipper.Classes.AnimationClips
{
	public sealed class ValueDelta : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			Start = stream.ReadSingle();
			Stop = stream.ReadSingle();
		}

		public float Start { get; private set; }
		public float Stop { get; private set; }
	}
}
