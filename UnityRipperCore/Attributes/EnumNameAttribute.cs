using System;

namespace UnityRipper
{
	internal class EnumNameAttribute : Attribute
	{
		public EnumNameAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; }
	}
}
