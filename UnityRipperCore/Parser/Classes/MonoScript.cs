using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class MonoScript : NamedObject
	{
		public MonoScript(AssetInfo assetInfo):
			base(assetInfo)
		{
			if (IsReadScript)
			{
				m_defaultReferences = new Dictionary<string, PPtr<Object>>();
				Icon = new PPtr<Object>(AssetsFile);
			}
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			if(IsReadScript)
			{
				Script = stream.ReadStringAligned();
				m_defaultReferences.Read(stream, () => new PPtr<Object>(AssetsFile));
				Icon.Read(stream);
			}
			ExecutionOrder = stream.ReadInt32();
			if (IsUIntHash)
			{
				PropertiesHash = stream.ReadUInt32();
			}
			else
			{
				PropertiesHash128.Read(stream);
			}
			ClassName = stream.ReadStringAligned();
			Namespace = stream.ReadStringAligned();
			AssemblyName = stream.ReadStringAligned();
			IsEditorScript = stream.ReadBoolean();
		}

		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach (Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}
			if(IsReadScript)
			{
				foreach (PPtr<Object> ptr in DefaultReferences.Values)
				{
					Object @object = ptr.FindObject();
					if (@object == null)
					{
						if (isLog)
						{
							Logger.Log(LogType.Warning, LogCategory.Export, $"{ToLogString()} DefaultReferences {ptr.ToLogString()} wasn't found ");
						}
					}
					else
					{
						yield return @object;
					}
				}

				Object icon = Icon.FindObject();
				if (icon == null)
				{
					if (isLog)
					{
						Logger.Log(LogType.Warning, LogCategory.Export, $"{ToLogString()} m_Icon {icon.ToLogString()} wasn't found ");
					}
				}
				else
				{
					yield return icon;
				}
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			if (IsReadScript)
			{
				node.Add("m_Script", Script);
				node.Add("m_DefaultReferences", DefaultReferences.ExportYAML());
				node.Add("m_Icon", Icon.ExportYAML());
			}
			node.Add("m_ExecutionOrder", ExecutionOrder);
			node.Add("m_ClassName", ClassName);
			node.Add("m_Namespace", Namespace);
			node.Add("m_AssemblyName", AssemblyName);
			node.Add("m_IsEditorScript", IsEditorScript);
			return node;
		}

		public string Script { get; private set; }
		public IReadOnlyDictionary<string, PPtr<Object>> DefaultReferences => m_defaultReferences;
		public PPtr<Object> Icon { get; }
		public int ExecutionOrder { get; private set; }
		public uint PropertiesHash { get; private set; }
		public Hash128 PropertiesHash128 { get; } = new Hash128();
		public string ClassName { get; private set; }
		public string Namespace { get; private set; }
		public string AssemblyName { get; private set; }
		public bool IsEditorScript { get; private set; }

		/// <summary>
		/// Unity Package
		/// </summary>
		public bool IsReadScript => Platform == Platform.NoTarget;
		/// <summary>
		/// Less than 5.0.0
		/// </summary>
		public bool IsUIntHash => Version.IsLess(5);
			
		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 4;
			}
		}

		private Dictionary<string, PPtr<Object>> m_defaultReferences;
	}
}
