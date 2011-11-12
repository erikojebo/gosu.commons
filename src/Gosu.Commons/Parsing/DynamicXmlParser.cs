using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Gosu.Commons.Parsing
{
    public class DynamicXmlParser : IDynamicXmlParser
    {
        private ConverterRegistry _converterRegistry = new ConverterRegistry();

        public dynamic Parse(string xml)
        {
            var document = XDocument.Parse(xml);
            var dynamicXmlElement = new DynamicXmlElement(document.Root, _converterRegistry);

            return dynamicXmlElement;
        }

        public void SetConverter<T>(Func<string, T> converter)
        {
            Func<string, object> untypedConverter = x => converter(x);
            _converterRegistry.Register(typeof(T), untypedConverter);
        }

        public void SetNamespaceAlias(string uri, string alias)
        {
        }
    }
}