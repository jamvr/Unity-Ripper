namespace UnityRipper.Classes.AssetBundles
{
	public sealed class AssetBundleScriptInfo : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			ClassName = stream.ReadStringAligned();
			NameSpace = stream.ReadStringAligned();
			AssemblyName = stream.ReadStringAligned();
			Hash = stream.ReadUInt32();
		}

		public string ClassName { get; private set; }
		public string NameSpace { get; private set; }
		public string AssemblyName { get; private set; }
		public uint Hash { get; private set; }
	}
}
