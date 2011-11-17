namespace Gosu.Commons.Parsing
{
    public class MissingValueException : DynamicParserException
    {
        public MissingValueException(string name)
            : base("Could not find attribute or child element: " + name) {}
    }
}