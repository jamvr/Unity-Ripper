namespace UnityRipper.Exporter.YAML
{
	public abstract class YAMLNode
	{
		internal virtual void Emit(Emitter emitter)
		{
			bool isWrote = false;
			if(!CustomTag.IsEmpty)
			{
				emitter.Write(CustomTag.ToString()).WriteWhitespace();
				isWrote = true;
			}
			if (Anchor != string.Empty)
			{
				emitter.Write("&").Write(Anchor).WriteWhitespace();
				isWrote = true;
			}
			
			if(isWrote)
			{
				if(IsMultyline)
				{
					emitter.WriteLine();
				}
			}
		}

		public abstract YAMLNodeType NodeType { get; }
		public abstract bool IsMultyline { get; }
		//public abstract bool IsComplex { get; }
		public abstract bool IsIndent { get; }
		
		public string Tag
		{
			get { return CustomTag.Content; }
			set
			{
				CustomTag = new YAMLTag(YAMLWriter.DefaultTagHandle, value);
			}
		}
		public YAMLTag CustomTag { get; set; }
		public string Anchor { get; set; } = string.Empty;
	}
}
