using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Gosu.Commons.DataStructures;

namespace Gosu.Commons.Parsing
{
    public class NamespaceRegistry
    {
        private readonly IList<XmlNamespace> _namespaces = new List<XmlNamespace>();
        private XmlNamespace _defaultNamespace;

        public void Register(string uri, string alias)
        {
            _namespaces.Add(new XmlNamespace { Uri = uri, Alias = alias });
        }

        public void SetDefaultNamespace(XNamespace defaultNamespace)
        {
            _defaultNamespace = new XmlNamespace { Uri = defaultNamespace };
        }

        public Maybe<XmlNamespace> GetNamespace(string name)
        {
            var xmlNamespace = _namespaces.FirstOrDefault(x => name.StartsWith(x.Alias));

            if (xmlNamespace == null)
            {
                return Maybe<XmlNamespace>.Nothing;
            }

            return xmlNamespace.ToMaybe();
        }

        public Maybe<XmlNamespace> GetDefaultNamespace()
        {
            if (_defaultNamespace == null)
            {
                return Maybe<XmlNamespace>.Nothing;
            }

            return _defaultNamespace.ToMaybe();
        }
    }
}