using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace Gosu.Commons.Parsing
{
    public class NamespaceRegistry 
    {
        private IList<Namespace> _namespaces = new List<Namespace>();
        private XNamespace _defaultNamespace;

        public void Register(string uri, string alias)
        {
            _namespaces.Add(new Namespace() { Uri = uri, Alias = alias });
        }

        public bool HasNamespaceFor(string name)
        {
            return GetNamespace(name) != null;
        }

        public XName GetPath(string name)
        {
            return GetNamespace(name).ToXNamespace() + GetNameWithoutNamespace(name);
        }

        public XName GetDefaultNamespacePath(string name)
        {
            return _defaultNamespace + name;
        }

        public void SetDefaultNamespace(XNamespace defaultNamespace)
        {
            _defaultNamespace = defaultNamespace;
        }

        public string GetNameWithoutNamespace(string name)
        {
            var ns = GetNamespace(name);
            return name.Substring(ns.Alias.Length);
        }

        public Namespace GetNamespace(string name)
        {
            return _namespaces.FirstOrDefault(x => name.StartsWith(x.Alias));
        }

        public XNamespace GetDefaultNamespace()
        {
            return _defaultNamespace;
        }
    }
}