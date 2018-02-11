using System.Collections.Generic;

namespace UnityRipper.Classes
{
	public static class ObjectUtils
	{
		public static List<Object> CollectDependencies(Object @object, bool isLog = false)
		{
			List<Object> deps = new List<Object>();
			deps.Add(@object);

			for (int i = 0; i < deps.Count; i++)
			{
				foreach (Object newDep in deps[i].FetchDependencies(isLog))
				{
					if (!deps.Contains(newDep))
					{
						deps.Add(newDep);
					}
				}
			}
			
			return deps;
		}
	}
}
