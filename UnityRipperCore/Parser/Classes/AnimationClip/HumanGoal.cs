using System;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.AnimationClips
{
	public sealed class HumanGoal : IStreamReadable
	{
		public HumanGoal(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}

			m_assetsFile = assetsFile;
			X = new XForm(assetsFile);
			if(IsReadHints)
			{
				if(IsVector3)
				{
					HintT3 = new Vector3f();
				}
				else
				{
					HintT4 = new Vector4f();
				}
			}
		}

		public void Read(EndianStream stream)
		{
			X.Read(stream);

			WeightT = stream.ReadSingle();
			WeightR = stream.ReadSingle();
			if(IsReadHints)
			{
				if(IsVector3)
				{
					HintT3.Read(stream);
				}
				else
				{
					HintT4.Read(stream);
				}
				HintWeightT = stream.ReadSingle();
			}
		}

		public XForm X { get; }
		public float WeightT { get; private set; }
		public float WeightR { get; private set; }
		public Vector3f HintT3 { get; private set; }
		public Vector4f HintT4 { get; private set; }
		public float HintWeightT { get; private set; }

		private Version Version => m_assetsFile.Version;

		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		private bool IsReadHints => Version.IsGreaterEqual(5);
		/// <summary>
		/// 5.4.0 and greater
		/// </summary>
		private bool IsVector3 => Version.IsGreaterEqual(5, 4);

		private readonly IAssetsFile m_assetsFile;
	}
}
