using System;

namespace UnityRipper.AssetsFiles
{
	public class ClassMember
	{
		public override string ToString()
		{
			return $"{Type} {Name}";
		}

		public int Level { get; set; }
		public string Type
		{
			get { return m_type; }
			set
			{
				if(string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("Invalid class memeber type");
				}
				m_type = value;
			}
		}
		public string Name
		{
			get { return m_name; }
			set
			{
				if(string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("Invalid class memeber name");
				}
				m_name = value;
			}
		}
		public int Size
		{
			get { return m_size; }
			set
			{
				if (value < -1)
				{
					throw new ArgumentException($"Invalid class memeber size {value}");
				}
				m_size = value;
			}
		}
		public int Index
		{
			get { return m_index; }
			set
			{
				if (value < 0)
				{
					throw new ArgumentException($"Invalid class memeber index {value}");
				}
				m_index = value;
			}
		}
		public bool IsArray
		{
			get { return m_isArray; }
			set { m_isArray = value; }
		}
		public int Num0
		{
			get { return m_num0; }
			set
			{
				if (value < 0)
				{
					throw new ArgumentException($"Invalid class memeber num0 value {value}");
				}
				m_num0 = value;
			}
		}
		public int Flag
		{
			get { return m_flag; }
			set
			{
				if (value < 0)
				{
					throw new ArgumentException($"Invalid class memeber flag {value}");
				}
				m_flag = value;
			}
		}
		public int ChildCount
		{
			get { return m_childCount; }
			set
			{
				if (value < 0)
				{
					throw new ArgumentException($"Invalid class memeber child count {value}");
				}
				m_childCount = value;
			}
		}

		public const int ClassSize = 24;

		private string m_type = null;
		private string m_name = null;
		private int m_size = 0;
		private int m_index = 0;
		private bool m_isArray = false;
		private int m_num0 = 0;
		private int m_flag = 0;
		private int m_childCount = 0;
	}
}
