using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class MeshRenderer : Renderer
	{
		public MeshRenderer(AssetInfo assetInfo):
			base(assetInfo)
		{
			AdditionalVertexStreams = new PPtr<Mesh>(AssetsFile);
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);
			
			if (IsReadVertex)
			{
				AdditionalVertexStreams.Read(stream);
			}
		}

		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}

			if(!AdditionalVertexStreams.IsNull)
			{
				Mesh mesh = AdditionalVertexStreams.FindObject();
				if (mesh == null)
				{
					if(isLog)
					{
						Logger.Log(LogType.Warning, LogCategory.Export, $"{ToLogString()} m_AdditionalVertexStreams {AdditionalVertexStreams.ToLogString()} wasn't found ");
					}
				}
				else
				{
					yield return mesh;
				}
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
#warning why prefabs dont contain this var?
			//node.Add("m_AdditionalVertexStreams", m_additionalVertexStreams.ExportYAML());
			return node;
		}

		public PPtr<Mesh> AdditionalVertexStreams { get; }

		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		private bool IsReadVertex => Version.IsGreaterEqual(5);
	}
}
