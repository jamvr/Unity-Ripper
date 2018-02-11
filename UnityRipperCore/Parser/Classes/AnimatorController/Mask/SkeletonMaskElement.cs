namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class SkeletonMaskElement : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			PathHash = stream.ReadUInt32();
			Weight = stream.ReadSingle();
		}

		public uint PathHash { get; private set; }
		public float Weight { get; private set; }
	}
}
