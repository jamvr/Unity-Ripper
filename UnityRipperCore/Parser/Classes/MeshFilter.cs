using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class MeshFilter : Component
	{
		public MeshFilter(AssetInfo assetInfo) :
			base(assetInfo)
		{
			Mesh = new PPtr<Mesh>(AssetsFile);
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			Mesh.Read(stream);
		}

		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach (Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}

			Mesh mesh = Mesh.FindObject();
			if (mesh == null)
			{
				if(isLog)
				{
					Logger.Log(LogType.Warning, LogCategory.Export, $"{ToLogString()} m_Mesh {Mesh.ToLogString()} wasn't found ");
				}
			}
			else
			{
				yield return mesh;
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.Add("m_Mesh", Mesh.ExportYAML());
			return node;
		}
		
		public PPtr<Mesh> Mesh { get; }
	}
}
