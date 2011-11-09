using System;

namespace Gosu.Commons.Parsing
{
    public class MissingValueException : Exception
    {
        public MissingValueException(string name)
            : base("Could not find attribute or child element: " + name) {}
    }
}