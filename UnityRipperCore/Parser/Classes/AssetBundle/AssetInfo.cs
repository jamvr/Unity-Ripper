using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;

namespace UnityRipper.Classes.AssetBundles
{
	public sealed class AssetInfo : IStreamReadable
	{
		public AssetInfo(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			Asset = new PPtr<Object>(assetsFile);
		}

		public void Read(EndianStream stream)
		{
			PreloadIndex = stream.ReadInt32();
			PreloadSize = stream.ReadInt32();
			Asset.Read(stream);
		}

		public IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			Object @object = Asset.FindObject();
			if(@object == null)
			{
				if(isLog)
				{
					Logger.Log(LogType.Warning, LogCategory.Export, $"AssetInfo's asset {Asset.ToLogString()} wasn't found ");
				}
			}
			else
			{
				yield return @object;
			}
		}

		public int PreloadIndex { get; private set; }
		public int PreloadSize { get; private set; }
		public PPtr<Object> Asset { get; }
	}
}
