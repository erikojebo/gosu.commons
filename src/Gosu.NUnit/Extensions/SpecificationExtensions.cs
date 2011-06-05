using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Gosu.Commons.Reflection;
using NUnit.Framework;

namespace Gosu.NUnit.Extensions
{
    public static class SpecificationExtensions
    {
        public static void ShouldNotBeNull(this object actual, string message = null)
        {
            Assert.IsNotNull(actual, message);
        }

        public static void ShouldBeNull(this object actual, string message = null)
        {
            Assert.IsNull(actual, message);
        }
        
        public static void ShouldEqual(this object actual, object expected, string message = null)
        {
            Assert.AreEqual(expected, actual, message);
        }
        
        public static void ShouldBeLessThan(this object actual, object other, string message = null)
        {
            Assert.That(actual, Is.LessThan(other), message);
        }

        public static void ShouldBeFalse(this bool actual, string message = null)
        {
            Assert.IsFalse(actual, message);
        }

        public static void ShouldBeTrue(this bool actual, string message = null)
        {
            Assert.IsTrue(actual, message);
        }

        public static void ShouldBe<T>(this object actual, string message = null)
        {
            Assert.IsInstanceOf<T>(actual, message);            
        }

        public static void ShouldBeEquivalentTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected, string message = null)
        {
            CollectionAssert.AreEquivalent(expected, actual, message);
        }

        public static void ShouldBeEmpty(this IEnumerable actual, string message = null)
        {
            CollectionAssert.IsEmpty(actual, message);
        }

        public static void ShouldNotBeEmpty(this IEnumerable actual, string message = null)
        {
            CollectionAssert.IsNotEmpty(actual, message);
        }

        public static void ShouldBeSameAs(this object actual, object expected, string message = null)
        {
            Assert.AreSame(expected, actual, message);
        }

        public static void ShouldContain<T>(this IEnumerable<T> actual, IEnumerable<T> expected, IEqualityComparer<T> comparer)
        {
            foreach (var x in expected)
            {
                actual.Contains(x, comparer)
                    .ShouldBeTrue("Expected collection to contain object: " + x);
            }
        }

        public static void ShouldContainString(this string actual, string expected, string message = null)
        {
            Assert.That(actual, Is.StringContaining(expected), message);
        }
        
        public static void ShouldContainStringIgnoringCase(this string actual, string expected, string message = null)
        {
            Assert.That(actual.ToLower(), Is.StringContaining(expected.ToLower()), message);
        }

        public static void ShouldFirePropertyChangedFor<TInstance>(this TInstance instance, string propertyName)
            where TInstance : INotifyPropertyChanged
        {
            var values = new Dictionary<Type, Tuple<object, object>>
                {
                    { typeof(int), new Tuple<object, object>(1, 2)},
                    { typeof(double), new Tuple<object, object>(1, 2)},
                    { typeof(bool), new Tuple<object, object>(1, 2)},
                    { typeof(float), new Tuple<object, object>(1, 2)},
                    { typeof(decimal), new Tuple<object, object>(1, 2)},
                    { typeof(short), new Tuple<object, object>(1, 2)},
                    { typeof(long), new Tuple<object, object>(1, 2)},
                    { typeof(DateTime), new Tuple<object, object>(new DateTime(2010, 1, 2), new DateTime(2010, 1, 3))},
                    { typeof(TimeSpan), new Tuple<object, object>(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2))},
                    { typeof(object), new Tuple<object, object>(new object(), new object())},
                };

            var propertyType = instance.GetPropertyType(propertyName);

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