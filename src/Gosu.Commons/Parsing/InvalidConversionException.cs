namespace Gosu.Commons.Parsing
{
    public class InvalidConversionException : DynamicParserException
    {
        public InvalidConversionException(string message) : base(message) {}
    }
}