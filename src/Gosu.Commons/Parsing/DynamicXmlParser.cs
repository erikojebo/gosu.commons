using System.Xml.Linq;

namespace Gosu.Commons.Parsing
{
    public class DynamicXmlParser : IDynamicXmlParser
    {
        public dynamic Parse(string xml)
        {
            var document = XDocument.Parse(xml);
            return new DynamicXmlElement(document.Root);
        }
    }
}