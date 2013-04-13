using System.Collections.Generic;
using System.ComponentModel;
using Gosu.Commons.Reflection;
using NUnit.Framework;

namespace Gosu.Specs.Commons.Reflection
{
    [TestFixture]
    public class ReflectionExtensionsSpecs
    {
        private Class _instance;
        private Class _subclassInstanceTypedAsBase;
        private Subclass _subclassInstance;

        [SetUp]
        public void SetUp()
        {
            _instance = new Class();
            _subclassInstance = new Subclass();
            _subclassInstanceTypedAsBase = _subclassInstance;
        }

        [Test]
        public void SetProperty_sets_property_with_given_name_to_given_value()
        {
            _instance.SetProperty("StringProperty", "Expected value");

            Assert.AreEqual("Expected value", _instance.StringProperty);
        }

        [Test]
        public void SetProperty_sets_subclass_property_for_base_class_instance()
        {
            _subclassInstanceTypedAsBase.SetProperty("SubclassProperty", "Expected value");

            Assert.AreEqual("Expected value", _subclassInstance.SubclassProperty);
        }

        [Test]
        public void CoerceProperty_sets_string_value_without_modification()
        {
            _instance.CoerceProperty("StringProperty", "Expected value");

            Assert.AreEqual("Expected value", _instance.StringProperty);
        }

        [Test]
        public void CoerceProperty_sets_subclass_property_for_base_class_instance()
        {
            _subclassInstanceTypedAsBase.CoerceProperty("SubclassProperty", "Expected value");

            Assert.AreEqual("Expected value", _subclassInstance.SubclassProperty);
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
        public void CoercePropertyDefaultingMissingValues_sets_property_to_value_when_value_is_non_empty()
        {
            _instance.CoercePropertyDefaultingMissingValues("DoubleProperty", "1.23");
            Assert.AreEqual(1.23, _instance.DoubleProperty);
        }

        [Test]
        public void CoercePropertyDefaultingMissingValues_sets_double_to_zero_for_null_value()
        {
            _instance.CoercePropertyDefaultingMissingValues("DoubleProperty", null);
            Assert.AreEqual(0, _instance.DoubleProperty);
        }

        [Test]
        public void CoercePropertyDefaultingMissingValues_sets_double_to_zero_for_empty_value()
        {
            _instance.CoercePropertyDefaultingMissingValues("DoubleProperty", "");
            Assert.AreEqual(0, _instance.DoubleProperty);
        }
        
        [Test]
        public void CoercePropertyDefaultingMissingValues_resets_double_to_zero_for_empty_value()
        {
            _instance.DoubleProperty = 1;

            _instance.CoercePropertyDefaultingMissingValues("DoubleProperty", "");
            Assert.AreEqual(0, _instance.DoubleProperty);
        }

        [Test]
        public void HasProperty_returns_false_if_property_does_not_exist()
        {
            Assert.IsFalse(_instance.HasProperty("NonExistingProperty"));
        }

        [Test]
        public void HasProperty_finds_subclass_property_for_instance_typed_as_base_class()
        {
            Assert.IsTrue(_subclassInstanceTypedAsBase.HasProperty("SubclassProperty"));
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

        [Test]
        public void GetPropertyType_finds_subclass_property_for_instance_typed_as_base_class()
        {
            Assert.AreEqual(typeof(string), _subclassInstanceTypedAsBase.GetPropertyType("SubclassProperty"));
        }

        [Test]
        public void TryCallMethod_calls_method_with_given_name()
        {
            _instance.TryCallMethod("Method1");
            Assert.IsTrue(_instance.WasMethod1Called);
        }

        [Test]
        public void TryCallMethod_returns_true_when_method_is_called()
        {
            Assert.IsTrue(_instance.TryCallMethod("Method1"));
        }

        [Test]
        public void TryCallMethod_returns_false_when_method_is_not_found()
        {
            Assert.IsFalse(_instance.TryCallMethod("NonExistingMethod"));
        }

        [Test]
        public void TryCallMethod_calls_subclass_method_in_instance_typed_as_base_class()
        {
            _subclassInstanceTypedAsBase.TryCallMethod("SubclassMethod");
            Assert.IsTrue(_subclassInstance.WasSubclassMethodCalled);
        }

        [Test]
        public void GetDefaultValue_returns_null_for_reference_type()
        {
            var defaultValue = _instance.GetType().GetDefaultValue();

            Assert.IsNull(defaultValue);
        }

        [Test]
        public void GetDefaultValue_returns_zero_for_double()
        {
            var defaultValue = typeof(double).GetDefaultValue();

            Assert.AreEqual(0, defaultValue);
        }

        [Test]
        public void ResetToDefaultValue_sets_reference_property_to_null()
        {
            _instance.StringProperty = "value";
            _instance.ResetToDefaultValue("StringProperty");

            Assert.IsNull(_instance.StringProperty);
        }

        [Test]
        public void ResetToDefaultValue_sets_double_property_to_zero()
        {
            _instance.DoubleProperty = 1;
            _instance.ResetToDefaultValue("DoubleProperty");

            Assert.AreEqual(0, _instance.DoubleProperty);
        }

        [Test]
        public void HasGenericTypeDefinition_IEnumerable_of_T_returns_true_for_List_of_T()
        {
            Assert.IsTrue(typeof(List<int>).HasGenericTypeDefinition(typeof(IEnumerable<>)));
        }

        [Test]
        public void HasGenericTypeDefinition_IEnumerable_of_T_returns_true_for_IList_of_T()
        {
            Assert.IsTrue(typeof(IList<int>).HasGenericTypeDefinition(typeof(IEnumerable<>)));
        }
        
        [Test]
        public void HasGenericTypeDefinition_IEnumerable_of_T_returns_true_for_non_generic_custom_collection_class_derrived_from_List_of_T()
        {
            Assert.IsTrue(typeof(CustomCollection).HasGenericTypeDefinition(typeof(IEnumerable<>)));
        }

        private class CustomCollection : List<int>
        {
            
        }

        private class Class
        {
            public bool WasMethod1Called;

            public string StringProperty { get; set; }
            public double DoubleProperty { get; set; }
            public int IntProperty { get; set; }

            public void Method1()
            {
                WasMethod1Called = true;
            }
        }

        private class Subclass : Class
        {
            public bool WasSubclassMethodCalled;

            public string SubclassProperty { get; set; }
            
            public void SubclassMethod()
            {
                WasSubclassMethodCalled = true;
            }
        }
    }
}