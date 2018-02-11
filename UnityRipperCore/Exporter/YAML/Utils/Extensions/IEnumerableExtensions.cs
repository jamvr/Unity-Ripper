using System;
using System.Collections.Generic;
using System.Text;

namespace UnityRipper.Exporter.YAML
{
	public static class IEnumerableExtensions
	{
		public static YAMLNode ExportYAML(this IEnumerable<bool> _this)
		{
			throw new NotImplementedException();
			/*YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Block;
			foreach (bool value in _this)
			{
				node.Add(value);
			}
			return node;*/
		}

		public static YAMLNode ExportYAML(this IEnumerable<byte> _this)
		{
			YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Raw;
			foreach(byte value in _this)
			{
				node.Add(value);
			}
			return node;
		}

		public static YAMLNode ExportYAML(this IEnumerable<ushort> _this)
		{
			throw new NotImplementedException();
			/*YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Block;
			foreach (ushort value in _this)
			{
				node.Add(value);
			}
			return node;*/
		}

		public static YAMLNode ExportYAML(this IEnumerable<short> _this)
		{
			throw new NotImplementedException();
			/*YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Block;
			foreach (short value in _this)
			{
				node.Add(value);
			}
			return node;*/
		}

		public static YAMLNode ExportYAML(this IEnumerable<uint> _this, bool isRaw)
		{
			YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = isRaw ? SequenceStyle.Raw : SequenceStyle.Block;
			foreach (uint value in _this)
			{
				node.Add(value);
			}
			return node;
		}

		public static YAMLNode ExportYAML(this IEnumerable<int> _this, bool isRaw)
		{
			YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = isRaw ? SequenceStyle.Raw : SequenceStyle.Block;
			foreach (int value in _this)
			{
				node.Add(value);
			}
			return node;
		}

		public static YAMLNode ExportYAML(this IEnumerable<ulong> _this)
		{
			throw new NotImplementedException();
			/*YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Block;
			foreach (ulong value in _this)
			{
				node.Add(value);
			}
			return node;*/
		}

		public static YAMLNode ExportYAML(this IEnumerable<long> _this)
		{
			throw new NotImplementedException();
			/*YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Block;
			foreach (long value in _this)
			{
				node.Add(value);
			}
			return node;*/
		}

		public static YAMLNode ExportYAML(this IEnumerable<float> _this)
		{
#warning TODO: check
			//throw new NotImplementedException();
			YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Block;
			foreach (float value in _this)
			{
				node.Add(value);
			}
			return node;
		}

		public static YAMLNode ExportYAML(this IEnumerable<string> _this)
		{
			YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Block;
			foreach (string value in _this)
			{
				node.Add(value);
			}
			return node;
		}

		public static YAMLNode ExportYAML(this IEnumerable<IYAMLExportable> _this)
		{
			YAMLSequenceNode node = new YAMLSequenceNode();
			node.Style = SequenceStyle.Block;
			foreach (IYAMLExportable export in _this)
			{
				node.Add(export.ExportYAML());
			}
			return node;
		}

		private static readonly StringBuilder s_sb = new StringBuilder();
	}
}
