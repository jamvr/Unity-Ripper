using System;
using System.Collections.Generic;
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
		}

		public static bool IsSerialized(Version version)
		{
			return version.IsGreaterEqual(5, 5);
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			ParsedForm.Read(stream);
			Name = ParsedForm.Name.Replace("/", "_");
			m_platforms = stream.ReadUInt32Array();
			m_offsets = stream.ReadUInt32Array();
			m_compressedLengths = stream.ReadUInt32Array();
			m_decompressedLengths = stream.ReadUInt32Array();
			m_compressedBlob = stream.ReadByteArray();
			stream.AlignStream(AlignType.Align4);

			m_dependencies = stream.ReadArray(() => new PPtr<SShader>(AssetsFile));
			ShaderIsBaked = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);
		}

		public override byte[] ExportBinary()
		{
#warning TODO: build text representation from binary parameters
			return Encoding.UTF8.GetBytes("Not implemented yet");
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
		
		public SerializedShader ParsedForm { get; } = new SerializedShader();
		public IReadOnlyList<uint> Platforms => m_platforms;
		public IReadOnlyList<uint> Offsets => m_offsets;
		public IReadOnlyList<uint> CompressedLengths => m_compressedLengths;
		public IReadOnlyList<uint> DecompressedLengths => m_decompressedLengths;
		public IReadOnlyList<byte> CompressedBlob => m_compressedBlob;
		public IReadOnlyList<PPtr<SShader>> Dependencies => m_dependencies;
		public bool ShaderIsBaked { get; private set; }

		private uint[] m_platforms;
		private uint[] m_offsets;
		private uint[] m_compressedLengths;
		private uint[] m_decompressedLengths;
		private byte[] m_compressedBlob;
		private PPtr<SShader>[] m_dependencies = null;
	}
}
