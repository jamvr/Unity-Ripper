using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace UnityRipper.Classes.Shaders
{
	public sealed class SerializedProperty : IStreamReadable
	{
		public void Read(EndianStream stream)
		{
			Name = stream.ReadStringAligned();
			Description = stream.ReadStringAligned();
			m_attributes = stream.ReadStringArray();
			Type = (SerializedPropertyType)stream.ReadInt32();
			Flags = (SerializedPropertyFlag)stream.ReadUInt32();
			stream.Read(m_defValue, 0, m_defValue.Length);
			DefTexture.Read(stream);
		}

		public StringBuilder ToString(StringBuilder sb)
		{
			sb.AppendIntent(2);
			foreach(string attribute in Attributes)
			{
				sb.Append('[').Append(attribute).Append(']').Append(' ');
			}
			if(Flags.IsHideInEnspector())
			{
				sb.Append("[HideInInspector]").Append(' ');
			}
			if (Flags.IsPerRendererData())
			{
				sb.Append("[PerRendererData]").Append(' ');
			}
			if (Flags.IsNoScaleOffset())
			{
				sb.Append("[NoScaleOffset]").Append(' ');
			}
			if (Flags.IsNormal())
			{
				sb.Append("[Normal]").Append(' ');
			}
			if (Flags.IsHDR())
			{
				sb.Append("[HDR]").Append(' ');
			}
			if (Flags.IsGamma())
			{
				sb.Append("[Gamma]").Append(' ');
			}

			sb.Append(Name).Append(' ').Append('(');
			sb.Append('"').Append(Description).Append('"').Append(',').Append(' ');

			switch(Type)
			{
				case SerializedPropertyType.Color:
				case SerializedPropertyType.Vector:
					sb.Append(Type);
					break;

				case SerializedPropertyType.Int:
				//case SerializedPropertyType.Float:
					sb.Append("Float");
					break;

				case SerializedPropertyType.Range:
					sb.Append(SerializedPropertyType.Range).Append('(');
					sb.Append(DefValue[1].ToString(CultureInfo.InvariantCulture)).Append(',').Append(' ');
					sb.Append(DefValue[2].ToString(CultureInfo.InvariantCulture));
					sb.Append(')');
					break;

				case SerializedPropertyType._2D:
				//case SerializedPropertyType._3D:
				//case SerializedPropertyType.Cube:
					switch(DefTexture.TexDim)
					{
						case 1:
							sb.Append("any");
							break;
						case 2:
							sb.Append("2D");
							break;
						case 3:
							sb.Append("3D");
							break;
						case 4:
							sb.Append("Cube");
							break;
						default:
							throw new NotSupportedException("Texture dimension isn't supported");

					}
					break;

				default:
					throw new NotSupportedException($"Serialized property type {Type} isn't supported");
			}
			sb.Append(')').Append(' ').Append('=').Append(' ');

			switch(Type)
			{
				case SerializedPropertyType.Color:
				case SerializedPropertyType.Vector:
					sb.Append('(');
					sb.Append(m_defValue[0].ToString(CultureInfo.InvariantCulture)).Append(',');
					sb.Append(m_defValue[1].ToString(CultureInfo.InvariantCulture)).Append(',');
					sb.Append(m_defValue[2].ToString(CultureInfo.InvariantCulture)).Append(',');
					sb.Append(m_defValue[3].ToString(CultureInfo.InvariantCulture));
					sb.Append(')');
					break;

				case SerializedPropertyType.Int:
				//case SerializedPropertyType.Float:
				case SerializedPropertyType.Range:
					sb.Append(m_defValue[0].ToString(CultureInfo.InvariantCulture));
					break;

				case SerializedPropertyType._2D:
				//case SerializedPropertyType._3D:
				//case SerializedPropertyType.Cube:
					sb.Append('"').Append(DefTexture.DefaultName).Append('"').Append(' ').Append("{}");
					break;

				default:
					throw new NotSupportedException($"Serialized property type {Type} isn't supported");
			}
			sb.Append('\n');
			return sb;
		}

		public string Name { get; private set; }
		public string Description { get; private set; }
		public IReadOnlyList<string> Attributes => m_attributes;
		public SerializedPropertyType Type { get; private set; }
		public SerializedPropertyFlag Flags { get; private set; }
		public IReadOnlyList<float> DefValue => m_defValue;
		public SerializedTextureProperty DefTexture { get; } = new SerializedTextureProperty();

		private string[] m_attributes;
		private float[] m_defValue = new float[4];
	}
}
