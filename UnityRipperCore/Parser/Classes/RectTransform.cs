namespace UnityRipper.Classes
{
	public sealed class RectTransform : Transform
	{
		public RectTransform(AssetInfo assetInfo) :
			base(assetInfo)
		{
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			AnchorMin.Read(stream);
			AnchorMax.Read(stream);
			AnchorPosition.Read(stream);
			SizeDelta.Read(stream);
			Pivot.Read(stream);
		}

		public Vector2f AnchorMin { get; } = new Vector2f();
		public Vector2f AnchorMax { get; } = new Vector2f();
		public Vector2f AnchorPosition { get; } = new Vector2f();
		public Vector2f SizeDelta { get; } = new Vector2f();
		public Vector2f Pivot { get; } = new Vector2f();
	}
}
