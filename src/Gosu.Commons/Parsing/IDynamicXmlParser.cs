using System;

namespace Gosu.Commons.Parsing
{
    public interface IDynamicXmlParser 
    {
        dynamic Parse(string xml);
        void SetConverter<T>(Func<string , T> converter);
        void SetNamespaceAlias(string uri, string alias);
    }
}