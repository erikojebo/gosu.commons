using System.Xml.Linq;
using Gosu.Commons.Dynamics;
using System.Linq;

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

            if (attribute != null)
            {
                return new SuccessfulInvocationResult(attribute.Value);
            }
            if (childElement != null)
            {
                return new SuccessfulInvocationResult(childElement.Value);
            }

            var childElements = _element.Elements(name.TrimEnd('s'));

            if (childElements.Any())
            {
                var dynamicXmlElements = childElements.Select(x => new DynamicXmlElement(x)).ToList();
                return new SuccessfulInvocationResult(dynamicXmlElements);
            }

            throw new MissingValueException(name);
        }
    }
}