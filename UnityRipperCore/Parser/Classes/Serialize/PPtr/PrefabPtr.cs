using System;

namespace UnityRipper.Classes
{
	public class PrefabPtr : InnerPPtr<Prefab>
	{
		public PrefabPtr(Prefab prefab)
		{
			if (prefab == null)
			{
				throw new ArgumentNullException(nameof(prefab));
			}
			m_prefab = prefab;
		}

		protected override string PathIDString => m_prefab.ExportID;

		private readonly Prefab m_prefab;
	}
}
