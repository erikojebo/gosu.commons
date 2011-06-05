using Gosu.Commons.Dynamics;

namespace Gosu.Commons.Builders
{
    public class DynamicBuilder<T> : HookableDynamicObject
        where T : class, new()
    {
        protected readonly T Entity = new T();

        public override InvocationResult MethodMissing(string methodName, object[] arguments)
        {
            var property = typeof(T).GetProperty(methodName);

            var propertyExists = property != null;

            if (propertyExists)
            {
                property.SetValue(Entity, arguments[0], null);
                return new SuccessfulInvocationResult(this);
            }

            return new FailedInvocationResult();
        }

        public T Build()
        {
            return Entity;
        }
    }
}