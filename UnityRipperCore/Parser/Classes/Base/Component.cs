using System;
using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public abstract class Component : EditorExtension
	{
		protected Component(AssetInfo assetInfo) :
			base(assetInfo)
		{
			GameObject = new PPtr<GameObject>(AssetsFile);
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			GameObject.Read(stream);
		}

		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach (Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}

			yield return GameObject.GetObject();
		}

		public GameObject GetRoot()
		{
			GameObject go = GameObject.GetObject();
			return go.GetRoot();
		}

		public int GetRootDepth()
		{
			GameObject go = GameObject.GetObject();
			return go.GetRootDepth();
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.Add("m_GameObject", GameObject.ExportYAML());
			return node;
		}

		public sealed override string ExportExtension => throw new NotSupportedException();

		public PPtr<GameObject> GameObject { get; }
	}
}
