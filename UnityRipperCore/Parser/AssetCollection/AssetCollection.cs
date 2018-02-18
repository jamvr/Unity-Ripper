using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityRipper.AssetsFiles;
using UnityRipper.Bundles;
using UnityRipper.Classes;
using UnityRipper.AssetExporters;
using UnityRipper.AssetExporters.Classes;

using Object = UnityRipper.Classes.Object;

namespace UnityRipper
{
	public class AssetCollection : IAssetCollection
	{
		public void Load(string filePath)
		{
			if (BundleFile.IsBundleFile(filePath))
			{
				LoadAssetBundle(filePath);
			}
			else
			{
				string fileName = Path.GetFileName(filePath);
				LoadAssetsFile(filePath, fileName);
			}
		}

		public void LoadAssetBundle(string filePath)
		{
			BundleFile bundle = new BundleFile();
			using (AssetsFileData fileData = bundle.Load(filePath))
			{
				foreach (AssetsFileEntry entry in fileData.AssetsEntries)
				{
					AssetsFile assetsFile = entry.ParseAssetsFile(this, filePath);
					AddAsset(assetsFile);
				}
				foreach (AssetsFileEntry entry in fileData.ResourceEntries)
				{
					ResourcesFile resesFile = entry.ParseResourcesFile(filePath);
					m_resources.Add(resesFile);
				}
			}
		}

		public void LoadAssetsFile(string filePath, string fileName)
		{
			AssetsFile assetsFile = new AssetsFile(this, filePath, fileName);
			assetsFile.Load(filePath);
			AddAsset(assetsFile);
		}

		public void Load(IReadOnlyCollection<string> filePathes)
		{
			foreach(string file in filePathes)
			{
				Load(file);
			}
		}

		public void Unload(string filepath)
		{
			for(int i = 0; i > m_files.Count; i++)
			{
				AssetsFile file = m_files[i];
				if(file.FilePath == filepath)
				{
					m_files.RemoveAt(i);
					m_ifiles.RemoveAt(i);
					i--;
				}
			}
		}
		
		public IAssetsFile GetAssetsFile(AssetsFilePtr ptr)
		{
			IAssetsFile file = FindAssetsFile(ptr);
			if (file == null)
			{
				throw new Exception($"AssetsFile with Name {ptr.Name} and FileName {ptr.FileName} was not found");
			}
			return file;
		}

		public IAssetsFile FindAssetsFile(AssetsFilePtr ptr)
		{
#warning TODO: improve checking
			return m_files.Find(t => t.Name == ptr.FileName);
		}

		public ResourcesFile FindResourcesFile(IAssetsFile iAssetsFile, string fileName)
		{
			AssetsFile assetsFile = (AssetsFile)iAssetsFile;
			string filePath = assetsFile.FilePath;
			foreach (ResourcesFile res in m_resources)
			{
				// assets bundles and loaded resources files
				if(res.FilePath == filePath && res.Name == fileName)
				{
					return res;
				}
			}

			string dirPath = Path.GetDirectoryName(filePath);
			filePath = Path.Combine(dirPath, fileName);
			if (File.Exists(filePath))
			{
				// lazy loading
				using (FileStream file = File.OpenRead(filePath))
				{
					using (BinaryReader stream = new BinaryReader(file))
					{
						if(file.Length > int.MaxValue)
						{
							throw new Exception($"Unsupported resource size {file.Length}");
						}
						byte[] buffer = new byte[file.Length];
						stream.Read(buffer, 0, buffer.Length);
						ResourcesFile resesFile = new ResourcesFile(filePath, fileName, buffer);
						m_resources.Add(resesFile);
						return resesFile;
					}
				}
			}
			return null;
		}

		public AssetType ToExportType(ClassIDType unityType)
		{
			return Exporter.ToExportType(unityType);
		}

		public string GetExportID(Object @object)
		{
			return Exporter.GetExportID(@object);
		}

		public ExportPointer CreateExportPointer(Object @object)
		{
			return Exporter.CreateExportPointer(@object);
		}

		public IEnumerable<Object> FetchObjects()
		{
			foreach(AssetsFile file in m_files)
			{
				foreach(Object @object in file.FetchObjects())
				{
					yield return @object;
				}
			}
		}

		private void AddAsset(AssetsFile file)
		{
			if(m_files.Any(t => t.Name == file.Name))
			{
				throw new ArgumentException($"Assets file with name '{file.Name}' already presents in collection", nameof(file));
			}

			if(file.IsReadBuildSettings)
			{
				FillVersion(file);
			}
			
			if (!m_files.All(t => t.Platform == file.Platform))
			{
				throw new ArgumentException($"Assets file '{file.Name}' has incompatible with other assets files platform {file.Platform} ", nameof(file));
			}
			
			m_files.Add(file);
			m_ifiles.Add(file);
		}

		private void FillVersion(AssetsFile file)
		{
			foreach (AssetPreloadData preload in file.PreloadData)
			{
				if(preload.UnityType == ClassIDType.BuildSettings)
				{	
					BuildSettings settings = (BuildSettings)preload.Object;
					file.Version.Parse(settings.BSVersion);
				}
			}
		}
		
		public AssetsExporter Exporter { get; } = new AssetsExporter();
		public IReadOnlyList<IAssetsFile> AssetsFiles => m_ifiles;

		private readonly List<AssetsFile> m_files = new List<AssetsFile>();
		private readonly List<IAssetsFile> m_ifiles = new List<IAssetsFile>();
		private readonly List<ResourcesFile> m_resources = new List<ResourcesFile>();
	}
}
