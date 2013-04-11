using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Gosu.Commons.Reflection;
using System.Linq;

namespace Gosu.Commons.Mapping
{
    public abstract class ObjectMapperConfiguration<TTarget>
    {
        protected Func<PropertyInfo, string> SourceToTargetPropertyNameConvention;
        protected List<string> IgnoredProperties = new List<string>();

        protected ObjectMapperConfiguration()
        {
            SourceToTargetPropertyNameConvention = propertyInfo => propertyInfo.Name;
        }

        /// <summary>
        /// Specifies how to get the name of a property in the target object
        /// from a given property name in the source object.
        /// </summary>
        /// <example>If the source object has properties which use a three letter Hungarian notation prefix, 
        /// such as 'StrValue', 'StrSomeOtherValue', 'IntSomeIntValue' and you try to escape from that
        /// horrible object to another object with properties called 'Value', 'SomeOtherValue' and 'SomeIntValue',
        /// you could specify a convention which strips the first three letters of each source property name
        /// to get the target property name.</example>
        /// <param name="sourceToTargetPropertyNameConvention">
        /// The function which returns a property name for the target object from a given property in the source object
        /// </param>
        /// <returns>The configuration object</returns>
        public ObjectMapperConfiguration<TTarget> Convention(Func<PropertyInfo, string> sourceToTargetPropertyNameConvention)
        {
            SourceToTargetPropertyNameConvention = sourceToTargetPropertyNameConvention;
            return this;
        }

        /// <summary>
        /// Specified a property in the target object which should not be mapped
        /// </summary>
        /// <param name="propertySelector">An expression which points out a property that should be ignored</param>
        /// <returns>The configuration object</returns>
        public ObjectMapperConfiguration<TTarget> Ignore(Expression<Func<TTarget, object>> propertySelector)
        {
            var propertyName = ExpressionParser.GetPropertyName(propertySelector);
            IgnoredProperties.Add(propertyName);

            return this;
        }
    }

    internal class InternalObjectMapperConfiguration<T> : ObjectMapperConfiguration<T>
    {
        public string GetTargetPropertyName(PropertyInfo sourcePropertyName)
        {
            return SourceToTargetPropertyNameConvention(sourcePropertyName);
        }

        public bool IsIgnored(PropertyInfo propertyInfo)
        {
            return IgnoredProperties.Any(x => x == propertyInfo.Name);
        }
    }
}