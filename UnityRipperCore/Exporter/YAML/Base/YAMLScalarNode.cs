//#define USE_HEX_SINGLE

using System;
using System.Globalization;

namespace UnityRipper.Exporter.YAML
{
	public sealed class YAMLScalarNode : YAMLNode
	{
		public YAMLScalarNode()
		{
		}

		public YAMLScalarNode(bool value) :
			this()
		{
			SetValue(value);
			Style = ScalarStyle.Plain;
		}

		public YAMLScalarNode(byte value) :
			this()
		{
			SetValue(value);
			Style = ScalarStyle.Plain;
		}

		public YAMLScalarNode(short value) :
			this()
		{
			SetValue(value);
			Style = ScalarStyle.Plain;
		}

		public YAMLScalarNode(ushort value) :
			this()
		{
			SetValue(value);
			Style = ScalarStyle.Plain;
		}

		public YAMLScalarNode(int value) :
			this()
		{
			SetValue(value);
			Style = ScalarStyle.Plain;
		}

		public YAMLScalarNode(uint value) :
			this()
		{
			SetValue(value);
			Style = ScalarStyle.Plain;
		}
		
		public YAMLScalarNode(long value) :
			this()
		{
			SetValue(value);
			Style = ScalarStyle.Plain;
		}

		public YAMLScalarNode(ulong value) :
			this()
		{
			SetValue(value);
			Style = ScalarStyle.Plain;
		}

		public YAMLScalarNode(float value) :
			this()
		{
			SetValue(value);
			Style = ScalarStyle.Plain;
		}

		public YAMLScalarNode(string value) :
			this()
		{
			SetValue(value);
			Style = GetStyle(value);
		}

		public void SetValue(bool value)
		{
			m_value = value;
			m_objectType = ScalarType.Boolean;
		}

		public void SetValue(byte value)
		{
			m_value = value;
			m_objectType = ScalarType.Byte;
		}

		public void SetValue(short value)
		{
			m_value = value;
			m_objectType = ScalarType.Int16;
		}

		public void SetValue(ushort value)
		{
			m_value = value;
			m_objectType = ScalarType.UInt16;
		}

		public void SetValue(int value)
		{
			m_value = value;
			m_objectType = ScalarType.Int32;
		}

		public void SetValue(uint value)
		{
			m_value = value;
			m_objectType = ScalarType.UInt32;
		}

		public void SetValue(long value)
		{
			m_value = value;
			m_objectType = ScalarType.Int64;
		}

		public void SetValue(ulong value)
		{
			m_value = value;
			m_objectType = ScalarType.UInt64;
		}

		public void SetValue(float value)
		{
			uint hex = BitConverterExtensions.ToUInt32(value);
#if USE_HEX_SINGLE
			// As Unity says it is more precise technic but output looks vague and less readable
			m_value = $"0x{hex.ToString("X8")}({value.ToString(CultureInfo.InvariantCulture)})";
#else
			m_value = value;
#endif
			m_objectType = ScalarType.Single;
		}

		public void SetValue(string value)
		{
			m_value = value;
			m_objectType = ScalarType.String;
		}

		internal override void Emit(Emitter emitter)
		{
			base.Emit(emitter);

			switch(Style)
			{
				case ScalarStyle.Hex:
				case ScalarStyle.Plain:
					emitter.Write(Value);
					break;

				case ScalarStyle.SingleQuoted:
					emitter.Write('\'');
					emitter.Write(Value);
					emitter.Write('\'');
					break;

				default:
					throw new Exception($"Unsupported scalar style {Style}");
			}
		}

		private ScalarStyle GetStyle(string value)
		{
			if(value.Contains(":"))
			{
				return ScalarStyle.SingleQuoted;
			}
			return ScalarStyle.Plain;
		}

		public static readonly YAMLScalarNode Empty = new YAMLScalarNode();

		public override YAMLNodeType NodeType => YAMLNodeType.Scalar;
		public override bool IsMultyline => false;
		public override bool IsIndent => false;

		public string Value
		{
			get
			{
				if (Style == ScalarStyle.Hex)
				{
					switch (m_objectType)
					{
						case ScalarType.Byte:
							return ((byte)m_value).ToString("x2");
						case ScalarType.Int16:
							return ((short)m_value).ToString("x4");
						case ScalarType.UInt16:
							return ((ushort)m_value).ToString("x4");
						case ScalarType.Int32:
							return ((int)m_value).ToString("x8");
						case ScalarType.UInt32:
							return ((uint)m_value).ToString("x8");
						case ScalarType.Int64:
							return ((long)m_value).ToString("x16");
						case ScalarType.UInt64:
							return ((ulong)m_value).ToString("x16");
						case ScalarType.Single:
							return ((float)m_value).ToString("x8");
						case ScalarType.Double:
							return ((double)m_value).ToString("x16");
						case ScalarType.String:
							return (string)m_value;
						default:
							throw new NotImplementedException(m_objectType.ToString());
					}
				}
				if(m_objectType == ScalarType.Boolean)
				{
					return (bool)m_value ? 1.ToString() : 0.ToString();
				}
				if(m_objectType == ScalarType.Single)
				{
					return ((float)m_value).ToString(CultureInfo.InvariantCulture);
				}
				if (m_objectType == ScalarType.Double)
				{
					return ((double)m_value).ToString(CultureInfo.InvariantCulture);
				}
				return m_value.ToString();
			}
			set { m_value = value; }
		}
		public ScalarStyle Style { get; set; }

		private ScalarType m_objectType = ScalarType.String;
		private object m_value = string.Empty;
	}
}
