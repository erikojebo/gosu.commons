using System;
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
            return Map<TTarget>(source, x => { });
        }

        /// <summary>
        /// Creates a new instance of the target type and maps all matching properties
        /// from the source object to the new object
        /// </summary>
        /// <typeparam name="TTarget">The type of the object to map to</typeparam>
        /// <param name="source">The object to map from</param>
        /// <returns>A new instance of with properties mapped from the source object</returns>
        /// <param name="configuration">The configuration of how to perform the mapping.
        /// Could, for example, specify properties to ignore or map differently than the default.</param>
        public TTarget Map<TTarget>(object source, Action<ObjectMapperConfiguration<TTarget>> configuration)
            where TTarget : new()
        {
            var target = new TTarget();

            Map(source, target, configuration);

            return target;
        }

        /// <summary>
        /// Finds all properties which exist in both target and source, and maps
        /// the values of those properties from the source object to the target object
        /// </summary>
        /// <param name="source">The object to map from</param>
        /// <param name="target">The object to map to</param>
        public void Map(object source, object target)
        {
            Map(source, target, x => {});
        }

        /// <summary>
        /// Finds all properties which exist in both target and source, and maps
        /// the values of those properties from the source object to the target object
        /// </summary>
        /// <param name="source">The object to map from</param>
        /// <param name="target">The object to map to</param>
        /// <param name="configuration">The configuration of how to perform the mapping.
        /// Could, for example, specify properties to ignore or map differently than the default.</param>
        public void Map<TTarget>(object source, TTarget target, Action<ObjectMapperConfiguration<TTarget>> configuration)
        {
            var config = InitializeConfiguration(configuration);

            var targetProperties = GetProperties(target);
            var sourceProperties = GetProperties(source);

            foreach (var targetProperty in targetProperties)
            {
                if (config.IsIgnored(targetProperty))
                    continue;

                var sourceProperty = sourceProperties.FirstOrDefault(x => config.GetTargetPropertyName(x) == targetProperty.Name);

                if (sourceProperty == null)
                    continue;

                var value = sourceProperty.GetValue(source, null);
                targetProperty.SetValue(target, value, null);
            }
        }

        private static InternalObjectMapperConfiguration<TTarget> InitializeConfiguration<TTarget>(Action<ObjectMapperConfiguration<TTarget>> configuration)
        {
            var config = new InternalObjectMapperConfiguration<TTarget>();

            configuration(config);
            
            return config;
        }

        private static PropertyInfo[] GetProperties(object obj)
        {
            return obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }
    }
}