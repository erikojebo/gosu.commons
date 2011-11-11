﻿using System;
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
        private readonly ConverterRegistry _converterRegistry = new ConverterRegistry();

        public DynamicXmlElement(XElement element)
        {
            _element = element;

            _converterRegistry.Register<int>(s => int.Parse(s));
            _converterRegistry.Register<double>(s => double.Parse(s));
            _converterRegistry.Register<decimal>(s => decimal.Parse(s));
            _converterRegistry.Register<float>(s => float.Parse(s));
            _converterRegistry.Register<DateTime>(s => DateTime.Parse(s));
            _converterRegistry.Register<TimeSpan>(s => TimeSpan.Parse(s));
            _converterRegistry.Register<bool>(s => bool.Parse(s));
            _converterRegistry.Register<String>(s => s);
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
                var convertibleValue = new ConvertibleStringValue(attribute.Value, _converterRegistry);
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
            var value = new ConvertibleStringValue(_element.Value, _converterRegistry);

            if (value.CanConvertTo(type))
            {
                return new SuccessfulInvocationResult(value.Convert(type));
            }

            return new FailedInvocationResult();
        }

        public void SetConverter(Type type, Func<string, object> converter)
        {
            _converterRegistry.Register(type, converter);
        }
    }
}