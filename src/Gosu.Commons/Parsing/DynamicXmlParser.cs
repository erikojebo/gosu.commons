using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Gosu.Commons.Parsing
{
    public class DynamicXmlParser : IDynamicXmlParser
    {
        private ValueConverter _valueConverter = new ValueConverter();
        private NamespaceRegistry _namespaceRegistry = new NamespaceRegistry();

        public dynamic Parse(string xml)
        {
            var document = XDocument.Parse(xml);

            _namespaceRegistry.SetDefaultNamespace(document.Root.GetDefaultNamespace());

            var dynamicXmlElement = new DynamicXmlElement(document.Root, _valueConverter, _namespaceRegistry);

            return dynamicXmlElement;
        }

        public void SetConverter<T>(Func<string, T> converter)
        {
            Func<string, object> untypedConverter = x => converter(x);
            _valueConverter.Register(typeof(T), untypedConverter);
        }

        public void SetNamespaceAlias(string uri, string alias)
        {
            _namespaceRegistry.Register(uri, alias);
        }
    }
}