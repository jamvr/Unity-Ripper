namespace UnityRipper.Exporter.YAML
{
	public struct YAMLTag
	{
		public YAMLTag(string handle, string content)
		{
			Handle = handle;
			Content = content;
		}

		public override string ToString()
		{
			if(IsEmpty)
			{
				return string.Empty;
			}
			return $"{Handle}{Content}";
		}

		public string ToHeaderString()
		{
			if(IsEmpty)
			{
				return string.Empty;
			}
			return $"{Handle} {Content}";
		}

		public bool IsEmpty
		{
			get { return string.IsNullOrEmpty(Handle); }
		}

		public string Handle { get; }
		public string Content { get; }
	}
}
