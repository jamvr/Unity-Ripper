using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class BuildSettings : GlobalGameManager
	{
		public BuildSettings(AssetInfo assetInfo):
			base(assetInfo)
		{
			if(IsReadBuildTags)
			{
				BuildGUID = new UnityGUID();
			}
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			m_scenes = stream.ReadStringArray();
			if (IsReadPreloadPlugin)
			{
				m_preloadedPlugins = stream.ReadStringArray();
			}

			if (IsReadEnabledVRDevices)
			{
				m_enabledVRDevices = stream.ReadStringArray();
				if(IsReadBuildTags)
				{
					m_buildTags = stream.ReadStringArray();
					BuildGUID.Read(stream);
				}
			}

			// bool flags
			HasPROVersion = stream.ReadBoolean();
			IsNoWatermarkBuild = stream.ReadBoolean();
			IsPrototypingBuild = stream.ReadBoolean();
			IsEducationalBuild = stream.ReadBoolean();
			if (IsReadBoolFlags)
			{
				IsEmbedded = stream.ReadBoolean();
				HasPublishingRights = stream.ReadBoolean();
				HasShadows = stream.ReadBoolean();
				HasSoftShadows = stream.ReadBoolean();
			}
			if (IsReadMoreBoolFlags)
			{
				HasLocalLightShadows = stream.ReadBoolean();
				HasAdvancedVersion = stream.ReadBoolean();
				EnableDynamicBatching = stream.ReadBoolean();
				IsDebugBuild = stream.ReadBoolean();
			}
			if (IsReadEvenMoreBoolFlags)
			{
				UsesOnMouseEvents = stream.ReadBoolean();
				HasClusterRendering = stream.ReadBoolean();
				stream.AlignStream(AlignType.Align4);
			}

			BSVersion = stream.ReadStringAligned();
			AuthToken = stream.ReadStringAligned();

			m_runtimeClassHashes.Read(stream);
			m_scriptHashes.Read(stream);
			m_graphicsAPIs = stream.ReadInt32Array();
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();

#warning TODO:
			throw new System.NotImplementedException();
		}

		public IReadOnlyList<string> Scenes => m_scenes;
		public IReadOnlyList<string> PreloadedPlugins => m_preloadedPlugins;
		public IReadOnlyList<string> EnabledVRDevices => m_enabledVRDevices;
		public IReadOnlyList<string> BuildTags => m_buildTags;
		public UnityGUID BuildGUID { get; }
		public bool HasPROVersion { get; private set; }
		public bool IsNoWatermarkBuild { get; private set; }
		public bool IsPrototypingBuild { get; private set; }
		public bool IsEducationalBuild { get; private set; }
		public bool IsEmbedded { get; private set; }
		public bool HasPublishingRights { get; private set; }
		public bool HasShadows { get; private set; }
		public bool HasSoftShadows { get; private set; }
		public bool HasLocalLightShadows { get; private set; }
		public bool HasAdvancedVersion { get; private set; }
		public bool EnableDynamicBatching { get; private set; }
		public bool IsDebugBuild { get; private set; }
		public bool UsesOnMouseEvents { get; private set; }
		public bool HasClusterRendering { get; private set; }
		public string BSVersion { get; private set; } = string.Empty;
		public string AuthToken { get; private set; } = string.Empty;
		public IReadOnlyDictionary<int, Hash128> RuntimeClassHashes => m_runtimeClassHashes;
		public IReadOnlyDictionary<Hash128, Hash128> ScriptHashes => m_scriptHashes;
		public IReadOnlyList<int> GraphicsAPIs => m_graphicsAPIs;
		
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		private bool IsReadPreloadPlugin => Version.IsGreaterEqual(5);
		/// <summary>
		/// 5.4.0 and greater
		/// </summary>
		private bool IsReadEnabledVRDevices => Version.IsGreaterEqual(5, 4);
		/// <summary>
		/// 5.6.0 and greater
		/// </summary>
		private bool IsReadBuildTags => Version.IsGreaterEqual(5, 6);
		/// <summary>
		/// 3.0.0 and greater
		/// </summary>
		private bool IsReadBoolFlags => Version.IsGreaterEqual(3, 0, 0, VersionType.Base);
		/// <summary>
		/// 3.5.0 and greater
		/// </summary>
		private bool IsReadMoreBoolFlags => Version.IsGreaterEqual(3, 5);
		/// <summary>
		/// Greater than 4.2.0a
		/// </summary>
		private bool IsReadEvenMoreBoolFlags => Version.IsGreater(4, 2, 0, VersionType.Alpha);

		private readonly Dictionary<int, Hash128> m_runtimeClassHashes = new Dictionary<int, Hash128>();
		private readonly Dictionary<Hash128, Hash128> m_scriptHashes = new Dictionary<Hash128, Hash128>();

		private string[] m_scenes;
		private string[] m_preloadedPlugins;
		private string[] m_enabledVRDevices;
		private string[] m_buildTags;
		private int[] m_graphicsAPIs;
	}
}
