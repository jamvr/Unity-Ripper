using System;
using System.Collections.Generic;
using System.IO;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.AnimationClips
{
	public sealed class Clip : IStreamReadable
	{
		public Clip(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			
			Binding = new ValueArrayConstant(assetsFile);
		}

		public void Read(EndianStream stream)
		{
			StreamedClip.Read(stream);
			DenseClip.Read(stream);
			ConstantClip.Read(stream);
			Binding.Read(stream);
		}

		public bool IsValid => StreamedClip.IsValid || DenseClip.IsValid || ConstantClip.IsValid;

		public StreamedClip StreamedClip { get; } = new StreamedClip();
		public DenseClip DenseClip { get; } = new DenseClip();
		public ConstantClip ConstantClip { get; } = new ConstantClip();
		public ValueArrayConstant Binding { get; }
	}
}
