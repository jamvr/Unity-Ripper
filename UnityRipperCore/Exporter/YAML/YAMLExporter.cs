using System.Collections.Generic;
using System.IO;

namespace UnityRipper.Exporter.YAML
{
	public static class YAMLExporter
	{
		public static void Export(IYAMLDocExportable @object, string path)
		{
			Export(@object, path, true);
		}

		public static void Export(IYAMLDocExportable @object, string path, bool withMetaInfo)
		{
			string directory = Path.GetDirectoryName(path);
			if(!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			using (FileStream fileStream = File.Open(path, FileMode.Create, FileAccess.Write))
			{
				using (StreamWriter streamWriter = new StreamWriter(fileStream))
				{
					Export(@object, streamWriter, withMetaInfo);
				}
			}
		}

		public static void Export(IYAMLDocExportable @object, StreamWriter streamWriter)
		{
			Export(@object, streamWriter, true);
		}

		public static void Export(IYAMLDocExportable @object, StreamWriter streamWriter, bool withMetaInfo)
		{
			YAMLWriter yamlWriter = new YAMLWriter();
			yamlWriter.IsWriteVersion = withMetaInfo;
			yamlWriter.IsWriteDefaultTag = withMetaInfo;
			YAMLDocument document = @object.ExportYAMLDocument();
			yamlWriter.Add(document);
			yamlWriter.Write(streamWriter);
		}


		public static void Export(IEnumerable<IYAMLDocExportable> objects, string path)
		{
			Export(objects, path, true);
		}

		public static void Export(IEnumerable<IYAMLDocExportable> objects, string path, bool withMetaInfo)
		{
			string directory = Path.GetDirectoryName(path);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			using (FileStream fileStream = File.Open(path, FileMode.Create, FileAccess.Write))
			{
				using (StreamWriter streamWriter = new StreamWriter(fileStream))
				{
					Export(objects, streamWriter, withMetaInfo);
				}
			}
		}

		public static void Export(IEnumerable<IYAMLDocExportable> objects, StreamWriter streamWriter)
		{
			Export(objects, streamWriter, true);
		}

		public static void Export(IEnumerable<IYAMLDocExportable> objects, StreamWriter streamWriter, bool withMetaInfo)
		{
			YAMLWriter yamlWriter = new YAMLWriter();
			yamlWriter.IsWriteVersion = withMetaInfo;
			yamlWriter.IsWriteDefaultTag = withMetaInfo;
			foreach (IYAMLDocExportable docExport in objects)
			{
				YAMLDocument document = docExport.ExportYAMLDocument();
				yamlWriter.Add(document);
			}
			yamlWriter.Write(streamWriter);
		}
	}
}
