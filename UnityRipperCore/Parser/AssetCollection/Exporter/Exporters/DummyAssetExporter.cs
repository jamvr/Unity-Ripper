using System;
using UnityRipper.Classes;

using Object = UnityRipper.Classes.Object;

namespace UnityRipper.AssetExporters
{
	internal class DummyAssetExporter : IAssetExporter
	{
		public IExportCollection CreateCollection(Object @object)
		{
			MonoScript monoScript = @object as MonoScript;
			if(monoScript != null)
			{
				return new SkipExportCollection(this, monoScript, monoScript.ClassName);
			}
			AnimatorController animController = @object as AnimatorController;
			if (animController != null)
			{
				return new SkipExportCollection(this, animController);
			}

			AssetBundle bundle = (AssetBundle)@object;
			string name = bundle.IsReadAssetBundleName ? bundle.AssetBundleName : bundle.Name;
			return new EmptyExportCollection(this, name);
		}

		public void Export(IExportCollection collection, string dirPath)
		{
		}

		public AssetType ToExportType(ClassIDType classID)
		{
			switch (classID)
			{
				case ClassIDType.AnimatorController:
					return AssetType.Serialized;

				case ClassIDType.MonoScript:
					return AssetType.Meta;

				default:
					throw new NotSupportedException(classID.ToString());
			}
		}
	}
}
