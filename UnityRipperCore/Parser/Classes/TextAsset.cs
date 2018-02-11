using System;
using System.Text;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public class TextAsset : NamedObject
	{
		public TextAsset(AssetInfo assetInfo):
			base(assetInfo)
		{
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			Script = stream.ReadStringAligned();
			if(IsReadPath)
			{
				PathName = stream.ReadStringAligned();
			}
		}

		public override byte[] ExportBinary()
		{
			return Encoding.UTF8.GetBytes(Script);
		}

		protected sealed override YAMLMappingNode ExportYAMLRoot()
		{
			throw new NotSupportedException();
		}
		
		public override string ExportExtension => "txt";

		public string Script { get; private set; } = string.Empty;
		public string PathName { get; private set; } = string.Empty;

		/// <summary>
		/// Less than 2017.0
		/// </summary>
		private bool IsReadPath => Version.IsLess(2017);
	}
}
