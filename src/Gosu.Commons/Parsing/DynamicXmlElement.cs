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

            if (name.EndsWith("ies"))
            {
                var substring = name.TrimEnd("ies") + "y";

                children = _element.Elements(substring);
            }
            else if (name.EndsWith("es"))
            {
                var substring = name.TrimEnd("es");

                children = _element.Elements(substring);
            }

            if (!children.Any())
            {
                var childName = name.TrimEnd('s');

                children = _element.Elements(childName);
            }

            return children.Select(x => new DynamicXmlElement(x)).ToList();
        }
    }
}