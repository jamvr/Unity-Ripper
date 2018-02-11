using System;

namespace UnityRipper
{
	public enum VersionType
	{
		Alpha = 0,
		Beta,
		Base,
		Patch,
		Final,
	}

	public static class VersionTypeExtentions
	{
		public static string ToLiteral(this VersionType _this)
		{
			switch(_this)
			{
				case VersionType.Alpha:
					return "a";

				case VersionType.Beta:
					return "b";

				case VersionType.Base:
					return string.Empty;
					
				case VersionType.Patch:
					return "p";

				case VersionType.Final:
					return "f";

				default:
					throw new Exception($"Unsupported vertion type {_this}");
			}
		}
	}
}
