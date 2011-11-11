using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Gosu.Commons.Dynamics;
using Gosu.Commons.Extensions;

namespace Gosu.Commons.Parsing
{
    public class DynamicXmlElement : HookableDynamicObject
    {
        private readonly XElement _element;
        private readonly Dictionary<Type, Func<string, object>> _converters;

        public DynamicXmlElement(XElement element)
        {
            _element = element;

            _converters = new Dictionary<Type, Func<string, object>>
                {
                    { typeof(int), s => int.Parse(s) },
                    { typeof(double), s => double.Parse(s) },
                    { typeof(decimal), s => decimal.Parse(s) },
                    { typeof(float), s => float.Parse(s) },
                    { typeof(DateTime), s => DateTime.Parse(s) },
                    { typeof(TimeSpan), s => TimeSpan.Parse(s) },
                    { typeof(bool), s => bool.Parse(s) },
                    { typeof(String), s => s }
                };
        }

        public List<DynamicXmlElement> Elements(string name)
        {
            var elements = _element.Elements(name);
            return GetDynamicXmlElements(elements);
        }

        protected override InvocationResult PropertyMissing(string name)
        {
            var attribute = _element.Attribute(name);
            var childElement = _element.Element(name);
            var childElements = GetChildElements(name);

            if (attribute != null)
            {
                var convertibleValue = new ConvertibleStringValue(attribute.Value);
                return new SuccessfulInvocationResult(convertibleValue);
            }
            if (childElement != null)
            {
                return new SuccessfulInvocationResult(new DynamicXmlElement(childElement));
            }
            if (childElements.Any())
            {
                return new SuccessfulInvocationResult(childElements);
            }

            throw new MissingValueException(name);
        }

        private List<DynamicXmlElement> GetChildElements(string name)
        {
            IEnumerable<XElement> children = new List<XElement>();

            var childName = name.TrimEnd('s');

            children = _element.Elements(childName);

            if (children.IsEmpty() && name.EndsWith("Elements"))
            {
                var substring = name.TrimEnd("Elements");
                children = _element.Elements(substring);
            }

            if (children.IsEmpty() && name.EndsWith("ies"))
            {
                var substring = name.TrimEnd("ies") + "y";
                children = _element.Elements(substring);
            }

            if (children.IsEmpty() && name.EndsWith("es"))
            {
                var substring = name.TrimEnd("es");
                children = _element.Elements(substring);
            }

            return GetDynamicXmlElements(children);
        }

        private List<DynamicXmlElement> GetDynamicXmlElements(IEnumerable<XElement> elements)
        {
            return elements.Select(x => new DynamicXmlElement(x)).ToList();
        }

        protected override InvocationResult ConvertionMissing(Type type, ConvertionMode conversionMode)
        {
            if (_converters.ContainsKey(type))
            {
                return new SuccessfulInvocationResult(_converters[type](_element.Value));
            }

            return new FailedInvocationResult();
        }

        public void SetConverter(Type type, Func<string, object> converter)
        {
            _converters[type] = converter;
        }
    }
}