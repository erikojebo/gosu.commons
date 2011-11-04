using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    }
}