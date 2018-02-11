using System.Collections.Generic;
using UnityRipper.Classes.MeshRenderers;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public abstract class Renderer : Component
	{
		protected Renderer(AssetInfo assetInfo) :
			base(assetInfo)
		{
			StaticBatchRoot = new PPtr<Transform>(AssetsFile);
			ProbeAnchor = new PPtr<Transform>(AssetsFile);
			LightProbeVolumeOverride = new PPtr<GameObject>(AssetsFile);
		}
		
		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			Enabled = stream.ReadBoolean();
			if(IsReadAlign)
			{
				stream.AlignStream(AlignType.Align4);
			}

			CastShadows = stream.ReadByte();
			ReceiveShadows = stream.ReadByte();
			if (IsReadAlign)
			{
				stream.AlignStream(AlignType.Align4);
			}

			if (IsReadMotionVector)
			{
				MotionVectors = stream.ReadByte();
				LightProbeUsage = stream.ReadByte();
				ReflectionProbeUsage = stream.ReadByte();
				stream.AlignStream(AlignType.Align4);
			}

			if (IsSmallLightIndex)
			{
				LightmapIndex = stream.ReadByte();
			}
			else
			{
				LightmapIndex = stream.ReadUInt16();
			}

			if(IsReadLightDynamic)
			{
				LightmapIndexDynamic = stream.ReadUInt16();
			}

			if (IsReadTileOffset)
			{
				LightmapTilingOffset.Read(stream);
			}
			if (IsReadTileDynamic)
			{
				LightmapTilingOffsetDynamic.Read(stream);
			}
			
			m_materials = stream.ReadArray(() => new PPtr<Material>(AssetsFile));

			if (IsReadSubsetIndices)
			{
				m_subsetIndices = stream.ReadUInt32Array();
			}
			else
			{
				StaticBatchInfo.Read(stream);
			}

			StaticBatchRoot.Read(stream);

			if (IsReadUseLight)
			{
				UseLightProbes = stream.ReadBoolean();
				stream.AlignStream(AlignType.Align4);
				if (IsReadReflectUsage)
				{
					ReflectionProbeUsage = stream.ReadInt32();
				}
			}

			ProbeAnchor.Read(stream);
			if (IsReadLightOverride)
			{
				LightProbeVolumeOverride.Read(stream);
			}
			stream.AlignStream(AlignType.Align4);

			SortingLayerID = stream.ReadInt32();

			if (IsReadLayer)
			{
				SortingLayer = stream.ReadInt16();
			}

			SortingOrder = stream.ReadInt16();
			stream.AlignStream(AlignType.Align4);
		}

		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}

			foreach (PPtr<Material> ptr in Materials)
			{
				if(ptr.IsNull)
				{
					continue;
				}

				Material mat = ptr.FindObject();
				if (mat == null)
				{
					if(isLog)
					{
						Logger.Log(LogType.Warning, LogCategory.Export, $"{ToLogString()} m_Materials {ptr.ToLogString()} wasn't found ");
					}
				}
				else
				{
					yield return mat;
				}
			}
			if (!StaticBatchRoot.IsNull)
			{
				yield return StaticBatchRoot.GetObject();
			}
			if(!ProbeAnchor.IsNull)
			{
				yield return ProbeAnchor.GetObject();
			}
			if(!LightProbeVolumeOverride.IsNull)
			{
				yield return LightProbeVolumeOverride.GetObject();
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
#warning TODO: check undefined vars
#warning TODO: write according to version
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.Add("m_Enabled", Enabled);
			node.Add("m_CastShadows", CastShadows);
			node.Add("m_ReceiveShadows", ReceiveShadows);
			if(IsReadDynamicOccludee)
			{
#warning TODO:
				node.Add("m_DynamicOccludee", 1);
			}
			else
			{
				node.Add("m_DynamicOccludee", 1);
			}
			node.Add("m_MotionVectors", MotionVectors);
			node.Add("m_LightProbeUsage", LightProbeUsage);
			node.Add("m_ReflectionProbeUsage", ReflectionProbeUsage);
			node.Add("m_Materials", Materials.ExportYAML());
			node.Add("m_StaticBatchInfo", StaticBatchInfo.ExportYAML());
			node.Add("m_StaticBatchRoot", StaticBatchRoot.ExportYAML());
			node.Add("m_ProbeAnchor", ProbeAnchor.ExportYAML());
			node.Add("m_LightProbeVolumeOverride", LightProbeVolumeOverride.ExportYAML());
#warning what are those vars?
			node.Add("m_ScaleInLightmap", 1);
			node.Add("m_PreserveUVs", 0);
			node.Add("m_IgnoreNormalsForChartDetection", 0);
			node.Add("m_ImportantGI", 0);
#warning TODO? is read this parameter or just write default value?
			node.Add("m_StitchLightmapSeams", 0);
			node.Add("m_SelectedEditorRenderState", 3);
			node.Add("m_MinimumChartSize", 4);
			node.Add("m_AutoUVMaxDistance", 0.5f);
			node.Add("m_AutoUVMaxAngle", 89);
			node.Add("m_LightmapParameters", PPtr<Object>.Empty.ExportYAML());
			node.Add("m_SortingLayerID", SortingLayerID);
			node.Add("m_SortingLayer", SortingLayer);
			node.Add("m_SortingOrder", SortingOrder);
			return node;
		}

		public bool Enabled { get; private set; }
		public byte CastShadows { get; private set; }
		public byte ReceiveShadows { get; private set; }
		public byte MotionVectors { get; private set; }
		public byte LightProbeUsage { get; private set; }
		public int ReflectionProbeUsage { get; private set; }
		public ushort LightmapIndex { get; private set; }
		public ushort LightmapIndexDynamic { get; private set; }
		public Vector4f LightmapTilingOffset { get; } = new Vector4f();
		public Vector4f LightmapTilingOffsetDynamic { get; } = new Vector4f();
		public IReadOnlyList<PPtr<Material>> Materials => m_materials;
		public IReadOnlyList<uint> SubsetIndices => m_subsetIndices;
		public StaticBatchInfo StaticBatchInfo { get; } = new StaticBatchInfo();
		public PPtr<Transform> StaticBatchRoot { get; }
		public bool UseLightProbes { get; private set; }
		public PPtr<Transform> ProbeAnchor { get; }
		public PPtr<GameObject> LightProbeVolumeOverride { get; }
		public int SortingLayerID { get; private set; }
		public short SortingLayer { get; private set; }
		public short SortingOrder { get; private set; }

		/// <summary>
		/// 5.0.0 to 5.3.x
		/// </summary>
		private bool IsReadAlign => Version.IsGreaterEqual(5) && Version.IsLess(5, 4);
		/// <summary>
		/// 2017.2 and greater
		/// </summary>
		private bool IsReadDynamicOccludee => Version.IsGreaterEqual(2017, 2);
		/// <summary>
		/// 5.4.0 and greater
		/// </summary>
		private bool IsReadMotionVector => Version.IsGreaterEqual(5, 4);
		/// <summary>
		/// Less 5.0.0
		/// </summary>
		private bool IsSmallLightIndex => Version.IsLess(5);
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		private bool IsReadLightDynamic => Version.IsGreaterEqual(5);
		/// <summary>
		/// 3.0.0 and greater
		/// </summary>
		private bool IsReadTileOffset => Version.IsGreaterEqual(3);
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		private bool IsReadTileDynamic => Version.IsGreaterEqual(5);
		/// <summary>
		/// Less 5.5.0
		/// </summary>
		private bool IsReadSubsetIndices => Version.IsLess(5, 5);
		/// <summary>
		/// Less 5.1.0
		/// </summary>
		private bool IsReadUseLight => Version.IsLess(5, 1);
		/// <summary>
		/// 5.0.x
		/// </summary>
		private bool IsReadReflectUsage => Version.IsGreaterEqual(5, 0);
		/// <summary>
		/// 5.3.0 and greater
		/// </summary>
		private bool IsReadLightOverride => Version.IsGreaterEqual(5, 4);
		/// <summary>
		/// 5.6.0 and greater
		/// </summary>
		private bool IsReadLayer => Version.IsGreaterEqual(5, 6, 0);

		private PPtr<Material>[] m_materials;
		private uint[] m_subsetIndices;
	}
}
