using System;
using System.Collections.Generic;

namespace UnityRipper
{
	public static class EndianStreamExtensions
	{
		public static T[] ReadEnum32Array<T>(this EndianStream stream, Func<int, T> converter)
		{
			int count = stream.ReadInt32();
			T[] array = new T[count];
			for (int i = 0; i < count; i++)
			{
				int value = stream.ReadInt32();
				array[i] = converter(value);
			}
			return array;
		}

		public static KeyValuePair<int, uint>[] ReadInt32KVPUInt32Array(this EndianStream stream)
		{
			int count = stream.ReadInt32();
			KeyValuePair<int, uint>[] array = new KeyValuePair<int, uint>[count];
			for (int i = 0; i < count; i++)
			{
				int key = stream.ReadInt32();
				uint value = stream.ReadUInt32();
				KeyValuePair<int, uint> kvp = new KeyValuePair<int, uint>(key, value);
				array[i] = kvp;
			}
			return array;
		}

		public static KeyValuePair<string, T>[] ReadKVPStringArray<T>(this EndianStream stream)
			where T: IStreamReadable, new()
		{
			int count = stream.ReadInt32();
			KeyValuePair<string, T>[] array = new KeyValuePair<string, T>[count];
			for(int i = 0; i < count; i++)
			{
				string key = stream.ReadStringAligned();
				T value = new T();
				value.Read(stream);
				KeyValuePair<string, T> kvp = new KeyValuePair<string, T>(key, value);
				array[i] = kvp;
			}
			return array;
		}

		public static KeyValuePair<string, T>[] ReadStringKVPArray<T>(this EndianStream stream, Func<T> valueInstantiator)
			where T : IStreamReadable
		{
			int count = stream.ReadInt32();
			KeyValuePair<string, T>[] array = new KeyValuePair<string, T>[count];
			for (int i = 0; i < count; i++)
			{
				string key = stream.ReadStringAligned();
				T value = valueInstantiator();
				value.Read(stream);
				KeyValuePair<string, T> kvp = new KeyValuePair<string, T>(key, value);
				array[i] = kvp;
			}
			return array;
		}
	}
}
