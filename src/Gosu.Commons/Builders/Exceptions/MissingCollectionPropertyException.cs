using System;

namespace Gosu.Commons.Builders.Exceptions
{
    public class MissingCollectionPropertyException : Exception
    {
        public MissingCollectionPropertyException(object propertyName)
            : base("Unable to find collection property: " + propertyName) {}
    }
}