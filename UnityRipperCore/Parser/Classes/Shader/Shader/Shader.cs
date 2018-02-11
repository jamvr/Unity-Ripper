using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityRipper.Classes.Shaders;

namespace UnityRipper.Classes
{
	public sealed class Shader : TextAsset
	{
		public Shader(AssetInfo assetInfo) :
			base(assetInfo)
		{
			if (IsEncoded)
			{
				SubProgramBlob = new ShaderSubProgramBlob();
			}
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			if(IsSerialized)
			{
				throw new NotSupportedException("Use SShader for serialized shaders");
			}

			if(IsEncoded)
			{
				uint decompressedSize = stream.ReadUInt32();
				int comressedSize = stream.ReadInt32();

				byte[] subProgramBlob = new byte[comressedSize];
				stream.Read(subProgramBlob, 0, comressedSize);
				stream.AlignStream(AlignType.Align4);

				if (comressedSize > 0 && decompressedSize > 0)
				{
					byte[] decompressedBuffer = new byte[decompressedSize];
					using (MemoryStream memStream = new MemoryStream(subProgramBlob))
					{
						using (Lz4Stream lz4Stream = new Lz4Stream(memStream))
						{
							int read = lz4Stream.Read(decompressedBuffer, 0, (int)decompressedSize);
							if (read != decompressedSize)
							{
								throw new Exception($"Can't properly decode sub porgram prob. Read {read} but expected decompressedSize");
							}
						}
					}

					using (MemoryStream memStream = new MemoryStream(decompressedBuffer))
					{
						using (EndianStream blobStream = new EndianStream(memStream))
						{
							SubProgramBlob.Read(blobStream);
						}
					}
				}
			}
			
			m_dependencies = stream.ReadArray(() => new PPtr<Shader>(AssetsFile));
			ShaderIsBaked = stream.ReadBoolean();
			stream.AlignStream(AlignType.Align4);
		}

		public override byte[] ExportBinary()
		{
			if (IsEncoded)
			{
				StringBuilder builder = new StringBuilder();
				builder.Append(Script).AppendLine();
				if (SubProgramBlob.SubPrograms.Length != 0)
				{
					builder.AppendLine("WARNING!!! THIS SHADER ISN'T VALID").AppendLine();
				}

#warning TODO: implement shader builder
				foreach (ShaderSubProgram subProgram in SubProgramBlob.SubPrograms)
				{
					if (string.IsNullOrEmpty(subProgram.ProgramData))
					{
						continue;
					}
					builder.AppendLine().Append(subProgram.ProgramData);
				}
				return Encoding.UTF8.GetBytes(builder.ToString());
			}
			else
			{
				return base.ExportBinary();
			}
		}

		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach (Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}
			foreach (PPtr<Shader> ptr in Dependencies)
			{
				Shader shad = ptr.FindObject();
				if (shad == null)
				{
					if(isLog)
					{
						Logger.Log(LogType.Warning, LogCategory.Export, $"{ToLogString()} m_dependencies {ptr.ToLogString()} wasn't found ");
					}
				}
				else
				{
					yield return shad;
				}
			}
		}

		public override string ExportExtension => "shader";

		public ShaderSubProgramBlob SubProgramBlob { get; }
		public IReadOnlyList<PPtr<Shader>> Dependencies => m_dependencies;
		public bool ShaderIsBaked { get; private set; }

		/// <summary>
		/// 5.3.0 till 5.5.0 exclusive
		/// </summary>
		public bool IsEncoded => Version.IsGreaterEqual(5, 3);

		/// <summary>
		/// 5.5.0 and greater
		/// </summary>
		private bool IsSerialized => SShader.IsSerialized(Version);

		private PPtr<Shader>[] m_dependencies = null;
	}
}
