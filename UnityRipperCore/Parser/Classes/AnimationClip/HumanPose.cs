using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.AnimationClips
{
	public sealed class HumanPose : IStreamReadable
	{
		public HumanPose(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}

			m_assetsFile = assetsFile;
			RootX = new XForm(assetsFile);
			LeftHandPose = new HandPose(assetsFile);
			RightHandPose = new HandPose(assetsFile);

			if(IsVector3)
			{
				LookAtPosition3 = new Vector3f();
			}
			else
			{
				LookAtPosition4 = new Vector4f();
			}
		}

		public void Read(EndianStream stream)
		{
			RootX.Read(stream);
			if(IsVector3)
			{
				LookAtPosition3.Read(stream);
			}
			else
			{
				LookAtPosition4.Read(stream);
			}
			LookAtWeight.Read(stream);
			m_goalArray = stream.ReadArray(() => new HumanGoal(m_assetsFile));
			LeftHandPose.Read(stream);
			RightHandPose.Read(stream);
			m_doFArray = stream.ReadSingleArray();

			if(IsReadTArray)
			{
				if(IsVector3)
				{
					m_TDoFArray3 = stream.ReadArray<Vector3f>();
				}
				else
				{
					m_TDoFArray4 = stream.ReadArray<Vector4f>();
				}
			}
		}

		public XForm RootX { get; }
		public Vector3f LookAtPosition3 { get; }
		public Vector4f LookAtPosition4 { get; }
		public Vector4f LookAtWeight { get; } = new Vector4f();
		public IReadOnlyList<HumanGoal> GoalArray => m_goalArray;
		public HandPose LeftHandPose { get; }
		public HandPose RightHandPose { get; }
		public IReadOnlyList<float> DoFArray => m_doFArray;
		public IReadOnlyList<Vector3f> TDoFArray3 => m_TDoFArray3;
		public IReadOnlyList<Vector4f> TDoFArray4 => m_TDoFArray4;

		private Version Version => m_assetsFile.Version;

		/// <summary>
		/// 5.4.0 and greater
		/// </summary>
		private bool IsVector3 => Version.IsGreaterEqual(5, 4);
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		private bool IsReadTArray => Version.IsGreaterEqual(5);
		
		private HumanGoal[] m_goalArray;
		private float[] m_doFArray;
		private Vector3f[] m_TDoFArray3;
		private Vector4f[] m_TDoFArray4;

		private readonly IAssetsFile m_assetsFile;
	}
}
