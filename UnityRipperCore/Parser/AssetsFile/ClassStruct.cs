using System;

namespace UnityRipper.AssetsFiles
{
	public class ClassStruct
	{
		public override string ToString()
		{
			return $"{BaseType} {BaseName}";
		}

		public int ClassID
		{
			get { return m_classID; }
			set { m_classID = value; }
		}
		public string BaseType
		{
			get { return m_baseType; }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException($"Invalid class base type {value}");
				}
				m_baseType = value;
			}
		}
		public string BaseName
		{
			get { return m_baseName; }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException($"Invalid class base name {value}");
				}
				m_baseName = value;
			}
		}
		public ClassMember[] Members
		{
			get { return m_members; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException($"Members for class struct arn't set");
				}
				m_members = value;
			}
		}

		private int m_classID = 0;
		private string m_baseType = null;
		private string m_baseName = null;
		private ClassMember[] m_members = null;
	}
}
