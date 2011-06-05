using System;
using System.Collections.Generic;
using System.Globalization;

namespace Gosu.Commons.Reflection
{
    public static class ReflectionExtensions
    {
        public static void SetProperty<T>(this T instance, string propertyName, object value)
        {
            var property = typeof(T).GetProperty(propertyName);
            property.SetValue(instance, value, null);
        }

        public static void CoerceProperty<T>(this T instance, string propertyName, string value)
        {
            var converters = new Dictionary<Type, Func<string, object>>
                {
                    { typeof(string), s => s },
                    { typeof(int), s => int.Parse(s, CultureInfo.InvariantCulture) },
                    { typeof(double), s => double.Parse(s, CultureInfo.InvariantCulture) }
                };

            var property = typeof(T).GetProperty(propertyName);
            var propertyType = property.PropertyType;

            var converter = converters[propertyType];
            var coercedValue = converter.Invoke(value);

            instance.SetProperty(propertyName, coercedValue);
        }

        public static bool HasProperty<T>(this T instance, string propertyName)
        {
            return typeof(T).GetProperty(propertyName) != null;
        }

        public static Type GetPropertyType<T>(this T instance, string propertyName)
        {
            return typeof(T).GetProperty(propertyName).PropertyType;
        }
    }
}