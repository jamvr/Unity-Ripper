using System.Collections.Generic;
using UnityRipper.Classes.Materials;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class Material : NamedObject
	{
		public Material(AssetInfo assetInfo) :
			base(assetInfo)
		{
			if(IsSerialized)
			{
				SerializedShader = new PPtr<SShader>(AssetsFile);
			}
			else
			{
				Shader = new PPtr<Shader>(AssetsFile);
			}
			SavedProperties = new UnityPropertySheet(AssetsFile);

			if (IsReadStringTagMap)
			{
				StringTagMap = new Dictionary<string, string>();
			}
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			if (IsSerialized)
			{
				SerializedShader.Read(stream);
			}
			else
			{
				Shader.Read(stream);
			}

			if(IsReadKeywords)
			{
				if(IsKeywordsArray)
				{
					m_shaderKeywordsArray = stream.ReadStringArray();
				}
				else
				{
					ShaderKeywords = stream.ReadStringAligned();
				}
			}

			if(IsReadLightmapFlags)
			{
				LightmapFlags = stream.ReadUInt32();
				if (IsReadOtherFlags)
				{
					EnableInstancingVariants = stream.ReadBoolean();
					DoubleSidedGI = stream.ReadBoolean();
					stream.AlignStream(AlignType.Align4);
				}
			}

			if (IsReadCustomRenderQueue)
			{
				CustomRenderQueue = stream.ReadInt32();
			}

			if (IsReadStringTagMap)
			{
				StringTagMap.Read(stream);
				if (IsReadDisabledShaderPasses)
				{
					m_disabledShaderPasses = stream.ReadStringArray();
				}
			}

			SavedProperties.Read(stream);
		}

		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}

			if(IsSerialized)
			{
				SShader shader = SerializedShader.FindObject();
				if(shader == null)
				{
					if (isLog)
					{
						Logger.Log(LogType.Warning, LogCategory.Export, $"{ToLogString()} m_Shader {SerializedShader.ToLogString()} wasn't found ");
					}
				}
				else
				{
					yield return shader;
				}
			}
			else
			{
				Shader shader = Shader.FindObject();
				if (shader == null)
				{
					if (isLog)
					{
						Logger.Log(LogType.Warning, LogCategory.Export, $"{ToLogString()} m_Shader {Shader.ToLogString()} wasn't found ");
					}
				}
				else
				{
					yield return shader;
				}
			}

			foreach(Object @object in SavedProperties.FetchDependencies(isLog))
			{
				yield return @object;
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.InsertBegin("serializedVersion", SerializedVersion);
			if (IsSerialized)
			{
				node.Add("m_Shader", SerializedShader.ExportYAML());
			}
			else
			{
				node.Add("m_Shader", Shader.ExportYAML());
			}
			if(IsKeywordsArray)
			{
				node.Add("m_ShaderKeywords", string.Join(" ", m_shaderKeywordsArray));
			}
			else
			{
				node.Add("m_ShaderKeywords", ShaderKeywords);
			}
			node.Add("m_LightmapFlags", LightmapFlags);
			node.Add("m_EnableInstancingVariants", EnableInstancingVariants);
			node.Add("m_DoubleSidedGI", DoubleSidedGI);
			node.Add("m_CustomRenderQueue", CustomRenderQueue);
			if(IsReadStringTagMap)
			{
				node.Add("stringTagMap", StringTagMap.ExportYAML());
			}
			else
			{
				node.Add("stringTagMap", YAMLMappingNode.Empty);
			}
			if (IsReadDisabledShaderPasses)
			{
#warning TODO: check on sample
				node.Add("disabledShaderPasses", DisabledShaderPasses.ExportYAML());
			}
			else
			{
				node.Add("disabledShaderPasses", YAMLSequenceNode.Empty);
			}
			node.Add("m_SavedProperties", SavedProperties.ExportYAML());
			return node;
		}
		
		public override string ExportExtension => "mat";
		
		public PPtr<Shader> Shader { get; }
		public PPtr<SShader> SerializedShader { get; }
		public IReadOnlyList<string> ShaderKeywordsArray => m_shaderKeywordsArray;
		public string ShaderKeywords { get; private set; } = string.Empty;
		public int CustomRenderQueue { get; private set; }
		public uint LightmapFlags { get; private set; }
		public bool EnableInstancingVariants { get; private set; }
		public bool DoubleSidedGI { get; private set; }
		public IReadOnlyList<string> DisabledShaderPasses => m_disabledShaderPasses;
		public Dictionary<string, string> StringTagMap { get; }
		public UnityPropertySheet SavedProperties { get; }
		
		/// <summary>
		/// 4.1.0b and greater
		/// </summary>
		private bool IsReadKeywords => Version.IsGreaterEqual(4, 1, 0, VersionType.Beta);
		/// <summary>
		/// Less 5.0.0
		/// </summary>
		private bool IsKeywordsArray => Version.IsLess(5);
		/// <summary>
		/// 4.3.0 and greater
		/// </summary>
		private bool IsReadCustomRenderQueue => Version.IsGreaterEqual(4, 3);
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		private bool IsReadLightmapFlags => Version.IsGreaterEqual(5);
		/// <summary>
		/// 5.6.0 and greater
		/// </summary>
		private bool IsReadOtherFlags => Version.IsGreater(5, 6);
		/// <summary>
		/// 5.1.0 and greater
		/// </summary>
		private bool IsReadStringTagMap => Version.IsGreater(5, 1);
		/// <summary>
		/// 5.6.0 and greater
		/// </summary>
		private bool IsReadDisabledShaderPasses => Version.IsGreater(5, 6);
		/// <summary>
		/// 5.5.0 and greater
		/// </summary>
		private bool IsSerialized => SShader.IsSerialized(Version);

		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 6;
			}
		}

		private string[] m_shaderKeywordsArray = null;
		private string[] m_disabledShaderPasses = null;
	}
}
