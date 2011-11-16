using System.Collections.Generic;
using Gosu.Commons.Dynamics;

namespace Gosu.Commons.Builders
{
    public class PropertyValueCollector : HookableDynamicObject
    {
        public readonly Dictionary<string, object> PropertyValues = new Dictionary<string, object>();

        protected override InvocationResult MethodMissing(string methodName, object[] arguments)
        {
            PropertyValues[methodName] = arguments[0];
            return new SuccessfulInvocationResult(this);
        }

        public void Initialize(object childInstance)
        {
            var type = childInstance.GetType();

            foreach (var propertyValue in PropertyValues)
            {
                var property = type.GetProperty(propertyValue.Key);
                property.SetValue(childInstance, propertyValue.Value, null);
            }
        }
    }
}