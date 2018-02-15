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
					AssetsFile assetsFile = entry.ParseAssetsFile(this);
					AddAsset(assetsFile);
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
			if (!m_files.All(t => t.Version == file.Version))
			{
			}
			
			m_files.Add(file);
			m_ifiles.Add(file);
			Test(file);
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

		private void Test(AssetsFile file)
		{
			foreach (AssetPreloadData preload in file.PreloadData)
			{
				switch(preload.UnityType)
				{
					case ClassIDType.SkinnedMeshRenderer:
						//SkinnedMeshRenderer skin = (SkinnedMeshRenderer)preload.RetreaveObject(preload.AssetInfo);
						//Exporter.Export(skin, "D:\\Test\\1\\");
						break;
						
					case ClassIDType.Mesh:
						//Mesh mesh = (Mesh)preload.RetreaveObject(preload.AssetInfo);
						//string mpath = Path.Combine("D:\\Test\\Mesh\\", mesh.Name);
						//mpath = $"{mpath}.{mesh.ExportExtension}";
						//YAMLExporter.Export(mesh, mpath);
						break;

					case ClassIDType.AnimationClip:
						//AnimationClip clip = (AnimationClip)preload.RetreaveObject(preload.AssetInfo);
						//string cpath = Path.Combine("D:\\Test\\AnimationClip\\", clip.Name);
						//cpath = $"{cpath}.{clip.ExportExtension}";
						//YAMLExporter.Export(clip, cpath);
						break;

					case ClassIDType.Material:
						//Material mat = (Material)preload.RetreaveObject(preload.AssetInfo);
						//string matpath = Path.Combine("D:\\Test\\Material\\", mat.Name);
						//matpath = $"{matpath}.{mat.ExportExtension}";
						//YAMLExporter.Export(mat, matpath);
						break;

					case ClassIDType.Shader:
						if(SShader.IsSerialized(file.Version))
						{
							//SShader shader = (SShader)preload.RetreaveObject(preload.AssetInfo);
						}
						else
						{
							/*Shader shader = (Shader)preload.RetreaveObject(preload.AssetInfo);
							string shaderpath = Path.Combine("D:\\Test\\Shader\\", shader.Name);
							shaderpath = $"{shaderpath}.{shader.ExportExtension}";
							using (FileStream fileStream = File.Open(shaderpath, FileMode.Create, FileAccess.Write))
							{
								byte[] content = shader.ExportBinary();
								fileStream.Write(content, 0, content.Length);
							}*/
						}
						break;
						
					case ClassIDType.TextAsset:
						/*TextAsset text = (TextAsset)preload.RetreaveObject(preload.AssetInfo);
						string textpath = Path.Combine("D:\\Test\\Text\\", text.Name);
						textpath = $"{textpath}.{text.ExportExtension}";
						using (FileStream fileStream = File.Open(textpath, FileMode.Create, FileAccess.Write))
						{
							byte[] content = text.ExportBinary();
							fileStream.Write(content, 0, content.Length);
						}*/
						break;

					default:
						//preload.RetreaveObject(preload.AssetInfo);
						break;
				}
			}
		}
		
		public AssetsExporter Exporter { get; } = new AssetsExporter();
		public IReadOnlyList<IAssetsFile> AssetsFiles => m_ifiles;

		private readonly List<AssetsFile> m_files = new List<AssetsFile>();
		private readonly List<IAssetsFile> m_ifiles = new List<IAssetsFile>();
	}
}
