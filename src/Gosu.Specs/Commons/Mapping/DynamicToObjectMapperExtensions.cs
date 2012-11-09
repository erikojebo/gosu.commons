using System;
using System.Dynamic;
using Gosu.Commons.Mapping;
using NUnit.Framework;

namespace Gosu.Specs.Commons.Mapping
{
    [TestFixture]
    public class DynamicToObjectMapperExtensions
    {
        private ExpandoObject _values;

        private dynamic Values
        {
            get { return _values; }
        }

        [SetUp]
        public void SetUp()
        {
            _values = new ExpandoObject();
        }

        [Test]
        public void Mapping_to_a_type_without_properties_does_not_throw_exception()
        {
            new ClassWithoutProperties().FromDynamic(_values);
        }

        [Test]
        [ExpectedException(typeof(ValueNotFoundException), ExpectedMessage = "Property", MatchType = MessageMatch.Contains)]
        public void Mapping_all_properties_for_type_with_single_property_that_does_not_have_a_corresponding_value_throws_exception()
        {
            new ClassWithSingleProperty().FromDynamic(_values, MappingMode.AllProperties);
        }

        [Test]
        public void Single_property_with_corresponding_value_of_expected_type_is_mapped()
        {
            Values.Property = "property value";
            var instance = new ClassWithSingleProperty().FromDynamic(_values);

            Assert.AreEqual("property value", instance.Property);
        }

        [Test]
        public void Multiple_properties_with_corresponding_values_of_expected_type_are_mapped()
        {
            Values.StringProperty = "string value";
            Values.IntProperty = 123;

            var instance = new ClassWithMultipleProperties().FromDynamic(_values);

            Assert.AreEqual(123, instance.IntProperty);
            Assert.AreEqual("string value", instance.StringProperty);
        }

        [Test]
        public void Mapping_only_properties_with_values_for_instance_where_only_one_property_has_value_only_maps_that_property()
        {
            Values.IntProperty = 123;
            
            var instance = new ClassWithMultipleProperties().FromDynamic(_values);
            
            Assert.IsNull(instance.StringProperty);
            Assert.AreEqual(123, instance.IntProperty);
        }

        private class ClassWithoutProperties { }
        private class ClassWithSingleProperty
        {
            public string Property { get; set; }
        }

        private class ClassWithMultipleProperties
        {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
        }
    }
}