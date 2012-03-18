using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Explora_Precios.Core.Helper
{
    public static class XmlExtensions
    {
        public static bool AttributeExists(this XmlReader xml, string tag, string attribute, string value, bool attributeEqual)
        {
            return xml.Name == tag && xml.HasAttributes && xml.GetAttribute(attribute) != null && !attributeEqual ? xml.GetAttribute(attribute).Contains(value) : xml.GetAttribute(attribute) == value;
        }

        public static bool IsTagElement(this XmlReader xml, string tag)
        {
            return xml.NodeType == XmlNodeType.Element && xml.Name == tag;
        }

        public static bool IsTagEndElement(this XmlReader xml, string tag)
        {
            return xml.NodeType == XmlNodeType.EndElement && xml.Name == tag;
        }

    }
}
