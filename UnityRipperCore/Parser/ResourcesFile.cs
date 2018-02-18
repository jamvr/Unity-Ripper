using System;
using System.Collections.Generic;

namespace UnityRipper
{
	public class ResourcesFile
	{
		public ResourcesFile(string filePath, string fileName, byte[] data)
		{
			if(string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException(nameof(fileName));
			}
			if(data == null || data.Length == 0)
			{
				throw new ArgumentNullException(nameof(data));
			}
			Name = fileName;
			Data = data;
		}

		/// <summary>
		/// Сontainer file path (asset bundle or resources file itself)
		/// </summary>
		public string FilePath { get; private set; }
		/// <summary>
		/// Name of resources file in file system or in asset bundle
		/// </summary>
		public string Name { get; private set; }
		public byte[] Data { get; }
	}
}
