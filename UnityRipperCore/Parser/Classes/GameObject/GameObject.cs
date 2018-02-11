using System;
using System.Collections.Generic;
using UnityRipper.Classes.GameObjects;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class GameObject : EditorExtension
	{
		public GameObject(AssetInfo assetInfo):
			base(assetInfo)
		{
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			Components = stream.ReadArray(() => new ComponentPair(AssetsFile));

			Layer = stream.ReadInt32();
			Name = stream.ReadStringAligned();
			Tag = stream.ReadUInt16();
			IsActive = stream.ReadBoolean();
		}
		
		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach (Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}
			foreach(ComponentPair pair in Components)
			{
				foreach (Object @object in pair.FetchDependencies(isLog))
				{
					yield return @object;
				}
			}
		}

		public GameObject GetRoot()
		{
			Transform root = Transform;
			while (true)
			{
				Transform parent = root.Father.TryGetObject();
				if (parent == null)
				{
					break;
				}
				else
				{
					root = parent;
				}
			}
			return root.GameObject.GetObject();
		}

		public int GetRootDepth()
		{
			Transform root = Transform;
			int depth = 0;
			while (true)
			{
				Transform parent = root.Father.TryGetObject();
				if (parent == null)
				{
					break;
				}

				root = parent;
				depth++;
			}
			return depth;
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.AddSerializedVersion(SerializedVersion);
			node.Add("m_Component", Components.ExportYAML());
			node.Add("m_Layer", Layer);
			node.Add("m_Name", Name);
#warning TODO: tag index to string name
			node.Add("m_TagString", "Untagged");
#warning what are those 3 params???
			node.Add("m_Icon", PPtr<Object>.Empty.ExportYAML());
			node.Add("m_NavMeshLayer", 0);
			node.Add("m_StaticEditorFlags", 0);
			node.Add("m_IsActive", IsActive);
			return node;
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(Name))
			{
				return base.ToString();
			}
			return $"{Name}({GetType().Name})";
		}
		
		public override string ExportExtension => throw new NotSupportedException();

		public Transform Transform
		{
			get
			{
				foreach(ComponentPair pair in Components)
				{
					Component comp = pair.Component.FindObject();
					if(comp == null)
					{
						continue;
					}

					if(comp.ClassID.IsTransform())
					{
						return (Transform)comp;
					}
				}
				return null;
			}
		}
		
		public ComponentPair[] Components { get; private set; }
		public int Layer { get; private set; }
		public string Name { get; private set; } = string.Empty;
		public ushort Tag { get; private set; }
		public bool IsActive { get; private set; }

		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 5;
			}
		}
	}
}
