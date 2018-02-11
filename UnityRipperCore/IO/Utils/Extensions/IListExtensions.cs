using System;
using System.Collections.Generic;

namespace UnityRipper
{
	public static class IListExtensions
	{
		public static void Read<T>(this IList<T> _this, EndianStream stream)
			where T : IStreamReadable, new()
		{
			int count = stream.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				T value = new T();
				value.Read(stream);
				_this.Add(value);
			}
		}

		public static void Read<T>(this IList<T> _this, EndianStream stream, Func<T> instantiator)
			where T : IStreamReadable
		{
			int count = stream.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				T value = instantiator();
				value.Read(stream);
				_this.Add(value);
			}
		}
	}
}
