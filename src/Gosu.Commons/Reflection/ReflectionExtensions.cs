using System;
using System.Collections.Generic;
using System.Globalization;
using Gosu.Commons.Extensions;

namespace Gosu.Commons.Reflection
{
    public static class ReflectionExtensions
    {
        public static void SetProperty(this object instance, string propertyName, object value)
        {
            var property = instance.GetType().GetProperty(propertyName);
            property.SetValue(instance, value, null);
        }

        public static void CoercePropertyDefaultingMissingValues(this object instance, string propertyName, string value)
        {
            instance.ResetToDefaultValue(propertyName);

            if (!value.IsNullOrEmpty())
            {
                instance.CoerceProperty(propertyName, value);
            }
        }

        public static void CoerceProperty(this object instance, string propertyName, string value)
        {
            var converters = new Dictionary<Type, Func<string, object>>
                {
                    { typeof(string), s => s },
                    { typeof(int), s => int.Parse(s, CultureInfo.InvariantCulture) },
                    { typeof(double), s => double.Parse(s, CultureInfo.InvariantCulture) }
                };

            var property = instance.GetType().GetProperty(propertyName);
            var propertyType = property.PropertyType;

            var converter = converters[propertyType];
            var coercedValue = converter.Invoke(value);

            instance.SetProperty(propertyName, coercedValue);
        }

        public static bool HasProperty(this object instance, string propertyName)
        {
            return instance.GetType().GetProperty(propertyName) != null;
        }

        public static Type GetPropertyType(this object instance, string propertyName)
        {
            return instance.GetType().GetProperty(propertyName).PropertyType;
        }

        public static void ResetToDefaultValue(this object instance, string propertyName)
        {
            var propertyType = instance.GetPropertyType(propertyName);
            var defaultValueForProperty = propertyType.GetDefaultValue();
        
            instance.SetProperty(propertyName, defaultValueForProperty);
        }

        public static bool TryCallMethod(this object instance, string methodName)
        {
            var method = instance.GetType().GetMethod(methodName);

            if (method == null)
            {
                return false;
            }

            method.Invoke(instance, null);

            return true;
        }

        public static object GetDefaultValue(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}