using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.AnimationClips
{
	public sealed class HandPose : IStreamReadable
	{
		public HandPose(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			GrabX = new XForm(assetsFile);
		}

		public void Read(EndianStream stream)
		{
			GrabX.Read(stream);

			m_doFArray = stream.ReadSingleArray();
			Override = stream.ReadSingle();
			CloseOpen = stream.ReadSingle();
			InOut = stream.ReadSingle();
			Grab = stream.ReadSingle();
		}

		public XForm GrabX { get; }
		public IReadOnlyList<float> DoFArray => m_doFArray;
		public float Override { get; private set; }
		public float CloseOpen { get; private set; }
		public float InOut { get; private set; }
		public float Grab { get; private set; }

		private float[] m_doFArray;
	}
}
