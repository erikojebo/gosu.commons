using System;
using System.Runtime.CompilerServices;
using Gosu.Commons.Reflection;
using Microsoft.CSharp.RuntimeBinder;

namespace Gosu.Commons.Mapping
{
    public class DynamicToObjectMapper
    {
        public T Map<T>(object source, MappingMode mappingMode) where T : new()
        {
            var instance = new T();

            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                try
                {
                    var value = GetDynamicMember(source, property.Name);
                    instance.SetProperty(property.Name, value);
                }
                catch (RuntimeBinderException)
                {
                    if (mappingMode == MappingMode.AllProperties)
                        throw new ValueNotFoundException(property.Name);
                }
            }

            return instance;
        }

        private static object GetDynamicMember(object obj, string memberName)
        {
            var binder = Binder.GetMember(CSharpBinderFlags.None, memberName, obj.GetType(),
                                          new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);

            return callsite.Target(callsite, obj);
        }
    }


    public enum MappingMode
    {
        PropertiesWithValues,
        AllProperties
    }

    public class ValueNotFoundException : Exception
    {
        public ValueNotFoundException(string propertyName) : base(string.Format("Could not find value for the property: " + propertyName))
        {
        }
    }

    public static class DynamicToObjectMapperExtensions
    {
        public static T FromDynamic<T>(this T instance, object source, MappingMode mappingMode = MappingMode.PropertiesWithValues) where T : new()
        {
            var mapper = new DynamicToObjectMapper();

            return mapper.Map<T>(source, mappingMode);
        }
    }
}