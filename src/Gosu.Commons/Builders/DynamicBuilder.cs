using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gosu.Commons.Builders.Exceptions;
using Gosu.Commons.Dynamics;

namespace Gosu.Commons.Builders
{
    public class DynamicBuilder<T> : HookableDynamicObject
        where T : class, new()
    {
        protected readonly T Entity = new T();
        private PropertyInfo _collectionProperty;

        protected override InvocationResult MethodMissing(string methodName, object[] arguments)
        {
            var value = arguments[0];

            if (MatchesCollectionProperty(methodName))
            {
                return AddChild(methodName, value);
            }

            return SetOrdinaryProperty(methodName, value);
        }

        private bool MatchesCollectionProperty(string methodName)
        {
            var propertyName = methodName + "s";
            _collectionProperty = typeof(T).GetProperty(propertyName);

            return _collectionProperty != null;
        }

        private InvocationResult AddChild(string methodName, object value)
        {
            var propertyType = _collectionProperty.PropertyType;

            var genericInterfaces = propertyType.GetInterfaces()
                .Where(x => x.IsGenericType);

            var collectionInterfaces = genericInterfaces
                .Where(x => x.GetGenericTypeDefinition() == typeof(ICollection<>));

            if (collectionInterfaces.Any())
            {
                var childTypes = collectionInterfaces.Select(x => x.GetGenericArguments()[0]);

                var childType = childTypes.First();
                object childInstance = GetChildInstance(childType, value);

                var addMethod = collectionInterfaces.First().GetMethod("Add");
                var collection = _collectionProperty.GetValue(Entity, null);
                addMethod.Invoke(collection, new[] { childInstance });
            }
            else if (propertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var childType = propertyType.GetGenericArguments()[0];
                object childInstance = GetChildInstance(childType, value);

                // Collection: "Objects"
                // Builder method called: .Object(...)
                // Add method: AddObject(...)
                var addMethod = typeof(T).GetMethod("Add" + methodName);

                addMethod.Invoke(Entity, new[] { childInstance });
            }

            return new SuccessfulInvocationResult(this);
        }

        private object GetChildInstance(Type childType, object value)
        {
            // If the argument to the method was an instance of the child type, use the
            // parameter value as the child to add
            if (childType.IsAssignableFrom(value.GetType()))
            {
                return value;
            }

            // Otherwise, create a new instance of the child type
            var childInstance = Activator.CreateInstance(childType);

            if (value is PropertyValueCollector)
                ((PropertyValueCollector)value).Initialize(childInstance);

            return childInstance;
        }

        private InvocationResult SetOrdinaryProperty(string propertyName, object value)
        {
            var property = typeof(T).GetProperty(propertyName);

            var propertyExists = property != null;

            if (!propertyExists)
            {
                throw new MissingPropertyException(propertyName, value);
            }

            if (property.PropertyType == typeof(DateTime) && value is string)
            {
                value = DateTime.Parse((string)value);
            }

            property.SetValue(Entity, value, null);
            return new SuccessfulInvocationResult(this);
        }

        public T Build()
        {
            return Entity;
        }
    }
}