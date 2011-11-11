using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Gosu.Commons.Parsing
{
    public class DynamicXmlParser : IDynamicXmlParser
    {
        private Dictionary<Type, Func<string, object>> _converters;

        public DynamicXmlParser()
        {
            _converters = new Dictionary<Type, Func<string, object>>();
        }

        public dynamic Parse(string xml)
        {
            var document = XDocument.Parse(xml);
            var dynamicXmlElement = new DynamicXmlElement(document.Root);

            foreach (var type in _converters.Keys)
            {
                dynamicXmlElement.SetConverter(type, _converters[type]);
            }

            return dynamicXmlElement;
        }

        public void SetConverter<T>(Func<string, T> converter)
        {
            Func<string, object> untypedConverter = x => converter(x);
            _converters[typeof(T)] = untypedConverter;
        }
    }
}