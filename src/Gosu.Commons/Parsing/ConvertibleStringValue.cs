using System;
using Gosu.Commons.Dynamics;

namespace Gosu.Commons.Parsing
{
    public class ConvertibleStringValue : HookableDynamicObject
    {
        private readonly string _value;
        private readonly ConverterRegistry _converterRegistry;

        public ConvertibleStringValue(string value, ConverterRegistry converterRegistry)
        {
            _value = value;
            _converterRegistry = converterRegistry;
        }

        protected override InvocationResult ConvertionMissing(Type type, ConvertionMode conversionMode)
        {
            if (CanConvertTo(type))
            {
                return new SuccessfulInvocationResult(Convert(type));
            }

            return new FailedInvocationResult();
        }

        public object Convert(Type type)
        {
            return _converterRegistry.Convert(type, _value);
        }

        public bool CanConvertTo(Type type)
        {
            return _converterRegistry.HasConverterFor(type);
        }
    }
}