using System.Collections.Generic;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class Animator : Behaviour
	{
		public Animator(AssetInfo assetInfo):
			base(assetInfo)
		{
			Avatar = new PPtr<Avatar>(AssetsFile);
			Controller = new PPtr<RuntimeAnimatorController>(AssetsFile);
		}

		public override void Read(EndianStream stream)
		{
			base.Read(stream);

			Avatar.Read(stream);
			Controller.Read(stream);
			CullingMode = stream.ReadInt32();

			if(IsReadUpdateMode)
			{
				UpdateMode = stream.ReadInt32();
			}

			ApplyRootMotion = stream.ReadBoolean();

			if(IsReadAnimatePhisics)
			{
				AnimatePhisics = stream.ReadBoolean();
			}

			if(IsReadBoolFlags)
			{
				if (IsReadLinearVelocityBlending)
				{
					LinearVelocityBlending = stream.ReadBoolean();
				}
				stream.AlignStream(AlignType.Align4);

				HasTransformHierarchy = stream.ReadBoolean();
				AllowConstantClipSamplingOptimization = stream.ReadBoolean();
				stream.AlignStream(AlignType.Align4);
			}
		}

		public override IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(isLog))
			{
				yield return @object;
			}

			Avatar avatar = Avatar.FindObject();
			if (avatar == null)
			{
				if (isLog)
				{
					Logger.Log(LogType.Warning, LogCategory.Export, $"{ToLogString()} m_Avatar {Avatar.ToLogString()} wasn't found ");
				}
			}
			else
			{
				yield return avatar;
			}

			RuntimeAnimatorController runetime = Controller.FindObject();
			if (runetime == null)
			{
				if (isLog)
				{
					Logger.Log(LogType.Warning, LogCategory.Export, $"{ToLogString()} m_Controller {Controller.ToLogString()} wasn't found ");
				}
			}
			else
			{
				yield return runetime;
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot()
		{
#warning TODO:
			YAMLMappingNode node = base.ExportYAMLRoot();
			node.Add("m_Avatar", Avatar.ExportYAML());
			node.Add("m_Controller", Controller.ExportYAML());
			node.Add("m_CullingMode", CullingMode);
			node.Add("m_UpdateMode", UpdateMode);
			node.Add("m_ApplyRootMotion", ApplyRootMotion);
			node.Add("m_LinearVelocityBlending", LinearVelocityBlending);
			node.Add("m_HasTransformHierarchy", HasTransformHierarchy);
			node.Add("m_AllowConstantClipSamplingOptimization", AllowConstantClipSamplingOptimization);
			return node;
		}

		public PPtr<Avatar> Avatar { get; }
		public PPtr<RuntimeAnimatorController> Controller { get; }
		public int CullingMode { get; private set; }
		public int UpdateMode { get; private set; }
		public bool ApplyRootMotion { get; private set; }
		public bool AnimatePhisics { get; private set; }
		public bool LinearVelocityBlending { get; private set; }
		public bool HasTransformHierarchy { get; private set; }
		public bool AllowConstantClipSamplingOptimization { get; private set; }

#warning unknown version
		public bool IsReadUpdateMode => Version.IsGreaterEqual(4, 2);
#warning unknown version
		public bool IsReadAnimatePhisics => Version.IsLessEqual(4, 6);
#warning unknown version
		public bool IsReadBoolFlags => Version.IsGreaterEqual(4, 2);
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		public bool IsReadLinearVelocityBlending => Version.IsGreaterEqual(5);

		private int SerializedVersion
		{
			get
			{
#warning TODO: serialized version acording to read version (current 2017.3.0f3)
				return 3;
			}
		}
	}
}
