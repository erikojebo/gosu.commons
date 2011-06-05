using Gosu.Commons.Reflection;
using NUnit.Framework;

namespace Gosu.Specs.Commons.Reflection
{
    [TestFixture]
    public class ReflectionExtensionsSpecs
    {
        private Class _instance;

        [SetUp]
        public void SetUp()
        {
            _instance = new Class();            
        }

        [Test]
        public void SetProperty_sets_property_with_given_name_to_given_value()
        {
            _instance.SetProperty("StringProperty", "Expected value");

            Assert.AreEqual("Expected value", _instance.StringProperty);
        }

        [Test]
        public void CoerceProperty_sets_string_value_without_modification()
        {
            _instance.CoerceProperty("StringProperty", "Expected value");

            Assert.AreEqual("Expected value", _instance.StringProperty);
        }

        [Test]
        public void CoerceProperty_converts_string_to_integer_when_setting_int_property()
        {
            _instance.CoerceProperty("IntProperty", "1");

            Assert.AreEqual(1, _instance.IntProperty);
        }

        [Test]
        public void CoerceProperty_converts_string_to_double_when_setting_double_property()
        {
            _instance.CoerceProperty("DoubleProperty", "1.23");
            Assert.AreEqual(1.23, _instance.DoubleProperty);
        }

        [Test]
        public void HasProperty_returns_false_if_property_does_not_exist()
        {
            Assert.IsFalse(_instance.HasProperty("NonExistingProperty"));
        }

        [Test]
        public void HasProperty_returns_true_if_property_exists()
        {
            Assert.IsTrue(_instance.HasProperty("StringProperty"));
        }

        [Test]
        public void GetPropertyType_returns_type_for_property_with_given_name()
        {
            Assert.AreEqual(typeof(double), _instance.GetPropertyType("DoubleProperty"));
        }

        private class Class
        {
            public string StringProperty { get; set; }
            public double DoubleProperty { get; set; }
            public int IntProperty { get; set; }
        }
    }
}