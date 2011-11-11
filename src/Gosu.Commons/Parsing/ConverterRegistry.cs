using System;
using System.Collections.Generic;

namespace Gosu.Commons.Parsing
{
    public class ConverterRegistry
    {
        Dictionary<Type, Func<string, object>> _converters =
            new Dictionary<Type, Func<string, object>>();

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
            return _converters[type](value);
        }

        public bool HasConverterFor(Type type)
        {
            return _converters.ContainsKey(type);
        }
    }
}