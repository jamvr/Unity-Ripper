using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityRipper.Classes;

using Object = UnityRipper.Classes.Object;

namespace UnityRipper.AssetExporters
{
	public class PrefabExportCollection : AssetExportCollection
	{
		public PrefabExportCollection(IAssetExporter assetExporter, Prefab prefab, IEnumerable<EditorExtension> objects) :
			base(assetExporter, prefab)
		{
			foreach(EditorExtension @object in objects)
			{
				AddObject(@object);
			}
		}
		
		public override string GetExportID(Object @object)
		{
			if(@object == Asset)
			{
				return base.GetExportID(@object);
			}
			return m_exportIDs[@object];
		}

		public override IEnumerable<Object> Objects
		{
			get
			{
				foreach (Object @object in base.Objects)
				{
					yield return @object;
				}
				foreach (Object comp in m_exportIDs.Keys)
				{
					yield return comp;
				}
			}
		}

		private void AddObject(EditorExtension @object)
		{
			if(@object == null)
			{
				throw new ArgumentNullException(nameof(@object));
			}

#warning ID compatible with Unity 2017.3f1. Add support depend of current version
			string exportID;
			do
			{
				m_builder.Length = 0;
				m_builder.Append((int)@object.ClassID);
				for (int i = 0; i < 15; i++)
				{
					int number = RandomUtils.Next(0, 10);
					m_builder.Append(number);
				}
				exportID = m_builder.ToString();
			}
			while (m_exportIDs.Values.Any(t => t == exportID));

			m_exportIDs.Add(@object, exportID);
		}
		
		private readonly Dictionary<Object, string> m_exportIDs = new Dictionary<Object, string>();
		private readonly StringBuilder m_builder = new StringBuilder();
	}
}
