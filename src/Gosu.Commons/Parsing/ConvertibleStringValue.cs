using System;
using System.Collections.Generic;
using Gosu.Commons.Dynamics;

namespace Gosu.Commons.Parsing
{
    public class ConvertibleStringValue : HookableDynamicObject
    {
        private readonly string _value;
        private Dictionary<Type, Func<string, object>> _converters;

        public ConvertibleStringValue(string value)
        {
            _value = value;
            _converters = new Dictionary<Type, Func<string, object>>
                {
                    { typeof(int), s => int.Parse(s) },
                    { typeof(double), s => double.Parse(s) },
                    { typeof(decimal), s => decimal.Parse(s) },
                    { typeof(float), s => float.Parse(s) },
                    { typeof(DateTime), s => DateTime.Parse(s) },
                    { typeof(TimeSpan), s => TimeSpan.Parse(s) },
                    { typeof(bool), s => bool.Parse(s) },
                    { typeof(String), s => s }
                };
        }

        public void SetConverter(Type type, Func<string, object> converter)
        {
            _converters[type] = converter;
        }

        protected override InvocationResult ConvertionMissing(Type type, ConvertionMode conversionMode)
        {
            if (_converters.ContainsKey(type))
            {
                return new SuccessfulInvocationResult(_converters[type](_value));
            }

            return new FailedInvocationResult();
        }

    }
}