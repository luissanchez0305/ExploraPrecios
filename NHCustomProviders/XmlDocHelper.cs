using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace NHCustomProviders
{
	/// <summary>Helper class for creating NHibernate XML mapping documents.</summary>
	public static class XmlDocHelper
	{
		#region Methods

		/// <summary>Creates a xml document for mapping a class.</summary>
		/// <param name="className">Name of the class to map.</param>
		/// <param name="tableName">Name of the table to map.</param>
		/// <param name="classNode">The class node.</param>
		/// <returns>The class node with the name and table attributes set</returns>
		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
		public static XmlDocument CreateClassMappingDocument(string className, string tableName, out XmlElement classNode)
		{
			IDictionary<string, string> attributes = new Dictionary<string, string>();

			XmlDocument doc = new XmlDocument();
			XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
			doc.AppendChild(decl);

			// creates the root node
			attributes["namespace"] = "NHCustomProviders";
			attributes["assembly"] = "NHCustomProviders";
			XmlElement rootNode = CreateElementWithAttributes(doc, "hibernate-mapping", attributes);
			doc.AppendChild(rootNode);

			// creates a class
			attributes.Clear();
			attributes["name"] = className;
			attributes["table"] = tableName;
			classNode = CreateElementWithAttributes(doc, "class", attributes);
			rootNode.AppendChild(classNode);

			return doc;
		}

		/// <summary>Creates an Xml element with the specified attributes.</summary>
		/// <param name="doc">The XmlDocument where the element will be added.</param>
		/// <param name="elementName">Name of the element to create.</param>
		/// <param name="attributes">The attributes to add to the element.</param>
		/// <returns>The Xml element with the specified attributes.</returns>
		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
		public static XmlElement CreateElementWithAttributes(XmlDocument doc, string elementName, IDictionary<string, string> attributes)
		{
			XmlElement element = doc.CreateElement(elementName, "urn:nhibernate-mapping-2.2");
			foreach (KeyValuePair<string, string> pair in attributes) {
				XmlAttribute attribute = doc.CreateAttribute(pair.Key);
				attribute.Value = pair.Value;
				element.Attributes.Append(attribute);
			}

			return element;
		}

		/// <summary>Creates an Xml attribute with the specified name and value.</summary>
		/// <param name="doc">The XmlDocument where the element will be added.</param>
		/// <param name="attributeName">Name of the attribute to create.</param>
		/// <param name="attributeValue">The value of the attribute.</param>
		/// <returns>The Xml attribute.</returns>
		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
		public static XmlAttribute CreateAttribute(XmlDocument doc, string attributeName, string attributeValue)
		{
			XmlAttribute attribute = doc.CreateAttribute(attributeName);
			attribute.Value = attributeValue;

			return attribute;
		}

		#endregion
	}
}