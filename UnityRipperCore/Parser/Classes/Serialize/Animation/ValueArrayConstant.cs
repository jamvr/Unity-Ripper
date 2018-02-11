using System;
using System.Collections.Generic;
using UnityRipper.AssetsFiles;
using UnityRipper.Exporter.YAML;

namespace UnityRipper.Classes
{
	public sealed class ValueArrayConstant : IStreamReadable, IYAMLExportable
	{
		public ValueArrayConstant(IAssetsFile assetsFile)
		{
			if(assetsFile == null)
			{
				throw new ArgumentNullException(nameof(assetsFile));
			}

			m_assetsFile = assetsFile;
		}

		public void Read(EndianStream stream)
		{
			m_valueArray = stream.ReadArray(() => new ValueConstant(m_assetsFile));
		}

		public YAMLNode ExportYAML()
		{
			throw new NotSupportedException();
		}
		
		public IReadOnlyList<ValueConstant> ValueArray => m_valueArray;

		private readonly IAssetsFile m_assetsFile;

		private ValueConstant[] m_valueArray;
	}
}
