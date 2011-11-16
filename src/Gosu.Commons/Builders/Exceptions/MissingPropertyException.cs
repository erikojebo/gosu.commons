using System;

namespace Gosu.Commons.Builders.Exceptions
{
    public class MissingPropertyException : Exception
    {
        public MissingPropertyException(string propertyName, object value) : base(CreateMessage(propertyName, value)) {}

        private static string CreateMessage(string propertyName, object value)
        {
            return string.Format(
                "Tried to set property '{0}' to value '{1}', but no such property was found",
                propertyName,
                value);
        }
    }
}