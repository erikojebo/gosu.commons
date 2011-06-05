using Gosu.Commons.Comparers;
using NUnit.Framework;

namespace Gosu.Specs.Commons.Comparers
{
    [TestFixture]
    public class PropertyComparerSpecs
    {
        private ClassWithProperties _instance;
        private PropertyComparer _comparer;
        private ClassWithProperties _referencedInstance;
        private ClassWithProperties _instanceWithSameValues;

        [SetUp]
        public void SetUp()
        {
            _referencedInstance = new ClassWithProperties();

            _instance = new ClassWithProperties
            {
                IntegerValue = 2,
                StringValue = "string",
                ClassReference = _referencedInstance
            };

            _instanceWithSameValues = new ClassWithProperties
            {
                IntegerValue = 2,
                StringValue = "string",
                ClassReference = _referencedInstance
            };

            _comparer = new PropertyComparer();
        }

        [Test]
        public void Instance_is_equal_to_self()
        {
            var areEqual = _comparer.Equals(_instance, _instance);

            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Instance_is_not_equal_to_null()
        {
            var areEqual = _comparer.Equals(_instance, null);

            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Instance_is_not_equal_to_value_of_other_type()
        {
            var areEqual = _comparer.Equals(_instance, 1);

            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Instance_is_not_equal_to_instance_with_other_values()
        {
            var instanceWithOtherValues = new ClassWithProperties
            {
                IntegerValue = 0,
                StringValue = "other string",
                ClassReference = new ClassWithProperties()
            };

            var areEqual = _comparer.Equals(_instance, instanceWithOtherValues);

            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Instance_is_equal_to_other_instance_with_same_values()
        {
            var areEqual = _comparer.Equals(_instance, _instanceWithSameValues);

            Assert.IsTrue(areEqual);
        }

        private class ClassWithProperties
        {
            public int IntegerValue { get; set; }
            public string StringValue { get; set; }
            public ClassWithProperties ClassReference { get; set; }
        }
    }
}