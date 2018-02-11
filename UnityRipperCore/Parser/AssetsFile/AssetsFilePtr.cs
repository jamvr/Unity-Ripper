using System;
using System.IO;

namespace UnityRipper.AssetsFiles
{
	public struct AssetsFilePtr
	{
		public AssetsFilePtr(string fileName, string name)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentException("File name isn't set", nameof(fileName));
			}
			if (name == null)
			{
				throw new ArgumentException("Name isn't set", nameof(name));
			}

			Name = name;
			FileName = fileName;
		}

		public override string ToString()
		{
			if(!string.IsNullOrEmpty(Name))
			{
				return Name;
			}
			if(!string.IsNullOrEmpty(FileName))
			{
				return Path.GetFileName(FileName);
			}
			return base.ToString();
		}
		
		public string Name { get; }
		public string FileName { get; }
	}
}
