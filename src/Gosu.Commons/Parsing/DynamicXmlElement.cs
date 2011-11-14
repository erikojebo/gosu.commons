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
        private readonly ConverterRegistry _converterRegistry;
        private readonly NamespaceRegistry _namespaceRegistry;

        public DynamicXmlElement(XElement element, ConverterRegistry converterRegistry,
            NamespaceRegistry namespaceRegistry)
        {
            _element = element;
            _converterRegistry = converterRegistry;
            _namespaceRegistry = namespaceRegistry;
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
            var namespaceChildElement = GetNamespaceChildElement(name);
            var defaultNamespaceChildElement = GetDefaultNamespaceChildElement(name);
            var namespaceChildElements = GetNamespaceChildElements(name);
            var defaultNamespaceChildElements = GetDefaultChildElements(name);

            if (attribute != null)
            {
                var convertibleValue = new ConvertibleStringValue(attribute.Value, _converterRegistry);
                return new SuccessfulInvocationResult(convertibleValue);
            }
            if (childElement != null)
            {
                return new SuccessfulInvocationResult(new DynamicXmlElement(childElement, _converterRegistry, _namespaceRegistry));
            }
            if (defaultNamespaceChildElement != null)
            {
                return new SuccessfulInvocationResult(new DynamicXmlElement(defaultNamespaceChildElement, _converterRegistry, _namespaceRegistry));
            }
            if (namespaceChildElement != null)
            {
                return new SuccessfulInvocationResult(new DynamicXmlElement(namespaceChildElement, _converterRegistry, _namespaceRegistry));
            }
            if (childElements.Any())
            {
                return new SuccessfulInvocationResult(childElements);
            }
            if (defaultNamespaceChildElements.Any())
            {
                return new SuccessfulInvocationResult(defaultNamespaceChildElements);
            }
            if (namespaceChildElements.Any())
            {
                return new SuccessfulInvocationResult(namespaceChildElements);
            }

            throw new MissingValueException(name);
        }

        private XElement GetNamespaceChildElement(string name)
        {
            var xmlNamespace = _namespaceRegistry.GetNamespace(name);

            if (xmlNamespace.IsNothing)
            {
                return null;
            }

            return _element.Element(xmlNamespace.Value.GetXName(name));
        }

        private IEnumerable<DynamicXmlElement> GetDefaultChildElements(string name)
        {
            var ns = _namespaceRegistry.GetDefaultNamespace();

            if (ns.IsNothing)
            {
                return null;
            }

            return GetChildElements(name, x => ns.Value.ToXNamespace() + x);
        }

        private IEnumerable<DynamicXmlElement> GetChildElements(string name)
        {
            return GetChildElements(name, x => x);
        }

        private IEnumerable<DynamicXmlElement> GetNamespaceChildElements(string name)
        {
            var xmlNamespace = _namespaceRegistry.GetNamespace(name);

            if (xmlNamespace.IsNothing)
            {
                return new List<DynamicXmlElement>();
            }

            var xNamespace = xmlNamespace.Value.ToXNamespace();
            var nameWithoutNamespace = xmlNamespace.Value.GetNameWithoutNamespacePrefix(name);

            return GetChildElements(nameWithoutNamespace, x => xNamespace + x);
        }

        private XElement GetDefaultNamespaceChildElement(string name)
        {
            var defaultNamespace = _namespaceRegistry.GetDefaultNamespace();

            if (defaultNamespace.IsNothing)
            {
                return null;
            }

            return _element.Element(defaultNamespace.Value.GetXName(name));
        }

        private IEnumerable<DynamicXmlElement> GetChildElements(string name, Func<string, XName> getQualifiedName)
        {
            var childName = name.TrimEnd('s');

            var children = _element.Elements(getQualifiedName(childName));

            if (children.IsEmpty() && name.EndsWith("Elements"))
            {
                var substring = name.TrimEnd("Elements");
                children = _element.Elements(getQualifiedName(substring));
            }

            if (children.IsEmpty() && name.EndsWith("ies"))
            {
                var substring = name.TrimEnd("ies") + "y";
                children = _element.Elements(getQualifiedName(substring));
            }

            if (children.IsEmpty() && name.EndsWith("es"))
            {
                var substring = name.TrimEnd("es");
                children = _element.Elements(getQualifiedName(substring));
            }

            return GetDynamicXmlElements(children);
        }

        private List<DynamicXmlElement> GetDynamicXmlElements(IEnumerable<XElement> elements)
        {
            return elements.Select(x => new DynamicXmlElement(x, _converterRegistry, _namespaceRegistry)).ToList();
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
    }
}