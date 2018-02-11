using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimatorControllers
{
	public sealed class ValueArray : IStreamReadable, IYAMLExportable
	{
		public ValueArray(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			if(IsPrimeFirst)
			{
				m_boolValues = stream.ReadBooleanArray();
				stream.AlignStream(AlignType.Align4);

				m_intValues = stream.ReadInt32Array();
				m_floatValues = stream.ReadSingleArray();
			}

			if(IsVector3)
			{
				m_position3Values = stream.ReadArray<Vector3f>();
			}
			else
			{
				m_position4Values = stream.ReadArray<Vector4f>();
			}
			m_quaternionValues = stream.ReadArray<Quaternionf>();
			if (IsVector3)
			{
				m_scale3Values = stream.ReadArray<Vector3f>();
			}
			else
			{
				m_scale4Values = stream.ReadArray<Vector4f>();
			}

			if (!IsPrimeFirst)
			{
				m_floatValues = stream.ReadSingleArray();
				m_intValues = stream.ReadInt32Array();
				m_boolValues = stream.ReadBooleanArray();
				stream.AlignStream(AlignType.Align4);
			}
		}

		public YAMLNode ExportYAML()
		{
			throw new NotSupportedException();
		}

		public IReadOnlyList<Vector3f> Position3Values => m_position3Values;
		public IReadOnlyList<Vector4f> Position4Values => m_position4Values;
		public IReadOnlyList<Quaternionf> QuaternionValues => m_quaternionValues;
		public IReadOnlyList<Vector3f> Scale3Values => m_scale3Values;
		public IReadOnlyList<Vector4f> Scale4Values => m_scale4Values;
		public IReadOnlyList<float> FloatValues => m_floatValues;
		public IReadOnlyList<int> IntValues => m_intValues;
		public IReadOnlyList<bool> BoolValues => m_boolValues;

		/// <summary>
		/// 5.4.0 and greater
		/// </summary>
		private bool IsVector3 => Version.IsGreaterEqual(5, 4);
		/// <summary>
		/// Less than 5.5.0
		/// </summary>
		private bool IsPrimeFirst => Version.IsLess(5, 5);

		private Version Version => m_assetsFile.Version;

		private Vector3f[] m_position3Values;
		private Vector4f[] m_position4Values;
		private Quaternionf[] m_quaternionValues;
		private Vector3f[] m_scale3Values;
		private Vector4f[] m_scale4Values;
		private float[] m_floatValues;
		private int[] m_intValues;
		private bool[] m_boolValues; 

		private readonly IAssetsFile m_assetsFile;
	}
}
