using System.Reflection;

namespace Gosu.Commons.Mapping
{
    internal class NullObjectMapperConfiguration : IObjectMapperConfiguration
    {
        public string GetTargetPropertyName(PropertyInfo sourceProperty)
        {
            return sourceProperty.Name;
        }

        public bool IsIgnored(PropertyInfo propertyInfo)
        {
            return false;
        }

        public object GetValue(object source, PropertyInfo sourceProperty, PropertyInfo targetProperty)
        {
            return sourceProperty.GetValue(source, null);
        }

        public bool IsCustom(PropertyInfo targetProperty)
        {
            return false;
        }
    }
}