using System;
using Gosu.Commons.Dynamics;

namespace Gosu.Commons.Parsing
{
    public class ConvertibleStringValue : HookableDynamicObject
    {
        private readonly string _value;
        private readonly ValueConverter _valueConverter;

        public ConvertibleStringValue(string value, ValueConverter valueConverter)
        {
            _value = value;
            _valueConverter = valueConverter;
        }

        protected override InvocationResult ConversionMissing(Type type, ConversionMode conversionMode)
        {
            if (CanConvertTo(type))
            {
                return new SuccessfulInvocationResult(Convert(type));
            }

            return new FailedInvocationResult();
        }

        public object Convert(Type type)
        {
            return _valueConverter.Convert(type, _value);
        }

        public bool CanConvertTo(Type type)
        {
            return _valueConverter.CanConvert(type);
        }
    }
}