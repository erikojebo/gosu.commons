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

        public DynamicXmlElement(XElement element)
        {
            _element = element;
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
                return new SuccessfulInvocationResult(attribute.Value);
            }
            if (childElement != null)
            {
                return new SuccessfulInvocationResult(childElement.Value);
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

        public static implicit operator string (DynamicXmlElement x)
        {
            return x._element.Value;
        }

        public static implicit operator int (DynamicXmlElement x)
        {
            return int.Parse(x._element.Value);
        }

        public static implicit operator double (DynamicXmlElement x)
        {
            return double.Parse(x._element.Value);
        }
        
        public static implicit operator DateTime (DynamicXmlElement x)
        {
            return DateTime.Parse(x._element.Value);
        }

        public static implicit operator TimeSpan (DynamicXmlElement x)
        {
            return TimeSpan.Parse(x._element.Value);
        }
    }
}