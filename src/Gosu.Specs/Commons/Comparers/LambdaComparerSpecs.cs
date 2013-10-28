using System;
using Gosu.Commons.Comparers;
using NUnit.Framework;

namespace Gosu.Specs.Commons.Comparers
{
    [TestFixture]
    public class LambdaComparerSpecs
    {
        private ClassWithProperties _right;
        private ClassWithProperties _left;

        [SetUp]
        public void SetUp()
        {
            _left = new ClassWithProperties { IntegerValue = 1, StringValue = "2", ClassReference = _left };
            _right = new ClassWithProperties { IntegerValue = 1, StringValue = "2", ClassReference = _left };
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Exception_is_thrown_if_no_value_getters_are_specified()
        {
            var comparer = new LambdaComparer<ClassWithProperties>();

            comparer.Equals(_left, _right);
        }

        [Test]
        public void Classes_with_single_equal_compared_value_are_equal()
        {
            var comparer = new LambdaComparer<ClassWithProperties>(x => x.IntegerValue);

            Assert.IsTrue(comparer.Equals(_left, _right));
        }

        [Test]
        public void Classes_with_single_non_equal_compared_value_are_different()
        {
            _right.IntegerValue = -1;

            var comparer = new LambdaComparer<ClassWithProperties>(x => x.IntegerValue);

            Assert.IsFalse(comparer.Equals(_left, _right));
        }
        
        [Test]
        public void Classes_with_single_compared_null_value_are_equal()
        {
            _right.StringValue = null;
            _left.StringValue = null;

            var comparer = new LambdaComparer<ClassWithProperties>(x => x.StringValue);

            Assert.IsTrue(comparer.Equals(_left, _right));
        }

        [Test]
        public void Classes_with_multiple_compared_values_for_which_only_the_first_is_equal_are_different()
        {
            _right.StringValue = null;

            var comparer = new LambdaComparer<ClassWithProperties>(x => x.IntegerValue, x => x.StringValue);

            Assert.IsFalse(comparer.Equals(_left, _right));
        }
        
        [Test]
        public void Classes_with_multiple_equal_compared_valeus_are_equal()
        {
            var comparer = new LambdaComparer<ClassWithProperties>(x => x.IntegerValue, x => x.StringValue);

            Assert.IsTrue(comparer.Equals(_left, _right));
        }

        [Test]
        public void Computed_values_can_be_used_for_comparison()
        {
            _left.StringValue = "ABC";
            _right.StringValue = "abc";
            _left.IntegerValue = 1;
            _right.IntegerValue = 2;

            var comparer = new LambdaComparer<ClassWithProperties>(
                x => x.IntegerValue > 0, 
                x => x.StringValue.ToLower(), 
                x => x.StringValue.ToUpper() + x.IntegerValue / x.IntegerValue,
                x => true);

            Assert.IsTrue(comparer.Equals(_left, _right));
        }

        private class ClassWithProperties
        {
            public int IntegerValue { get; set; }
            public string StringValue { get; set; }
            public ClassWithProperties ClassReference { get; set; }
        }
    }
}