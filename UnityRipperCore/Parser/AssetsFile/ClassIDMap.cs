using System;

namespace UnityRipper.AssetsFiles
{
	public struct ClassIDMap
	{
		public ClassIDMap(int uniqueID, int classID)
		{
			ClassIDType type = (ClassIDType)classID;
			if (!Enum.IsDefined(typeof(ClassIDType), type))
			{
				throw new Exception($"Unsupported class id {classID}");
			}

			UniqueID = uniqueID;
			ClassID = classID;
		}

		public override string ToString()
		{
			return $"[{UniqueID}, {ClassID}]";
		}

		public ClassIDType IDType => (ClassIDType)ClassID;

		/// <summary>
		/// e.g. different ids for different scripts but same ClassID MonoBehaviour
		/// </summary>
		public int UniqueID { get; }
		public int ClassID { get; }
	}
}
