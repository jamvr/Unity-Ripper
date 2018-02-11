using System;
using UnityRipper.AssetsFiles;

namespace UnityRipper
{
	public class AssetInfo
	{
		public AssetInfo(IAssetsFile assetFile, long pathID, ClassIDMap classMap)
		{
			if(assetFile == null)
			{
				throw new ArgumentNullException(nameof(assetFile));
			}
			AssetFile = assetFile;

			PathID = pathID;
			ClassMap = classMap;
		}

		public IAssetsFile AssetFile { get; }

		public long PathID { get; }
		public ClassIDMap ClassMap { get; }
	}
}
