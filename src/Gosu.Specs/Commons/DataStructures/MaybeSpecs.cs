using System;
using Gosu.Commons.DataStructures;
using NUnit.Framework;

namespace Gosu.Specs.Commons.DataStructures
{
    [TestFixture]
    public class MaybeSpecs
    {
        [Test]
        public void Nothing_is_equal_to_nothing()
        {
            Assert.AreEqual(Maybe<int>.Nothing, Maybe<int>.Nothing);
            Assert.IsTrue(Maybe<int>.Nothing == Maybe<int>.Nothing);
        }

        [Test]
        public void Nothing_is_not_equal_to_value()
        {
            Assert.AreNotEqual(Maybe<int>.Nothing, 1.ToMaybe());
        }

        [Test]
        public void Value_is_value_used_when_creating_instance()
        {
            Assert.AreEqual(1, 1.ToMaybe().Value);
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [Test]
        public void Accessing_value_on_nothing_throws_invalid_operation_exception()
        {
            var value = Maybe<int>.Nothing.Value;
        }

        [Test]
        public void Nothing_is_nothing()
        {
            Assert.IsTrue(Maybe<int>.Nothing.IsNothing);
        }

        [Test]
        public void Value_is_not_nothing()
        {
            Assert.IsFalse(1.ToMaybe().IsNothing);
        }

        [Test]
        public void Nothing_has_no_value()
        {
            Assert.IsFalse(Maybe<int>.Nothing.HasValue);
        }

        [Test]
        public void Value_has_value()
        {
            Assert.IsTrue(1.ToMaybe().HasValue);
        }
    }
}