using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public interface IPPtr<T> : IStreamReadable, IYAMLExportable
		where T: Object
	{
		T FindObject();
		T GetObject();

		bool IsNull { get; }
	}
}
