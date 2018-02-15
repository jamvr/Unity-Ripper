using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityRipper.AssetsFiles;

using Object = UnityRipper.Classes.Object;

namespace UnityRipper
{
	public class Program
	{
		public static IEnumerable<Object> FetchExportObjects(AssetCollection collection)
		{
			//yield break;
			return collection.FetchObjects()
				.Where(t => t.AssetsFile.Name == "29ae6f85a6c2d4e9fa563e182a4b73bd")
				.Where(t => t.ClassID == ClassIDType.Material);
		}

		public static void Main(string[] args)
		{
			Logger.Instance = ConsoleLogger.Instance;
			Config.IsAdvancedLog = true;
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
				
				LoadDependencies(collection, args);
				ValidateCollection(collection);

				string name = Path.GetFileNameWithoutExtension(args.First());
				string exportPath = ".\\Ripped\\" + name;
				if (Directory.Exists(exportPath))
				{
					Directory.Delete(exportPath, true);
				}
				collection.Exporter.Export(exportPath, FetchExportObjects(collection));

				Logger.Instance.Log(LogType.Info, LogCategory.General, "Finished");
			}
			catch(Exception ex)
			{
				Logger.Instance.Log(LogType.Error, LogCategory.General, ex.ToString());
			}
			Console.ReadKey();
		}

		private static void LoadDependencies(AssetCollection collection, IEnumerable<string> files)
		{
			HashSet<string> directories = new HashSet<string>();
			foreach (string filePath in files)
			{
				string dirPath = Path.GetDirectoryName(filePath);
				directories.Add(dirPath);
			}

			HashSet<string> processed = new HashSet<string>();
			foreach (IAssetsFile assetsFile in collection.AssetsFiles)
			{
				processed.Add(assetsFile.Name);
			}

			for (int i = 0; i < collection.AssetsFiles.Count; i++)
			{
				IAssetsFile assetsFile = collection.AssetsFiles[i];
				foreach (AssetsFilePtr ptr in assetsFile.Dependencies)
				{
					string fileName = ptr.FileName;
					if (processed.Contains(fileName))
					{
						continue;
					}
					processed.Add(fileName);

					bool found = false;
					foreach (string loadName in FetchNameVariants(fileName))
					{
						found = TryLoadDependency(collection, directories, fileName, loadName);
						if (found)
						{
							// not sure is this necessary?
							foreach (IAssetsFile cur in collection.AssetsFiles)
							{
								processed.Add(cur.Name);
							}

							break;
						}
					}
					if(!found)
					{
						Logger.Instance.Log(LogType.Warning, LogCategory.Import, $"Dependency '{fileName}' wasn't found");
					}
				}
			}
		}

		private static void ValidateCollection(AssetCollection collection)
		{
			IEnumerable<Version> versions = collection.AssetsFiles.Select(t => t.Version).Distinct();
			if(versions.Count() > 1)
			{
				Logger.Instance.Log(LogType.Warning, LogCategory.Import, $"Asset collection has incompatible (possible) with each assets file versions. Here they are:");
				foreach(Version version in versions)
				{
					Logger.Instance.Log(LogType.Warning, LogCategory.Import, version.ToString());
				}
			}
		}

		private static bool TryLoadDependency(AssetCollection collection, IEnumerable<string> directories, string originalName, string loadName)
		{
			foreach (string dirPath in directories)
			{
				string filePath = Path.Combine(dirPath, loadName);
				if (!File.Exists(filePath))
				{
					continue;
				}

				try
				{
					collection.LoadAssetsFile(filePath, originalName);
					Logger.Instance.Log(LogType.Info, LogCategory.Import, $"Dependency '{filePath}' loaded");
				}
				catch (Exception ex)
				{
					Logger.Instance.Log(LogType.Error, LogCategory.Import, $"Can't parse dependency file {filePath}");
					Logger.Instance.Log(LogType.Error, LogCategory.Debug, ex.ToString());
				}
				return true;
			}
			return false;
		}

		private static IEnumerable<string> FetchNameVariants(string name)
		{
			yield return name;

			const string libraryFolder = "library";
			if (name.ToLower().StartsWith(libraryFolder))
			{
				string fixedName = name.Substring(libraryFolder.Length + 1);
				yield return fixedName;
			}
		}
	}
}
