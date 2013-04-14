using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Gosu.Commons.Reflection;

namespace Gosu.Commons.Mapping
{
    public class ObjectMapper
    {
        private readonly IList<Preconfiguration> _configurations = new List<Preconfiguration>();

        /// <summary>
        /// Creates a new instance of the target type and maps all matching properties
        /// from the source object to the new object
        /// </summary>
        /// <typeparam name="TTarget">The type of the object to map to</typeparam>
        /// <param name="source">The object to map from</param>
        /// <returns>A new instance of with properties mapped from the source object</returns>
        public TTarget Map<TTarget>(object source)
            where TTarget : class, new()
        {
            return Map<TTarget>(source, GetConfiguration(source, typeof(TTarget)));
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
            where TTarget : class, new()
        {
            return Map<TTarget>(source, InitializeConfiguration(configuration));
        }

        /// <summary>
        /// Creates a new instance of the target type and maps all matching properties
        /// from the source object to the new object
        /// </summary>
        /// <typeparam name="TTarget">The type of the object to map to</typeparam>
        /// <param name="source">The object to map from</param>
        /// <param name="configuration">The configuration of how to perform the mapping.
        /// Could, for example, specify properties to ignore or map differently than the default.</param>
        /// <returns>A new instance of with properties mapped from the source object</returns>
        public TTarget Map<TTarget>(object source, IObjectMapperConfiguration configuration)
            where TTarget : class, new()
        {
            if (source == null)
                return null;

            var target = new TTarget();

            Map(source, target, configuration);

            return target;
        }

        /// <summary>
        /// Creates a new instance of the target type and maps all matching properties
        /// from the source object to the new object
        /// </summary>
        /// <param name="targetType">The type of the object to map to</param>
        /// <param name="source">The object to map from</param>
        /// <returns>A new instance of with properties mapped from the source object</returns>
        public object Map(Type targetType, object source)
        {
            return Map(targetType, source, GetConfiguration(source, targetType));
        }

        /// <summary>
        /// Creates a new instance of the target type and maps all matching properties
        /// from the source object to the new object
        /// </summary>
        /// <param name="targetType">The type of the object to map to</param>
        /// <param name="source">The object to map from</param>
        /// <param name="configuration">The configuration of how to perform the mapping.
        /// Could, for example, specify properties to ignore or map differently than the default.</param>
        /// <returns>A new instance of with properties mapped from the source object</returns>
        public object Map(Type targetType, object source, IObjectMapperConfiguration configuration)
        {
            if (source == null)
                return null;

            var target = InstantiateTarget(targetType, source);

            Map(source, target, configuration);

            return target;
        }

        private object InstantiateTarget(Type targetType, object source)
        {
            object target;

            if (targetType.IsArray)
                target = InstantiateArray(targetType, source);
            else if (targetType.IsInterface && IsIEnumerable(targetType))
                target = InstantiateList(targetType);
            else
                target = Activator.CreateInstance(targetType);

            return target;
        }

        private object InstantiateArray(Type targetType, object source)
        {
            var length = GetCollectionLength(source);
            return Array.CreateInstance(targetType.GetElementType(), length);
        }

        private static object InstantiateList(Type targetType)
        {
            var listType = typeof(List<>).MakeGenericType(targetType.GetGenericArguments().First());
            return Activator.CreateInstance(listType);
        }

        private static bool IsIEnumerable(Type targetType)
        {
            return targetType.HasGenericTypeDefinition(typeof(IEnumerable<>));
        }

        private int GetCollectionLength(object source)
        {
            var sourceType = source.GetType();

            if (sourceType.IsArray)
                return ((object[])source).Length;

            if (source is ICollection)
                return ((ICollection)source).Count;

            throw new Exception(string.Format("Mapping of the collection type {0} is not supported", sourceType.Name));
        }

        /// <summary>
        /// Finds all properties which exist in both target and source, and maps
        /// the values of those properties from the source object to the target object
        /// </summary>
        /// <param name="source">The object to map from</param>
        /// <param name="target">The object to map to</param>
        public void Map(object source, object target)
        {
            Map(source, target, GetConfiguration(source, target.GetType()));
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

        /// <summary>
        /// Finds all properties which exist in both target and source, and maps
        /// the values of those properties from the source object to the target object
        /// </summary>
        /// <param name="source">The object to map from</param>
        /// <param name="target">The object to map to</param>
        /// <param name="config">The configuration of how to perform the mapping.
        /// Could, for example, specify properties to ignore or map differently than the default.</param>
        public void Map(object source, object target, IObjectMapperConfiguration config)
        {
            // If the source object is a null collection, just let the ordinary mapping handle pushing
            // the null value into the matching property on the target
            if (target.GetType().IsArray && source != null)
            {
                MapArray(source, target);
                return;
            }
            if (target is IEnumerable && source != null)
            {
                MapCollection(source, target);
                return;
            }

            MapObject(source, target, config);
        }

        private void MapObject(object source, object target, IObjectMapperConfiguration config)
        {
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

        /// <summary>
        /// Creates a configuration that should be used when mapping between the given source and target types
        /// from now on, for this ObjectMapper instance.
        /// </summary>
        /// <param name="configuration">The configuration to use when mapping between TSource and TTarget</param>
        /// <typeparam name="TSource">The type to map from</typeparam>
        /// <typeparam name="TTarget">The type to map to</typeparam>
        /// <returns></returns>
        public ObjectMapper ConfigureMap<TSource, TTarget>(Action<ObjectMapperConfiguration<TSource, TTarget>> configuration)
        {
            var config = InitializeConfiguration(configuration);

            var preconfiguration = new Preconfiguration
                {
                    SourceType = typeof(TSource),
                    TargetType = typeof(TTarget),
                    Configuration = config
                };

            _configurations.Add(preconfiguration);

            return this;
        }

        private IObjectMapperConfiguration GetConfiguration(object source, Type targetType)
        {
            if (source == null)
                return new NullObjectMapperConfiguration();

            return GetConfiguration(source.GetType(), targetType);
        }

        private IObjectMapperConfiguration GetConfiguration(Type sourceType, Type targetType)
        {
            var preconfig = _configurations.FirstOrDefault(x => x.SourceType == sourceType && x.TargetType == targetType);

            return preconfig != null ? preconfig.Configuration : new NullObjectMapperConfiguration();
        }

        private class Preconfiguration
        {
            public Type SourceType { get; set; }
            public Type TargetType { get; set; }
            public IObjectMapperConfiguration Configuration { get; set; }
        }
    }
}