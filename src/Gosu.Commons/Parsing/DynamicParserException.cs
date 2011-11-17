using System;

namespace Gosu.Commons.Parsing
{
    public class DynamicParserException : Exception
    {
        public DynamicParserException(string message)
            : base(message) {}
    }
}