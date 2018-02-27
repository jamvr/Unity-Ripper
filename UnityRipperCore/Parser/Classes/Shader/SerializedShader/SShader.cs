using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityRipper.Classes.Shaders;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class SShader : NamedObject
	{
		public SShader(AssetInfo assetInfo):
			base(assetInfo)
		{
			ParsedForm = new SerializedShader(AssetsFile);
		}

		public static bool IsSerialized(Version version)
		{
			return version.IsGreaterEqual(5, 5);
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			ParsedForm.Read(stream);
			Name = ParsedForm.Name;
			
			m_platforms = stream.ReadEnum32Array((t) => (GPUPlatform)t);
			uint[] offsets = stream.ReadUInt32Array();
			uint[] compressedLengths = stream.ReadUInt32Array();
			uint[] decompressedLengths = stream.ReadUInt32Array();
			byte[] compressedBlob = stream.ReadByteArray();
			stream.AlignStream(AlignType.Align4);

			m_subProgramBlobs = new ShaderSubProgramBlob[m_platforms.Length];
			using (MemoryStream memStream = new MemoryStream(compressedBlob))
			{
				for(int i = 0; i < m_platforms.Length; i++)
				{
					uint offset = offsets[i];
					uint compressedLength = compressedLengths[i];
					uint decompressedLength = decompressedLengths[i];

					memStream.Position = offset;
					byte[] decompressedBuffer = new byte[decompressedLength];
					using (Lz4Stream lz4Stream = new Lz4Stream(memStream, (int)compressedLength))
					{
						int read = lz4Stream.Read(decompressedBuffer, 0, decompressedBuffer.Length);
						if (read != decompressedLength)
						{
							throw new Exception($"Can't properly decode shader blob. Read {read} but expected {decompressedLength}");
						}
					}

					using (MemoryStream blobMem = new MemoryStream(decompressedBuffer))
					{
						using (EndianStream blobStream = new EndianStream(blobMem))
						{
							ShaderSubProgramBlob blob = new ShaderSubProgramBlob(AssetsFile);
							blob.Read(blobStream);
							m_subProgramBlobs[i] = blob;
						}
					}
				}
			}

			m_dependencies = stream.ReadArray(() => new PPtr<SShader>(AssetsFile));
			ShaderIsBaked = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);
		}

		public override byte[] ExportBinary()
		{
			StringBuilder sb = new StringBuilder();
			ParsedForm.ToString(sb, this);			
			return Encoding.UTF8.GetBytes(sb.ToString());
		}

		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}
			foreach (PPtr<SShader> ptr in Dependencies)
			{
				SShader shad = ptr.FindObject();
				if(shad == null)
				{
					if (isLog)
					{
						Logger.Log(LogType.Warning, LogCategory.Export, $"{ToLogString()} shader m_dependencies {ptr.ToLogString()} wasn't found ");
					}
				}
				else
				{
					yield return shad;
				}
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			throw new NotSupportedException();
		}
		
		public override string ExportExtension => "shader";
		
		public SerializedShader ParsedForm { get; }
		public IReadOnlyList<GPUPlatform> Platforms => m_platforms;
		public IReadOnlyList<ShaderSubProgramBlob> SubProgramBlobs => m_subProgramBlobs;
		public IReadOnlyList<PPtr<SShader>> Dependencies => m_dependencies;
		public bool ShaderIsBaked { get; private set; }

		private GPUPlatform[] m_platforms;
		private ShaderSubProgramBlob[] m_subProgramBlobs;
		private PPtr<SShader>[] m_dependencies = null;
	}
}
