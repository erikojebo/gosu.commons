using System.Reflection;

namespace Gosu.Commons.Mapping
{
    public interface IObjectMapperConfiguration
    {
        string GetTargetPropertyName(PropertyInfo sourceProperty);
        bool IsIgnored(PropertyInfo propertyInfo);
        object GetValue(object source, PropertyInfo sourceProperty, PropertyInfo targetProperty);
        bool IsCustom(PropertyInfo targetProperty);
    }
}