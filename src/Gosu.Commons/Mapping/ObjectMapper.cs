using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Gosu.Commons.Mapping
{
    public class ObjectMapper
    {
        /// <summary>
        /// Creates a new instance of the target type and maps all matching properties
        /// from the source object to the new object
        /// </summary>
        /// <typeparam name="TTarget">The type of the object to map to</typeparam>
        /// <param name="source">The object to map from</param>
        /// <returns>A new instance of with properties mapped from the source object</returns>
        public TTarget Map<TTarget>(object source)
            where TTarget : new()
        {
            return Map<TTarget>(source, new NullObjectMapperConfiguration());
        }

        /// <summary>
        /// Creates a new instance of the target type and maps all matching properties
        /// from the source object to the new object
        /// </summary>
        /// <typeparam name="TTarget">The type of the object to map to</typeparam>
        /// <typeparam name="TSource">The type of the object to map from</typeparam>
        /// <param name="source">The object to map from</param>
        /// <returns>A new instance of with properties mapped from the source object</returns>
        /// <param name="configuration">The configuration of how to perform the mapping.
        /// Could, for example, specify properties to ignore or map differently than the default.</param>
        public TTarget Map<TSource, TTarget>(TSource source, Action<ObjectMapperConfiguration<TSource, TTarget>> configuration)
            where TTarget : new()
        {
            return Map<TTarget>(source, InitializeConfiguration(configuration));
        }
        
        public TTarget Map<TTarget>(object source, IObjectMapperConfiguration configuration)
            where TTarget : new()
        {
            var target = new TTarget();

            Map(source, target, configuration);

            return target;
        }
        
        public object Map(Type targetType, object source)
        {
            return Map(targetType, source, new NullObjectMapperConfiguration());
        }
        
        public object Map(Type targetType, object source, IObjectMapperConfiguration configuration)
        {
            object target;

            if (targetType.IsArray)
            {
                var length = GetCollectionLength(source);
                target = Array.CreateInstance(targetType.GetElementType(), length);
            }
            else
            {
                target = Activator.CreateInstance(targetType);    
            }

            Map(source, target, configuration);

            return target;
        }

        private int GetCollectionLength(object source)
        {
            var sourceType = source.GetType();

            if (sourceType.IsArray)
            {
                return ((object[])source).Length;
            }
            if (source is ICollection)
            {
                return ((ICollection)source).Count;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds all properties which exist in both target and source, and maps
        /// the values of those properties from the source object to the target object
        /// </summary>
        /// <param name="source">The object to map from</param>
        /// <param name="target">The object to map to</param>
        public void Map(object source, object target)
        {
            Map(source, target, new NullObjectMapperConfiguration());
        }

        /// <summary>
        /// Finds all properties which exist in both target and source, and maps
        /// the values of those properties from the source object to the target object
        /// </summary>
        /// <typeparam name="TTarget">The type of the object to map to</typeparam>
        /// <typeparam name="TSource">The type of the object to map from</typeparam>
        /// <param name="source">The object to map from</param>
        /// <param name="target">The object to map to</param>
        /// <param name="configuration">The configuration of how to perform the mapping.
        /// Could, for example, specify properties to ignore or map differently than the default.</param>
        public void Map<TSource, TTarget>(TSource source, TTarget target, Action<ObjectMapperConfiguration<TSource, TTarget>> configuration)
        {
            var config = InitializeConfiguration(configuration);

            Map(source, target, config);
        }

        public void Map(object source, object target, IObjectMapperConfiguration config)
        {
            //if (target.GetType().GetGenericTypeDefinition() == typeof(List<>))
            if (target.GetType().IsArray)
            {
                MapArray(source, target);
                return;
            }
            if (target is IEnumerable)
            {
                MapCollection(source, target);
                return;
            }

            var targetProperties = GetProperties(target);
            var sourceProperties = GetProperties(source);

            foreach (var targetProperty in targetProperties)
            {
                if (config.IsIgnored(targetProperty))
                    continue;

                var sourceProperty = sourceProperties.FirstOrDefault(x => config.GetTargetPropertyName(x) == targetProperty.Name);

                if (sourceProperty == null && !config.IsCustom(targetProperty))
                    continue;

                var value = config.GetValue(source, sourceProperty, targetProperty);

                if (!CanBeAssignedWithoutMapping(targetProperty, value))
                    value = Map(targetProperty.PropertyType, value);

                targetProperty.SetValue(target, value, null);
            }
        }

        private void MapArray(object source, object target)
        {
            var targetElementType = target.GetType().GetElementType();

            var index = 0;
            
            foreach (var sourceElement in (IEnumerable)source)
            {
                ((object[])target)[index] = Map(targetElementType, sourceElement);

                index++;
            }
        }

        private static bool CanBeAssignedWithoutMapping(PropertyInfo targetProperty, object value)
        {
            // If the value is of the same type or of a sub type of the property type in the 
            // target, then the value can be assigned to the target property without further ado.
            // If the value is a value type just try to assign it. For example, trying to assign
            // an int to a double property would not pass the first test, but it would still be a
            // valid assignment. Since we can't really do much mapping between value type values
            // just try to go ahead with the assignment.
            return targetProperty.PropertyType.IsInstanceOfType(value) || value is ValueType;
        }

        private void MapCollection(object source, object target)
        {
            var targetElementType = target.GetType().GetGenericArguments().First();

            foreach (var sourceElement in (IEnumerable)source)
            {
                ((IList)target).Add(Map(targetElementType, sourceElement));
            }
        }

        private InternalObjectMapperConfiguration<TSource, TTarget> InitializeConfiguration<TSource, TTarget>(
            Action<ObjectMapperConfiguration<TSource, TTarget>> configuration)
        {
            var config = new InternalObjectMapperConfiguration<TSource, TTarget>();

            configuration(config);
            
            return config;
        }

        private PropertyInfo[] GetProperties(object obj)
        {
            return obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }
    }
}