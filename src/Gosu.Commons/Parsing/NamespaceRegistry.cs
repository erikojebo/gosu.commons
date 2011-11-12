using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace Gosu.Commons.Parsing
{
    public class NamespaceRegistry 
    {
        private IList<Namespace> _namespaces = new List<Namespace>();

        public void Register(string uri, string alias)
        {
            _namespaces.Add(new Namespace() { Uri = uri, Alias = alias });
        }

        private class Namespace
        {
            public string Uri { get; set; }
            public string Alias { get; set; }

            public XNamespace ToXNamespace()
            {
                return Uri;
            }
        }

        public bool HasNamespaceFor(string name)
        {
            return name.StartsWith("IR");
        }

        public XName GetPath(string name)
        {
            return _namespaces.First(x => x.Alias == "IR").ToXNamespace() + name.Substring("IR".Length);
        }
    }
}