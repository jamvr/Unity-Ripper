using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public abstract class NamedObject : EditorExtension
	{
		protected NamedObject(AssetInfo assetInfo) :
			base(assetInfo)
		{
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			Name = stream.ReadStringAligned();
			if(string.IsNullOrEmpty(Name))
			{
				Name = GetType().Name;
			}
		}

		public override string ToString()
		{
			return $"{Name}({GetType().Name})";
		}

		public override string ToLogString()
		{
			return $"{GetType().Name}'s({Name})[{PathID}]";
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode root = base.ExportYAMLRoot();
			root.Add("m_Name", Name);
			return root;
		}
		
		public string Name { get; protected set; }
	}
}
