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
            var target = new TTarget();

            Map(source, target);

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

        public void Map(object source, object target, Action<ObjectMapperConfiguration> configuration)
        {
            var config = new InternalObjectMapperConfiguration();
            configuration(config);

            var targetProperties = GetProperties(target);
            var sourceProperties = GetProperties(source);

            foreach (var targetProperty in targetProperties)
            {
                var sourceProperty = sourceProperties.FirstOrDefault(x => config.GetTargetPropertyName(x) == targetProperty.Name);

                if (sourceProperty == null)
                    continue;

                var value = sourceProperty.GetValue(source, null);
                targetProperty.SetValue(target, value, null);
            }
        }

        private static PropertyInfo[] GetProperties(object obj)
        {
            return obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }
    }
}