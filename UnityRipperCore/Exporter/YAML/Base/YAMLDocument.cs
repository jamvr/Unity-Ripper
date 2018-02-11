using System;

namespace UnityRipper.Exporter.YAML
{
	public sealed class YAMLDocument
	{
		public YAMLDocument(YAMLNode root)
		{
			if(root == null)
			{
				throw new ArgumentNullException(nameof(root));
			}
			Root = root;
		}

		internal void Emit(Emitter emitter, bool isSeparator)
		{
			if(isSeparator)
			{
				emitter.Write("---").WriteWhitespace();
			}

			Root.Emit(emitter);
		}

		public YAMLNode Root { get; }
	}
}
