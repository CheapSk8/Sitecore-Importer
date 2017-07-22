using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Specialized.Content.Import
{
    public class XmlHelpers
    {
        public static string FindXPath(XmlNode node, bool ignoreErrors = false)
        {
            StringBuilder builder = new StringBuilder();
            while (node != null)
            {
                switch (node.NodeType)
                {
                    case XmlNodeType.Attribute:
                        builder.Insert(0, "/@" + node.Name);
                        node = ((XmlAttribute)node).OwnerElement;
                        break;
                    case XmlNodeType.Element:
                        int index = FindElementIndex((XmlElement)node, ignoreErrors);
                        if (index >= 0)
                        {
                            builder.Insert(0, "/" + node.Name + "[" + index + "]");
                            node = node.ParentNode;
                        }
                        break;
                    case XmlNodeType.Document:
                        return builder.ToString();
                    default:
                        if (!ignoreErrors)
                        {
                            throw new ArgumentException("Only elements and attributes are supported");
                        }
                        return string.Empty;
                }
            }
            if (!ignoreErrors)
            {
                throw new ArgumentException("Node was not in a document");
            }
            return string.Empty;
        }

        public static int FindElementIndex(XmlElement element, bool ignoreErrors = false)
        {
            XmlNode parentNode = element.ParentNode;
            if (parentNode is XmlDocument)
            {
                return 1;
            }
            XmlElement parent = (XmlElement)parentNode;
            int index = 1;
            foreach (XmlNode candidate in parent.ChildNodes)
            {
                if (candidate is XmlElement && candidate.Name == element.Name)
                {
                    if (candidate == element)
                    {
                        return index;
                    }
                    index++;
                }
            }
            if (!ignoreErrors)
            {
                throw new ArgumentException("Couldn't find element within parent");
            }
            return -1;
        }
    }

    public static class XmlExtensions
    {
        public static void IterateXmlNodes(this XmlDocument xmlDoc, Action<XmlNode> elementAction, bool includeAttributes = false)
        {
            if (xmlDoc != null && elementAction != null)
            {
                foreach (XmlNode node in xmlDoc.ChildNodes)
                {
                    IterateNode(node, elementAction, includeAttributes);
                }
            }
        }

        public static void IterateNode(XmlNode node, Action<XmlNode> elementAction, bool includeAttributes = false)
        {
            elementAction(node);

            if (node.Attributes != null && includeAttributes == true)
            {
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    IterateNode(attribute, elementAction);
                }
            }

            foreach (XmlNode childNode in node.ChildNodes)
            {
                IterateNode(childNode, elementAction, includeAttributes);
                
            }
        }
    }
}
