using System;
using System.Collections.Generic;
using System.IO;
using UnityRipper.AssetExporters.Classes;

using Object = UnityRipper.Classes.Object;

namespace UnityRipper.AssetsFiles
{
	internal class AssetsFile : IAssetsFile
	{
		public AssetsFile(IAssetCollection collection, string filepath)
		{
			if(collection == null)
			{
				throw new ArgumentNullException(nameof(collection));
			}
			if (string.IsNullOrEmpty(filepath))
			{
				throw new ArgumentException("filepath");
			}

			m_collection = collection;
			FilePath = filepath;
			Name = Path.GetFileNameWithoutExtension(FilePath);
			if(Name == string.Empty)
			{
				throw new ArgumentException($"Can't obtain name from fullname {filepath}", "fullname");
			}
		}

		public void Load(string assetPath)
		{
			if (!File.Exists(assetPath))
			{
				throw new Exception($"Asset at path '{assetPath}' doesn't exist");
			}

			using (FileStream stream = File.OpenRead(assetPath))
			{
				Parse(stream);
				if (stream.Position != stream.Length)
				{
					//throw new Exception($"Read {read} but expected {m_length}");
				}
			}
		}

		public void Parse(byte[] buffer)
		{
			using (MemoryStream memStream = new MemoryStream(buffer))
			{
				Parse(memStream);
				if (memStream.Position != buffer.Length)
				{
					//throw new Exception($"Read {read} but expected {m_length}");
				}
			}
		}

		public void Parse(Stream baseStream)
		{
			Reset();
			using (EndianStream stream = new EndianStream(baseStream, EndianType.BigEndian))
			{
				stream.StartPosition = baseStream.Position;
				int tableSize = stream.ReadInt32();
				if (tableSize <= 0)
				{
					throw new Exception($"Invalid table size {tableSize} for asset file {Name}");
				}
				int dataEnd = stream.ReadInt32();
				if (dataEnd <= 0)
				{
					throw new Exception($"Invalid data end {dataEnd} for asset file {Name}");
				}
				Generation = (FileGeneration)stream.ReadInt32();
				if (!Enum.IsDefined(typeof(FileGeneration), Generation))
				{
					throw new Exception($"Unsuported file generation {Generation} for asset file '{Name}'");
				}
				long dataOffset = stream.StartPosition + stream.ReadUInt32();

				//reference itself because sharedFileIDs start from 1
				AssetsFilePtr dependency = new AssetsFilePtr(Name, string.Empty);
				m_dependencies.Add(dependency);

				if (IsTableAtTheEnd)
				{
					stream.BaseStream.Position = dataEnd - tableSize;
					stream.BaseStream.Position++;
				}
				else
				{
					stream.BaseStream.Position += 4;
				}

				if (IsReadVersion)
				{
					string version = stream.ReadStringZeroTerm();
					Version.Parse(version);
				}
				if (IsReadPlatform)
				{
					Platform = (Platform)stream.ReadUInt32();

					// reverse stream endian
					if ((uint)Platform > byte.MaxValue)
					{
						Platform = (Platform)BitConverterExtensions.Reverse((uint)Platform);
						stream.EndianType = EndianType.LittleEndian;
					}

					if (!Enum.IsDefined(typeof(Platform), Platform))
					{
						throw new Exception($"Unsuported platform {Platform} for asset file '{Name}'");
					}
				}
				if (IsReadBaseDefinitions)
				{
					BaseDefinitions = stream.ReadBoolean();
				}

				int baseCount = stream.ReadInt32();
				for (int i = 0; i < baseCount; i++)
				{
					if (IsBase5)
					{
						ReadBase5(stream);
					}
					else
					{
						ReadBase(stream);
					}
				}

				if (IsSkipZero)
				{
					stream.BaseStream.Position += 4;
				}

				int assetCount = stream.ReadInt32();
				if (assetCount < 0)
				{
					throw new Exception($"Invalid asset count {assetCount} for asset file {Name}");
				}
				
				for (int i = 0; i < assetCount; i++)
				{
					if (IsNeedAlign)
					{
						stream.AlignStream(AlignType.Align4);
					}

					long pathID;
					if (IsLongPathID)
					{
						pathID = stream.ReadInt64();
					}
					else
					{
						pathID = stream.ReadInt32();
					}
					long offset = dataOffset + stream.ReadUInt32();
					int size = stream.ReadInt32();

					ClassIDMap classMap;
					if (IsReadSubClass)
					{
						int index = stream.ReadInt32();
						classMap = m_classIDs[index];
					}
					else
					{
						int uniqueID = stream.ReadInt32();
						int classID = stream.ReadUInt16();
						classMap = new ClassIDMap(uniqueID, classID);
						stream.BaseStream.Position += 2;
					}

					if (IsSkipUnknown)
					{
						//this is a single byte, not an int32
						//the next entry is aligned after this
						//but not the last!
						stream.BaseStream.Position++;
					}
					
					AssetInfo assetInfo = new AssetInfo(this, pathID, classMap);
					AssetPreloadData asset = new AssetPreloadData(assetInfo, offset, size);
					m_assetPreloads.Add(pathID, asset);
				}

				if (IsReadPreload)
				{
					//this looks like a list of assets that need to be preloaded in memory before anytihng else
					int count = stream.ReadInt32();
					for (int i = 0; i < count; i++)
					{
						int num1 = stream.ReadInt32();
						stream.AlignStream(AlignType.Align4);
						long pathID = stream.ReadInt64();
					}
				}

				int dependenciesCount = stream.ReadInt32();
				for (int i = 0; i < dependenciesCount; i++)
				{
					string name = stream.ReadStringZeroTerm();
					stream.BaseStream.Position += 20;
					string fileName = stream.ReadStringZeroTerm();
					dependency = new AssetsFilePtr(fileName, name);
					m_dependencies.Add(dependency);
				}
				
				foreach(AssetPreloadData preload in m_assetPreloads.Values)
				{
					preload.Parse(stream);
				}
			}
		}

		public void Reset()
		{
			m_dependencies.Clear();
			m_classIDs.Clear();
			m_classStructs.Clear();
			m_assetPreloads.Clear();
		}
		
		public Object GetObject(int fileIndex, long pathID)
		{
			return FindObject(fileIndex, pathID, false);
		}

		public Object GetObject(long pathID)
		{
			Object @object = FindObject(pathID);
			if(@object == null)
			{
				throw new Exception($"Object with path ID {pathID} wasn't found");
			}

			return @object;
		}

		public Object FindObject(int fileIndex, long pathID)
		{
			return FindObject(fileIndex, pathID, true);
		}

		public Object FindObject(long pathID)
		{
			if(m_assetPreloads.TryGetValue(pathID, out AssetPreloadData preload))
			{
				return preload.Object;
			}
			return null;
		}

		public AssetInfo GetAssetInfo(long pathID)
		{
			return m_assetPreloads[pathID].AssetInfo;
		}

		public AssetType ToExportType(ClassIDType unityType)
		{
			return m_collection.ToExportType(unityType);
		}

		public string GetExportID(Object @object)
		{
			return m_collection.GetExportID(@object);
		}

		public ExportPointer CreateExportPointer(Object @object)
		{
			return m_collection.CreateExportPointer(@object);
		}

		public IEnumerable<Object> FetchObjects()
		{
			foreach(AssetPreloadData preload in m_assetPreloads.Values)
			{
				if(preload.Object != null)
				{
					yield return preload.Object;
				}
			}
		}

		public override string ToString()
		{
			return Name;
		}

		private void ReadBase(EndianStream stream)
		{
			ClassStruct classStruct = new ClassStruct();
			classStruct.ClassID = stream.ReadInt32();
			classStruct.BaseType = stream.ReadStringZeroTerm();
			classStruct.BaseName = stream.ReadStringZeroTerm();

			List<ClassMember> members = new List<ClassMember>();
			stream.BaseStream.Position += 20;
			int memberCount = stream.ReadInt32();
			for(int i = 0; i < memberCount; i++)
			{
				ReadClassMember(stream, members, 0);
			}
			classStruct.Members = members.ToArray();

			AddClassStruct(classStruct);
		}

		private void ReadBase5(EndianStream stream)
		{
			int classID = stream.ReadInt32();
			if(IsReadSubClass)
			{
				// same class ID with different subclasses coored several times but we need unique class ID
				// so create unique class ID and store corresponding class ID

				stream.BaseStream.Position++;

				int uniqueClassID;
				int subClassID = stream.ReadInt16();
				if (subClassID == -1)
				{
					// no subclass. that meen that class ID is unique by itself
					uniqueClassID = classID;
				}
				else if (subClassID >= 0)
				{
					// ordinal class ID should be >= 0, so we can use whole negative int range
					uniqueClassID = -1 - subClassID;
				}
				else
				{
					throw new Exception($"Invalid type value {subClassID} for asset file {Name}");
				}

				ClassIDMap id = new ClassIDMap(uniqueClassID, classID);
				m_classIDs.Add(id);

				if(classID == (int)ClassIDType.MonoBehaviour)
				{
					stream.BaseStream.Position += 16;
				}
				classID = uniqueClassID;
			}
			else if(classID < 0)
			{
				stream.BaseStream.Position += 16;
			}
			stream.BaseStream.Position += 16;

			if (BaseDefinitions)
			{
				ClassStruct classStruct = new ClassStruct();
				classStruct.ClassID = classID;

				int memberCount = stream.ReadInt32();
				if(memberCount < 0)
				{
					throw new Exception($"Invalid member count {memberCount}");
				}
				int stringSize = stream.ReadInt32();
				if (stringSize < 0)
				{
					throw new Exception($"Invalid string size {stringSize}");
				}

				long stringPosition = stream.BaseStream.Position + memberCount * ClassMember.ClassSize;

				List<ClassMember> members = new List<ClassMember>();
				for(int i = 0; i < memberCount; i++)
				{
					ReadClassMember5(stream, classStruct, members, stringPosition);
				}
				stream.BaseStream.Position += stringSize;
				classStruct.Members = members.ToArray();

				AddClassStruct(classStruct);
			}
		}

		private void ReadClassMember(EndianStream stream, List<ClassMember> members, int level)
		{
			ClassMember member = new ClassMember();
			member.Level = level;
			member.Type = stream.ReadStringZeroTerm();
			member.Name = stream.ReadStringZeroTerm();
			member.Size = stream.ReadInt32();
			member.Index = stream.ReadInt32();
			member.IsArray = stream.ReadInt32() != 0;
			member.Num0 = stream.ReadInt32();
			member.Flag = stream.ReadInt32();
			member.ChildCount = stream.ReadInt32();

			members.Add(member);
			for(int i = 0; i < member.ChildCount; i++)
			{
				ReadClassMember(stream, members, level + 1);
			}
		}

		private void ReadClassMember5(EndianStream stream, ClassStruct classStruct, List<ClassMember> members, long stringPosition)
		{
			ushort num0 = stream.ReadUInt16();
			byte level = stream.ReadByte();
			bool isArray = stream.ReadBoolean();

			string memberType = ReadClassType(stream, stringPosition);
			string memberName = ReadClassType(stream, stringPosition);

			int size = stream.ReadInt32();
			int index = stream.ReadInt32();
			int flag = stream.ReadInt32();

			if (index == 0)
			{
				classStruct.BaseType = memberType;
				classStruct.BaseName = memberName;
			}
			else
			{
				ClassMember member = new ClassMember();
				member.Level = level;
				member.Type = memberType;
				member.Name = memberName;
				member.Size = size;
				member.Index = index;
				member.IsArray = isArray;
				member.Num0 = num0;
				member.Flag = flag;
				members.Add(member);
			}
		}

		private string ReadClassType(EndianStream stream, long stringPosition)
		{
			ushort typeIndex = stream.ReadUInt16();
			bool isCustomType = stream.ReadUInt16() == 0;
			if (isCustomType)
			{
				//memTypeIndex is an offset in the string block
				long memberPosition = stream.BaseStream.Position;
				stream.BaseStream.Position = stringPosition + typeIndex;
				string typeName = stream.ReadStringZeroTerm();
				stream.BaseStream.Position = memberPosition;
				return typeName;
			}
			else
			{
				//memTypeIndex is an index in an internal strig array
				SerializeClassType classType = (SerializeClassType)typeIndex;
				if (!Enum.IsDefined(typeof(SerializeClassType), classType))
				{
					throw new Exception($"Unsupported asset class type name '{classType}' for asset file '{Name}'");
				}
				return classType.ToTypeString();
			}
		}

		private void AddClassStruct(ClassStruct classStruct)
		{
			int classID = classStruct.ClassID;
			if (m_classStructs.ContainsKey(classID))
			{
#warning TODO: unknown behaviour. why does asset contains same files with same ID?
				if(Version.IsEqual(2017, 1, 0, VersionType.Patch, 5))
				{
					return;
				}
				throw new Exception($"Asset file {Name} already contains class struct with class id {classID}");
			}
			m_classStructs.Add(classID, classStruct);
		}

		private Object FindObject(int fileIndex, long pathID, bool isSafe)
		{
			if (fileIndex >= m_dependencies.Count)
			{
				if(isSafe)
				{
					return null;
				}
				throw new Exception($"AssetsFile with index {fileIndex} was not found in dependencies");
			}

			AssetsFilePtr filePtr = m_dependencies[fileIndex];
			IAssetsFile file = m_collection.FindAssetsFile(filePtr);
			if (file == null)
			{
				if(isSafe)
				{
					return null;
				}
				throw new Exception($"AssetsFile with index {fileIndex} was not found in collection");
			}

			Object @object = file.FindObject(pathID);
			if (@object == null)
			{
				if(isSafe)
				{
					return null;
				}
				throw new Exception($"Object with path ID {pathID} was not found");
			}
			return @object;
		}

		public string Name { get; }
		public string FilePath { get; }
		public FileGeneration Generation { get; private set; }
		public Version Version { get; } = new Version();
		public Platform Platform { get; private set; }

		public IReadOnlyList<AssetsFilePtr> Dependencies => m_dependencies;
		public IReadOnlyCollection<AssetPreloadData> PreloadData => m_assetPreloads.Values;

		public IAssetCollection Collection => m_collection;

		/// <summary>
		/// 3.0.0b and greater
		/// </summary>
		public bool IsReadVersion => Generation >= FileGeneration.FG_300Beta;
		/// <summary>
		/// 3.0.0 and greater
		/// </summary>
		public bool IsReadPlatform => Generation >= FileGeneration.FG_300_342;
		/// <summary>
		/// 2.5.0 to 2.6.1
		/// </summary>
		public bool IsReadBuildSettings => Generation == FileGeneration.FG_250_261;

		private bool BaseDefinitions { get; set; }

		/// <summary>
		/// Less than 3.0.0
		/// </summary>
		private bool IsTableAtTheEnd => Generation <= FileGeneration.FG_300_342;
		private bool IsReadSubClass => Generation >= FileGeneration.FG_5unknown;
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		private bool IsReadBaseDefinitions => Generation >= FileGeneration.FG_500;
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		private bool IsBase5 => Generation >= FileGeneration.FG_500;
		/// <summary>
		/// 3.0.0b to 4.x.x
		/// </summary>
		private bool IsSkipZero => Generation >= FileGeneration.FG_300Beta && Generation < FileGeneration.FG_500;
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		private bool IsNeedAlign => Generation >= FileGeneration.FG_500;
		/// <summary>
		/// 5.0.0 anf greater
		/// </summary>
		private bool IsLongPathID => Generation >= FileGeneration.FG_500;
		/// <summary>
		/// 5.0.1 to 5.4.x
		/// </summary>
		private bool IsSkipUnknown => Generation == FileGeneration.FG_501_54;
		/// <summary>
		/// 5.0.0 and greater
		/// </summary>
		private bool IsReadPreload => Generation >= FileGeneration.FG_500;

		private readonly List<AssetsFilePtr> m_dependencies = new List<AssetsFilePtr>();
		private readonly List<ClassIDMap> m_classIDs = new List<ClassIDMap>();
		private readonly Dictionary<int, ClassStruct> m_classStructs = new Dictionary<int, ClassStruct>();
		private readonly Dictionary<long, AssetPreloadData> m_assetPreloads = new Dictionary<long, AssetPreloadData>();

		private readonly IAssetCollection m_collection;
	}
}
