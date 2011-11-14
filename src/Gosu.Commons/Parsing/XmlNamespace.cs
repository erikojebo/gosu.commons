using System;
using System.Xml.Linq;

namespace Gosu.Commons.Parsing
{
    public class XmlNamespace
    {
        public XNamespace Uri { get; set; }
        public string Alias { get; set; }

        public XmlNamespace()
        {
            Alias = "";
        }

        public XNamespace ToXNamespace()
        {
            return Uri;
        }

        public string GetNameWithoutNamespacePrefix(string name)
        {
            return name.Substring(Alias.Length);
        }

        public XName GetXName(string name)
        {
            return ToXNamespace() + GetNameWithoutNamespacePrefix(name);
        }
    }
}