using System;
using System.Collections.Generic;

namespace Gosu.Commons.Parsing
{
    public class ValueConverter
    {
        private readonly Dictionary<Type, Func<string, object>> _converters =
            new Dictionary<Type, Func<string, object>>();

        public ValueConverter()
        {
            Register<int>(s => int.Parse(s));
            Register<double>(s => double.Parse(s));
            Register<decimal>(s => decimal.Parse(s));
            Register<float>(s => float.Parse(s));
            Register<DateTime>(s => DateTime.Parse(s));
            Register<TimeSpan>(s => TimeSpan.Parse(s));
            Register<bool>(s => bool.Parse(s));
            Register<String>(s => s);
        }

        public void Register(Type type, Func<string, object> converter)
        {
            _converters[type] = converter;
        }

        public void Register<T>(Func<string, object> converter)
        {
            _converters[typeof(T)] = converter;
        }

        public object Convert(Type type, string value)
        {
            if (HasConverterFor(type))
            {
                return _converters[type](value);
            }
            if (type.IsEnum)
            {
                return Enum.Parse(type, value);
            }

            throw new InvalidConversionException("No converter was found for type: " + type);
        }

        public bool CanConvert(Type type)
        {
            if (type.IsEnum)
            {
                return true;
            }

            return HasConverterFor(type);
        }

        private bool HasConverterFor(Type type)
        {
            return _converters.ContainsKey(type);
        }
    }
}