using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes.AnimationClips
{
	public sealed class AnimationEvent : IStreamReadable, IYAMLExportable
	{
		public AnimationEvent(IAssetsFile assetsFile)
		{
			if (assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}
			ObjectReferenceParameter = new PPtr<Object>(assetsFile);
		}

		public void Read(EndianStream stream)
		{
			Time = stream.ReadSingle();

			FunctionName = stream.ReadStringAligned();
			StringParameter = stream.ReadStringAligned();
			ObjectReferenceParameter.Read(stream);

			FloatParameter = stream.ReadSingle();
			IntParameter = stream.ReadInt32();
			MessageOptions = stream.ReadInt32();
		}

		public YAMLNode ExportYAML()
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("time", Time);
			node.Add("functionName", FunctionName);
			node.Add("data", StringParameter);
			node.Add("objectReferenceParameter", ObjectReferenceParameter.ExportYAML());
			node.Add("floatParameter", FloatParameter);
			node.Add("intParameter", IntParameter);
			node.Add("messageOptions", MessageOptions);
			return node;
		}

		public IEnumerable<Object> FetchDependencies(bool isLog = false)
		{
			if(!ObjectReferenceParameter.IsNull)
			{
				Object @object = ObjectReferenceParameter.FindObject();
				if (@object == null)
				{
					if (isLog)
					{
						Logger.Log(LogType.Warning, LogCategory.Export, $"AnimationEvent's objectReferenceParameter {ObjectReferenceParameter.ToLogString()} wasn't found ");
					}
				}
				else
				{
					yield return @object;
				}
			}
		}

		public float Time { get; private set; }
		public string FunctionName { get; private set; }
		public string StringParameter { get; private set; }
		public PPtr<Object> ObjectReferenceParameter { get; }
		public float FloatParameter { get; private set; }
		public int IntParameter { get; private set; }
		public int MessageOptions { get; private set; }
	}
}
