using System;
using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public class Transform : Component
	{
		public Transform(AssetInfo assetInfo) :
			base(assetInfo)
		{
			Father = new PPtr<Transform>(AssetsFile);
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);
			
			LocalRotation.Read(stream);
			LocalPosition.Read(stream);
			LocalScale.Read(stream);
			m_children = stream.ReadArray(() => new PPtr<Transform>(AssetsFile));
			Father.Read(stream);
		}

		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}

			foreach (PPtr<Transform> ptr in Children)
			{
				yield return ptr.GetObject();
			}
			if (!Father.IsNull)
			{
				yield return Father.GetObject();
			}
		}

		public string GetRootPath()
		{
			string pre = string.Empty;
			if(!Father.IsNull)
			{
				pre = Father.GetObject().GetRootPath() + "/";
			}
			return pre + GameObject.GetObject().Name;
		}

		public int GetSiblingIndex()
		{
			Transform father = Father.FindObject();
			if(father == null)
			{
				return 0;
			}

			for(int i = 0; i < father.Children.Count; i++)
			{
				PPtr<Transform> child = father.Children[i];
				if (child.PathID == PathID)
				{
					return i;
				}
			}
			throw new Exception("Transorm hasn't been found among father's children");
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.Add("m_LocalRotation", LocalRotation.ExportYAML());
			node.Add("m_LocalPosition", LocalPosition.ExportYAML());
			node.Add("m_LocalScale", LocalScale.ExportYAML());
			node.Add("m_Children", Children.ExportYAML());
			node.Add("m_Father", Father.ExportYAML());
			node.Add("m_RootOrder", GetSiblingIndex());
			node.Add("m_LocalEulerAnglesHint", LocalRotation.ToEuler().ExportYAML());
			return node;
		}
		
		public Quaternionf LocalRotation { get; } = new Quaternionf();
		public Vector3f LocalPosition { get; } = new Vector3f();
		public Vector3f LocalScale { get; } = new Vector3f();
		public IReadOnlyList<PPtr<Transform>> Children => m_children;
		public PPtr<Transform> Father { get; }

		private PPtr<Transform>[] m_children;
	}
}
