using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public abstract class Object : IStreamReadable, IYAMLDocExportable
	{
		protected Object(AssetInfo assetInfo)
		{
			if (assetInfo == null)
			{
				throw new ArgumentNullException(nameof(assetInfo));
			}
			m_assetInfo = assetInfo;
			if (assetInfo.ClassMap.IDType != ClassID)
			{
				throw new ArgumentException($"Try to initialize '{ClassID}' with '{assetInfo.ClassMap.IDType}' asset data", nameof(assetInfo));
			}

			if (IsAsset)
			{
				GUID = new UnityGUID(Guid.NewGuid());
			}
		}

		public void Read(byte[] buffer)
		{
			using (MemoryStream baseStream = new MemoryStream(buffer))
			{
				using (EndianStream stream = new EndianStream(baseStream))
				{
					Read(stream);

					if (stream.BaseStream.Position != buffer.Length)
					{
						throw new Exception($"Read less {stream.BaseStream.Position} than expected {buffer.Length}");
					}
				}
			}

			if(Config.IsGenerateGUIDByContent)
			{
				using (MD5 md5 = MD5.Create())
				{
					byte[] md5Hash = md5.ComputeHash(buffer);
					GUID = new UnityGUID(md5Hash);
				}
			}
		}

		public virtual void Read(EndianStream stream)
		{
			Reset();

			if (IsReadHideFlag)
			{
				ObjectHideFlags = stream.ReadUInt32();
			}
		}

		public virtual void Reset()
		{
		}

		/// <summary>
		/// Export object's content in such formats as txt or png
		/// </summary>
		/// <returns>Object's content</returns>
		public virtual byte[] ExportBinary()
		{
			throw new NotSupportedException($"Type {GetType()} doesn't support binary export");
		}

		public YAMLDocument ExportYAMLDocument()
		{
			YAMLMappingNode node = ExportYAMLRoot();
			YAMLMappingNode root = new YAMLMappingNode();
			root.Tag = ClassID.ToInt().ToString();
			root.Anchor = ExportID;
			root.Add(ClassID.ToString(), node);

			YAMLDocument document = new YAMLDocument(root);
			return document;
		}
		
		public virtual IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			yield break;
		}

		public virtual string ToLogString()
		{
			return $"{GetType().Name}'s[{PathID}]";
		}

		protected virtual YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_ObjectHideFlags", ObjectHideFlags);
			return node;
		}

		public IAssetsFile AssetsFile => m_assetInfo.AssetFile;
		public ClassIDType ClassID => m_assetInfo.ClassMap.IDType;
		public bool IsAsset => ClassID.IsAsset();
		public virtual string ExportExtension => "asset";
		public long PathID => m_assetInfo.PathID;
		
		public string ExportID
		{
			get { return AssetsFile.GetExportID(this); }
		}

		public UnityGUID GUID
		{
			get
			{
				if(!Config.IsGenerateGUIDByContent && !IsAsset)
				{
					throw new NotSupportedException("GUIDs are supported only for asset files");
				}
				return m_guid;
			}
			protected set
			{
				m_guid = value;
			}
		}

		public uint ObjectHideFlags { get; set; }

		protected Platform Platform => AssetsFile.Platform;
		protected Version Version => AssetsFile.Version;

		private bool IsReadHideFlag
		{
			get { return Platform == Platform.NoTarget; }
		}

		private readonly AssetInfo m_assetInfo;

		private UnityGUID m_guid;
	}
}
