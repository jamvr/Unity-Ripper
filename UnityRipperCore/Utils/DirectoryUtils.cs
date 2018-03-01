using System.IO;
using System.Linq;

namespace UnityRipper
{
	public static class DirectoryUtils
	{
		public static string GetMaxIndexName(string dirPath, string fileName)
		{
			DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
			if (!dirInfo.Exists)
			{
				return fileName;
			}
			
			var files = dirInfo.EnumerateFiles().Select(t => t.Name).Where(t => t.StartsWith(fileName));
			files = files.Where(t => t.IndexOf('.') > 0).OrderByDescending(t => t);

			int length = fileName.Length;
			foreach (string existFile in files)
			{
				int dotIndex = existFile.IndexOf('.', length);
				if (length == dotIndex)
				{
					fileName = fileName + 1.ToString();
					break;
				}

				string subIndexStr = existFile.Substring(length, dotIndex - length);
				if (int.TryParse(subIndexStr, out int index))
				{
					fileName = fileName + (index + 1);
					break;
				}
			}
			return fileName;
		}
	}
}
