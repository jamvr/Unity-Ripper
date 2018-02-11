using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityRipper.AssetsFiles;

namespace UnityRipper
{
	class Program
	{
		public static void Main(string[] args)
		{
			Logger.Instance = ConsoleLogger.Instance;
			Config.IsAdvanceLog = true;
			Config.IsGenerateGUIDByContent = false;
			Config.IsExportDependencies = true;

			if (args.Length == 0)
			{
				Console.WriteLine("No arguments");
				Console.ReadKey();
				return;
			}
			foreach(string arg in args)
			{
				if(!File.Exists(arg))
				{
					Console.WriteLine($"File {arg} doesn't exist");
					Console.ReadKey();
					return;
				}
			}

			try
			{
				AssetCollection collection = new AssetCollection();
				collection.Load(args);

				HashSet<string> directories = new HashSet<string>();
				foreach (string filePath in args)
				{
					string dirPath = Path.GetDirectoryName(filePath);
					directories.Add(dirPath);
				}

				HashSet<string> precessed = new HashSet<string>();
				foreach (IAssetsFile assetsFile in collection.AssetsFiles)
				{
					precessed.Add(assetsFile.Name);
				}

				for (int i = 0; i < collection.AssetsFiles.Count; i++)
				{
					IAssetsFile assetsFile = collection.AssetsFiles[i];
					foreach (AssetsFilePtr ptr in assetsFile.Dependencies)
					{
						string fileName = ptr.FileName;
						if (precessed.Contains(fileName))
						{
							continue;
						}
						precessed.Add(fileName);

						bool added = false;
						foreach (string dirPath in directories)
						{
							string filePath = Path.Combine(dirPath, fileName);
							if (File.Exists(filePath))
							{
								try
								{
									collection.Load(filePath);
									foreach (IAssetsFile cur in collection.AssetsFiles)
									{
										// not sure is this necessary?
										precessed.Add(cur.Name);
									}
									added = true;
									Logger.Instance.Log(LogType.Info, LogCategory.Import, $"Dependency '{filePath}' loaded");
									break;
								}
								catch
								{
									Logger.Instance.Log(LogType.Error, LogCategory.Import, $"Can't parse dependency file {filePath}");
								}
							}
						}

						if(!added)
						{
							Logger.Instance.Log(LogType.Warning, LogCategory.Import, $"Dependency '{fileName}' wasn't found");
						}
					}
				}

				string name = Path.GetFileNameWithoutExtension(args.First());
				string exportPath = ".\\Ripped\\" + name;
				if (Directory.Exists(exportPath))
				{
					Directory.Delete(exportPath, true);
				}
				collection.Exporter.Export(exportPath, collection.FetchObjects());

				Logger.Instance.Log(LogType.Info, LogCategory.General, "Finished");
			}
			catch(Exception ex)
			{
				Logger.Instance.Log(LogType.Error, LogCategory.General, ex.ToString());
			}
			Console.ReadKey();
		}
	}
}
