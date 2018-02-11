using System;

namespace UnityRipper
{
	public class Version
	{		
		public static bool operator ==(Version left, Version right)
		{
			if(ReferenceEquals(right, null))
			{
				return false;
			}

			if (left.Major != right.Major)
			{
				return false;
			}
			if (left.Minor != right.Minor)
			{
				return false;
			}
			if(left.Build != right.Build)
			{
				return false;
			}
			if (left.Type != right.Type)
			{
				return false;
			}
			if(left.TypeNumber != right.TypeNumber)
			{
				return false;
			}
			return true;
		}

		public static bool operator !=(Version left, Version right)
		{
			return !(left == right);
		}

		public static bool operator >(Version left, Version right)
		{
			if(left.Major > right.Major)
			{
				return true;
			}
			if(left.Major < right.Major)
			{
				return false;
			}

			if (left.Minor > right.Minor)
			{
				return true;
			}
			if (left.Minor < right.Minor)
			{
				return false;
			}

			if (left.Build > right.Build)
			{
				return true;
			}
			if (left.Build < right.Build)
			{
				return false;
			}

			if (left.Type > right.Type)
			{
				return true;
			}
			if (left.Type < right.Type)
			{
				return false;
			}

			if (left.TypeNumber > right.TypeNumber)
			{
				return true;
			}
			return false;
		}

		public static bool operator >=(Version left, Version right)
		{
			if (left.Major > right.Major)
			{
				return true;
			}
			if (left.Major < right.Major)
			{
				return false;
			}

			if (left.Minor > right.Minor)
			{
				return true;
			}
			if (left.Minor < right.Minor)
			{
				return false;
			}

			if (left.Build > right.Build)
			{
				return true;
			}
			if (left.Build < right.Build)
			{
				return false;
			}

			if (left.Type > right.Type)
			{
				return true;
			}
			if (left.Type < right.Type)
			{
				return false;
			}

			if (left.TypeNumber > right.TypeNumber)
			{
				return true;
			}
			if(left.TypeNumber < right.TypeNumber)
			{
				return false;
			}
			return true;
		}

		public static bool operator <(Version left, Version right)
		{
			if (left.Major < right.Major)
			{
				return true;
			}
			if (left.Major > right.Major)
			{
				return false;
			}

			if (left.Minor < right.Minor)
			{
				return true;
			}
			if (left.Minor > right.Minor)
			{
				return false;
			}

			if (left.Build < right.Build)
			{
				return true;
			}
			if (left.Build > right.Build)
			{
				return false;
			}

			if (left.Type < right.Type)
			{
				return true;
			}
			if (left.Type > right.Type)
			{
				return false;
			}

			if (left.TypeNumber < right.TypeNumber)
			{
				return true;
			}
			return false;
		}
		
		public static bool operator <=(Version left, Version right)
		{
			if (left.Major < right.Major)
			{
				return true;
			}
			if (left.Major > right.Major)
			{
				return false;
			}

			if (left.Minor < right.Minor)
			{
				return true;
			}
			if (left.Minor > right.Minor)
			{
				return false;
			}

			if (left.Build < right.Build)
			{
				return true;
			}
			if (left.Build > right.Build)
			{
				return false;
			}

			if (left.Type < right.Type)
			{
				return true;
			}
			if (left.Type > right.Type)
			{
				return false;
			}

			if (left.TypeNumber < right.TypeNumber)
			{
				return true;
			}
			if (left.TypeNumber > right.TypeNumber)
			{
				return false;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(obj, null))
			{
				return false;
			}
			if(typeof(System.Version) != obj.GetType())
			{
				return false;
			}
			return this == (Version)obj;
		}

		public override int GetHashCode()
		{
			int hash = Major.GetHashCode();
			unchecked
			{
				hash += 17 * Minor.GetHashCode();
				hash += 23 * Build.GetHashCode();
				hash += 29 * (int)Type;
				hash += 31 * TypeNumber;
			}
			return hash;
		}

		public override string ToString()
		{
			string result = $"{Major}.{Minor}";
			if(Build != 0)
			{
				result = $"{result}.{Build}";
			}
			if(Type != VersionType.Base)
			{
				result = $"{result}{Type.ToLiteral()}{TypeNumber}";
			}
			return result;
		}

		public bool IsEqual(int major)
		{
			return Major == major;
		}

		public bool IsEqual(int major, int minor)
		{
			return Major == major && Minor == minor;
		}

		public bool IsEqual(int major, int minor, int build)
		{
			return Major == major && Minor == minor && Build == build;
		}

		public bool IsEqual(int major, int minor, int build, VersionType type)
		{
			return Major == major && Minor == minor && Build == build && Type == type;
		}

		public bool IsEqual(int major, int minor, int build, VersionType type, int typeNumer)
		{
			return Major == major && Minor == minor && Build == build && Type == type && TypeNumber == typeNumer;
		}

		public bool IsEqual(string version)
		{
			Version compareVersion = new Version();
			compareVersion.Parse(version);
			return this == compareVersion;
		}

		public bool IsLess(int major)
		{
			return Major < major;
		}

		public bool IsLess(int major, int minor)
		{
			if (Major < major)
			{
				return true;
			}
			if (Major > major)
			{
				return false;
			}
			return Minor < major;
		}

		public bool IsLess(int major, int minor, int build)
		{
			if (Major < major)
			{
				return true;
			}
			if (Major > major)
			{
				return false;
			}

			if (Minor < minor)
			{
				return true;
			}
			if (Minor > minor)
			{
				return false;
			}
			return Build < build;
		}

		public bool IsLess(int major, int minor, int build, VersionType type)
		{
			if (Major < major)
			{
				return true;
			}
			if (Major > major)
			{
				return false;
			}

			if (Minor < minor)
			{
				return true;
			}
			if (Minor > minor)
			{
				return false;
			}

			if (Build < build)
			{
				return true;
			}
			if (Build > build)
			{
				return false;
			}

			return Type < type;
		}

		public bool IsLess(int major, int minor, int build, VersionType type, int typeNumber)
		{
			if (Major < major)
			{
				return true;
			}
			if (Major > major)
			{
				return false;
			}

			if (Minor < minor)
			{
				return true;
			}
			if (Minor > minor)
			{
				return false;
			}

			if (Build < build)
			{
				return true;
			}
			if (Build > build)
			{
				return false;
			}

			if (Type < type)
			{
				return true;
			}
			if (Type > type)
			{
				return false;
			}

			return TypeNumber < typeNumber;
		}

		public bool IsLess(string version)
		{
			Version compareVersion = new Version();
			compareVersion.Parse(version);
			return this < compareVersion;
		}

		public bool IsLessEqual(int major)
		{
			return Major <= major;
		}

		public bool IsLessEqual(int major, int minor)
		{
			if (Major < major)
			{
				return true;
			}
			if (Major > major)
			{
				return false;
			}
			return Minor <= minor;
		}

		public bool IsLessEqual(int major, int minor, int build)
		{
			if (Major < major)
			{
				return true;
			}
			if (Major > major)
			{
				return false;
			}

			if (Minor < minor)
			{
				return true;
			}
			if (Minor > minor)
			{
				return false;
			}
			return Build <= build;
		}

		public bool IsLessEqual(int major, int minor, int build, VersionType type)
		{
			if (Major < major)
			{
				return true;
			}
			if (Major > major)
			{
				return false;
			}

			if (Minor < minor)
			{
				return true;
			}
			if (Minor > minor)
			{
				return false;
			}

			if (Build < build)
			{
				return true;
			}
			if (Build > build)
			{
				return false;
			}

			return Type <= type;
		}

		public bool IsLessEqual(int major, int minor, int build, VersionType type, int typeNumber)
		{
			if (Major < major)
			{
				return true;
			}
			if (Major > major)
			{
				return false;
			}

			if (Minor < minor)
			{
				return true;
			}
			if (Minor > minor)
			{
				return false;
			}

			if (Build < build)
			{
				return true;
			}
			if (Build > build)
			{
				return false;
			}

			if (Type < type)
			{
				return true;
			}
			if (Type > type)
			{
				return false;
			}

			return TypeNumber <= typeNumber;
		}

		public bool IsLessEqual(string version)
		{
			Version compareVersion = new Version();
			compareVersion.Parse(version);
			return this <= compareVersion;
		}

		public bool IsGreater(int major)
		{
			return Major > major;
		}

		public bool IsGreater(int major, int minor)
		{
			if(Major > major)
			{
				return true;
			}
			if(Major < major)
			{
				return false;
			}
			return Minor > minor;
		}

		public bool IsGreater(int major, int minor, int build)
		{
			if (Major > major)
			{
				return true;
			}
			if (Major < major)
			{
				return false;
			}

			if (Minor > minor)
			{
				return true;
			}
			if (Minor < minor)
			{
				return false;
			}
			return Build > build;
		}

		public bool IsGreater(int major, int minor, int build, VersionType type)
		{
			if (Major > major)
			{
				return true;
			}
			if (Major < major)
			{
				return false;
			}

			if (Minor > minor)
			{
				return true;
			}
			if (Minor < minor)
			{
				return false;
			}

			if (Build > build)
			{
				return true;
			}
			if (Build < build)
			{
				return false;
			}

			return Type > type;
		}

		public bool IsGreater(int major, int minor, int build, VersionType type, int typeNumber)
		{
			if (Major > major)
			{
				return true;
			}
			if (Major < major)
			{
				return false;
			}

			if (Minor > minor)
			{
				return true;
			}
			if (Minor < minor)
			{
				return false;
			}

			if (Build > build)
			{
				return true;
			}
			if (Build < build)
			{
				return false;
			}

			if (Type > type)
			{
				return true;
			}
			if (Type < type)
			{
				return false;
			}

			return TypeNumber > typeNumber;
		}

		public bool IsGreater(string version)
		{
			Version compareVersion = new Version();
			compareVersion.Parse(version);
			return this > compareVersion;
		}

		public bool IsGreaterEqual(int major)
		{
			return Major >= major;
		}

		public bool IsGreaterEqual(int major, int minor)
		{
			if (Major > major)
			{
				return true;
			}
			if (Major < major)
			{
				return false;
			}
			return Minor >= minor;
		}

		public bool IsGreaterEqual(int major, int minor, int build)
		{
			if (Major > major)
			{
				return true;
			}
			if (Major < major)
			{
				return false;
			}

			if (Minor > minor)
			{
				return true;
			}
			if (Minor < minor)
			{
				return false;
			}
			return Build >= build;
		}

		public bool IsGreaterEqual(int major, int minor, int build, VersionType type)
		{
			if (Major > major)
			{
				return true;
			}
			if (Major < major)
			{
				return false;
			}

			if (Minor > minor)
			{
				return true;
			}
			if (Minor < minor)
			{
				return false;
			}

			if (Build > build)
			{
				return true;
			}
			if (Build < build)
			{
				return false;
			}

			return Type >= type;
		}

		public bool IsGreaterEqual(int major, int minor, int build, VersionType type, int typeNumber)
		{
			if (Major > major)
			{
				return true;
			}
			if (Major < major)
			{
				return false;
			}

			if (Minor > minor)
			{
				return true;
			}
			if (Minor < minor)
			{
				return false;
			}

			if (Build > build)
			{
				return true;
			}
			if (Build < build)
			{
				return false;
			}

			if (Type > type)
			{
				return true;
			}
			if (Type < type)
			{
				return false;
			}

			return TypeNumber >= typeNumber;
		}

		public bool IsGreaterEqual(string version)
		{
			Version compareVersion = new Version();
			compareVersion.Parse(version);
			return this >= compareVersion;
		}

		public void Parse(string version)
		{
			if (string.IsNullOrEmpty(version))
			{
				throw new Exception($"Invalid version number {version}");
			}

			string[] modules = version.Split(ModuleSeparator);
			if(modules.Length > 1)
			{
				// ex. - 4.7.1f1/n1
#warning TODO: wtf is that?
			}
			
			string[] parts = modules[0].Split(PartSeparator);

			int build;
			if (parts.Length == 1)
			{
				Major = ParseVersionType(parts[0]);
				return;
			}
			if (!int.TryParse(parts[0], out build))
			{
				throw new Exception($"Can't parse build part of version {version}");
			}
			Major = build;

			int major;
			if (parts.Length == 2)
			{
				Minor = ParseVersionType(parts[1]);
				return;
			}
			if(!int.TryParse(parts[1], out major))
			{
				throw new Exception($"Can't parse major part of version {version}");
			}
			Minor = major;
			
			if (parts.Length == 3)
			{
				Build = ParseVersionType(parts[2]);
				return;
			}

			throw new Exception($"Can't parse version {version}");
		}

		private int ParseVersionType(string version)
		{
			int i;
			for(i = 0; i < version.Length; i++)
			{
				char symb = version[i];
				if(symb >= '0' && symb <= '9')
				{
					continue;
				}
				break;
			}

			string sub = version.Substring(0, i);
			int isub;
			if(!int.TryParse(sub, out isub))
			{
				throw new Exception($"Can't get version and its version type from '{version}'");
			}

			if (i == version.Length)
			{
				Type = VersionType.Base;
				TypeNumber = 0;
			}
			else
			{
				char type = version[i++];
				switch(type)
				{
					case 'a':
						Type = VersionType.Alpha;
						break;

					case 'b':
						Type = VersionType.Beta;
						break;

					case 'p':
						Type = VersionType.Patch;
						break;

					case 'f':
						Type = VersionType.Final;
						break;

					default:
						throw new Exception($"Unsupported version type {type} for version '{version}'");
				}

				if(i == version.Length)
				{
					throw new Exception($"Type number wasn't found for version '{version}'");
				}

				string typeNumb = version.Substring(i);
				int typeNumber;
				if(!int.TryParse(typeNumb, out typeNumber))
				{
					throw new Exception($"Can't parse type number for version {version}");
				}

				TypeNumber = typeNumber;
			}
			return isub;
		}

		public int Major { get; private set; }
		public int Minor { get; private set; }
		public int Build { get; private set; }
		public VersionType Type { get; private set; }
		public int TypeNumber { get; private set; }

		private readonly char[] PartSeparator = new char[] { '.' };
		private readonly char[] ModuleSeparator = new char[] { '\n' };
	}
}
