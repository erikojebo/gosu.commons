using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Gosu.Commons.Dynamics;

namespace Gosu.Commons.Parsing
{
    public class DynamicXmlElement : HookableDynamicObject
    {
        private readonly XElement _element;

        public DynamicXmlElement(XElement element)
        {
            _element = element;
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
            if (name.EndsWith("ies"))
            {
                var substring = name.Substring(0, name.Length - 3) + "y";

                var elements = _element.Elements(substring);

                if (elements.Any())
                {
                    return elements.Select(x => new DynamicXmlElement(x)).ToList();
                }
            }
            else if (name.EndsWith("es"))
            {
                var substring = name.Substring(0, name.Length - 2);

                var elements = _element.Elements(substring);

                if (elements.Any())
                {
                    return elements.Select(x => new DynamicXmlElement(x)).ToList();
                }
            }

            var childName = name.TrimEnd('s');

            var childElements = _element.Elements(childName);

            if (childElements.Any())
            {
                return childElements.Select(x => new DynamicXmlElement(x)).ToList();
            }

            return new List<DynamicXmlElement>();
        }
    }
}