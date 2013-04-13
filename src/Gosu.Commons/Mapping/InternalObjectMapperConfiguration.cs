using System.Linq;
using System.Reflection;

namespace Gosu.Commons.Mapping
{
    internal class InternalObjectMapperConfiguration<TSource, TTarget> : ObjectMapperConfiguration<TSource, TTarget>, IObjectMapperConfiguration
    {
        public string GetTargetPropertyName(PropertyInfo sourceProperty)
        {
            return SourceToTargetPropertyNameConvention(sourceProperty);
        }

        public bool IsIgnored(PropertyInfo propertyInfo)
        {
            return IgnoredProperties.Any(x => x == propertyInfo.Name);
        }

        public object GetValue(object source, PropertyInfo sourceProperty, PropertyInfo targetProperty)
        {
            if (CustomValueSelectors.ContainsKey(targetProperty.Name))
                return CustomValueSelectors[targetProperty.Name]((TSource)source);

            return sourceProperty.GetValue(source, null);
        }

        public bool IsCustom(PropertyInfo targetProperty)
        {
            return CustomValueSelectors.ContainsKey(targetProperty.Name);
        }
    }
}