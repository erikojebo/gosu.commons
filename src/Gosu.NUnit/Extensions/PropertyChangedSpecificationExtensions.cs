using System;
using System.Collections.Generic;
using System.ComponentModel;
using Gosu.Commons.Reflection;

namespace Gosu.NUnit.Extensions
{
    public static class PropertyChangedSpecificationExtensions
    {
        public static void ShouldFirePropertyChangedFor<TInstance>(this TInstance instance, string propertyName)
            where TInstance : INotifyPropertyChanged
        {
            var values = new Dictionary<Type, Tuple<object, object>>
                {
                    { typeof(int), new Tuple<object, object>(1, 2) },
                    { typeof(int?), new Tuple<object, object>(1, 2) },
                    { typeof(double), new Tuple<object, object>(1d, 2d) },
                    { typeof(double?), new Tuple<object, object>(1d, 2d) },
                    { typeof(bool), new Tuple<object, object>(true, false) },
                    { typeof(bool?), new Tuple<object, object>(true, false) },
                    { typeof(float), new Tuple<object, object>(1, 2) },
                    { typeof(float?), new Tuple<object, object>(1, 2) },
                    { typeof(decimal), new Tuple<object, object>(1m, 2m) },
                    { typeof(decimal?), new Tuple<object, object>(1m, 2m) },
                    { typeof(short), new Tuple<object, object>((short)1, (short)2) },
                    { typeof(short?), new Tuple<object, object>((short)1, (short)2) },
                    { typeof(long), new Tuple<object, object>(1, 2) },
                    { typeof(long?), new Tuple<object, object>(1, 2) },
                    { typeof(DateTime), new Tuple<object, object>(new DateTime(2010, 1, 2), new DateTime(2010, 1, 3)) },
                    { typeof(DateTime?), new Tuple<object, object>(new DateTime(2010, 1, 2), new DateTime(2010, 1, 3)) },
                    { typeof(TimeSpan), new Tuple<object, object>(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)) },
                    { typeof(TimeSpan?), new Tuple<object, object>(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)) },
                    { typeof(object), new Tuple<object, object>(new object(), new object()) },
                };

            var propertyType = instance.GetPropertyType(propertyName);

            if (!values.ContainsKey(propertyType))
            {
                string message = "Could not assert property changed behaviour since there are no predefined " +
                                 "values for the given property type. Please specify values manually, " +
                                 "using the alternate overload of the assertion.";
                throw new Exception(message);
            }

            var valueTuple = values[propertyType];

            ShouldFirePropertyChangedFor(
                instance,
                propertyName,
                x => x.SetProperty(propertyName, valueTuple.Item1),
                x => x.SetProperty(propertyName, valueTuple.Item2));
        }

        public static void ShouldFirePropertyChangedFor<TInstance>(this TInstance instance, string propertyName, Action<TInstance> firstSetter, Action<TInstance> secondSetter)
            where TInstance : INotifyPropertyChanged
        {
            var propertyNames = new List<string>();

            firstSetter.Invoke(instance);

            instance.PropertyChanged += (sender, eventArgs) => propertyNames.Add(eventArgs.PropertyName);

            secondSetter.Invoke(instance);

            propertyNames.Contains(propertyName).ShouldBeTrue("No property changed event fired for property: " + propertyName);
        }
    }
}