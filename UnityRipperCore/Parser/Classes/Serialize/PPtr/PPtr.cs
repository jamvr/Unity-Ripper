using System;
using UnityRipper.AssetExporters.Classes;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class PPtr<T> : IPPtr<T>
		where T: Object
	{
		private PPtr()
		{
		}
		
		public PPtr(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public PPtr(Object @object):
			this(@object.AssetsFile)
		{
			AssetsFileIndex = 0;
			PathID = @object.PathID;
		}

		public void Read(EndianStream stream)
		{
			AssetsFileIndex = stream.ReadInt32();

			if (IsLongID)
			{
				PathID = stream.ReadInt64();
			}
			else
			{
				PathID = stream.ReadInt32();
			}
		}

		public YAMLNode ExportYAML()
		{
#warning TODO: rewrite all export yaml methods to methods with export paramter
			if (IsNull)
			{
				return ExportPointer.EmptyPointer.ExportYAML();
			}

			T @object = FindObject();
			if (@object == null)
			{
				ClassIDType classType = typeof(T).ToUnityClassType();
				AssetType assetType = m_assetsFile.ToExportType(classType);
				ExportPointer pointer = new ExportPointer(classType, assetType);
				return pointer.ExportYAML();
			}

			return m_assetsFile.CreateExportPointer(@object).ExportYAML();
		}

		public T FindObject()
		{
			if (PathID == 0)
			{
				return null;
			}
			Object @object = m_assetsFile.FindObject(AssetsFileIndex, PathID);
			if(@object == null)
			{
				return null;
			}
			T t = @object as T;
			if (t == null)
			{
				throw new Exception($"Object's type {@object.ClassID} doesn't assignable from {typeof(T).Name}");
			}
			return t;
		}

		public T TryGetObject()
		{
			if(PathID == 0)
			{
				return null;
			}
			return GetObject();
		}

		public T GetObject()
		{
			Object @object = m_assetsFile.GetObject(AssetsFileIndex, PathID);
			T t = @object as T;
			if(t == null)
			{
				throw new Exception($"Object's type {@object.ClassID} doesn't assignable from {typeof(T).Name}");
			}
			return t;
		}
		
		public override string ToString()
		{
			return $"[{AssetsFileIndex}, {PathID}]";
		}

		public string ToLogString()
		{
			if(Config.IsAdvancedLog)
			{
				string depName = m_assetsFile.Dependencies[AssetsFileIndex].FileName;
				ClassIDType classID = typeof(T).ToUnityClassType();
				return $"[{classID} {PathID}({depName})]";
			}
			return ToString();
		}

		public static readonly IYAMLExportable Empty = new PPtr<T>();
		
		public bool IsNull => PathID == 0;
		public bool IsValid => FindObject() != null;

		/// <summary>
		/// 0 means current file
		/// </summary>
		public int AssetsFileIndex { get; private set; }
		/// <summary>
		/// It is acts more like a hash in some games
		/// </summary>
		public long PathID { get; private set; }

		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		private bool IsLongID => Version.IsGreaterEqual(5);

		private Version Version => m_assetsFile.Version;

		private readonly IAssetsFile m_assetsFile;
	}
}
