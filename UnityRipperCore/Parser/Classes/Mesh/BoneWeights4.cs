using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.Meshes
{
	/// <summary>
	/// BoneInfluence in old versions
	/// </summary>
	public sealed class BoneWeights4 : IStreamReadable, IYAMLExportable
	{
		public void Read(EndianStream stream)
		{
			m_weight[0] = stream.ReadSingle();
			m_weight[1] = stream.ReadSingle();
			m_weight[2] = stream.ReadSingle();
			m_weight[3] = stream.ReadSingle();
			m_boneIndex[0] = stream.ReadInt32();
			m_boneIndex[1] = stream.ReadInt32();
			m_boneIndex[2] = stream.ReadInt32();
			m_boneIndex[3] = stream.ReadInt32();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("weight[0]", m_weight[0]);
			node.Add("weight[1]", m_weight[1]);
			node.Add("weight[2]", m_weight[2]);
			node.Add("weight[3]", m_weight[3]);
			node.Add("boneIndex[0]", m_boneIndex[0]);
			node.Add("boneIndex[1]", m_boneIndex[1]);
			node.Add("boneIndex[2]", m_boneIndex[2]);
			node.Add("boneIndex[3]", m_boneIndex[3]);
			return node;
		}

		public IReadOnlyList<float> Weight => m_weight;
		public IReadOnlyList<int> BoneIndex => m_boneIndex;

		private readonly float[] m_weight = new float[4];
		private readonly int[] m_boneIndex = new int[4];
	}
}
