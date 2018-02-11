namespace UnityRipper.Classes.AnimationClips.Editor
{
	public class StreamedCurveKey : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			Index = stream.ReadInt32();
			TCB.Read(stream);
			Value = stream.ReadSingle();
		}

		public int Index { get; private set; }
		public Vector3f TCB { get; } = new Vector3f();
		public float Value { get; private set; }
	}
}
