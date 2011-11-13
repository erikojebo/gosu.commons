using System.Xml.Linq;

namespace Gosu.Commons.Parsing
{
    public class Namespace
    {
        public string Uri { get; set; }
        public string Alias { get; set; }

        public XNamespace ToXNamespace()
        {
            return Uri;
        }
    }
}