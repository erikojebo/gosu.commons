using System;
using System.Collections;
using System.Collections.Generic;
using Gosu.Commons.Mapping;
using NUnit.Framework;

namespace Gosu.Specs.Commons.Mapping
{
    [TestFixture]
    public class ObjectMapperSpecs
    {
        private SourceClass _source;
        private ObjectMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _source = new SourceClass();
            _mapper = new ObjectMapper();
        }

        [Test]
        public void Single_property_with_same_name_is_mapped()
        {
            var target = new TargetWithMatchingSingleProperty();

            _mapper.Map(_source, target);

            Assert.AreEqual("string value 1", target.StringProperty1);
        }

        [Test]
        public void Multiple_properties_with_same_name_are_mapped()
        {
            var target = new TargetWithMatchingProperties();

            _mapper.Map(_source, target);

            Assert.AreEqual("string value 1", target.StringProperty1);
            Assert.AreEqual("string value 2", target.StringProperty2);
            Assert.AreEqual(1, target.IntProperty);
        }

        [Test]
        public void Properties_in_target_but_not_in_source_are_ignored()
        {
            var target = new TargetWithMorePropertiesThanSource();

            _mapper.Map(_source, target);

            Assert.AreEqual(0, target.DoubleProperty);
        }

        [Test]
        public void Mapping_without_a_target_object_creates_a_new_instance()
        {
            var target = _mapper.Map<TargetWithMorePropertiesThanSource>(_source);

            Assert.AreEqual("string value 1", target.StringProperty1);
            Assert.AreEqual("string value 2", target.StringProperty2);
            Assert.IsTrue(target.BoolProperty);
            Assert.AreEqual(1, target.IntProperty);
            Assert.AreEqual(0, target.DoubleProperty);
        }

        [Test]
        public void A_convention_can_be_used_to_match_properties_with_different_names()
        {
            var target = new TargetWithDifferentNames();

            _mapper.Map(_source, target, x => x.Convention(p => "SomeOtherNameFor" + p.Name));

            Assert.AreEqual("string value 1", target.SomeOtherNameForStringProperty1);
            Assert.AreEqual("string value 2", target.SomeOtherNameForStringProperty2);
        }

        [Test]
        public void Matching_properties_can_be_ignored_though_the_configuration()
        {
            var target = _mapper.Map<SourceClass, TargetWithMatchingProperties>(
                _source,
                x => x.Ignore(t => t.StringProperty1)
                      .Ignore(t => t.IntProperty));

            Assert.IsNull(target.StringProperty1);
            Assert.AreEqual("string value 2", target.StringProperty2);
            Assert.AreEqual(0, target.IntProperty);
        }

        [Test]
        public void Custom_functions_can_be_specified_for_getting_values_of_mapped_properties()
        {
            var target = _mapper.Map<SourceClass, TargetWithMatchingProperties>(
                _source,
                x => x.Custom(t => t.StringProperty1, s => s.StringProperty2.ToUpper())
                      .Custom(t => t.IntProperty, s => 123));

            Assert.AreEqual("STRING VALUE 2", target.StringProperty1);
            Assert.AreEqual("string value 2", target.StringProperty2);
            Assert.AreEqual(123, target.IntProperty);
        }

        [Test]
        public void Properties_in_the_target_which_have_custom_mapping_functions_are_mapped_even_though_they_have_no_matching_property_in_the_source_object()
        {
            var target = _mapper.Map<SourceClass, TargetWithMorePropertiesThanSource>(
                _source,
                x => x.Custom(t => t.DoubleProperty, s => 123));

            Assert.AreEqual(123, target.DoubleProperty);
        }

        [Test]
        public void Collection_of_objects_can_be_mapped_to_new_collection_of_mapped_items()
        {
            var sources = new List<SourceClass>
                {
                    new SourceClass { StringProperty1 = "first string value" },
                    new SourceClass { StringProperty1 = "second string value"}
                };

            var targets = _mapper.Map<List<TargetWithMatchingProperties>>(sources);

            Assert.AreEqual(2, targets.Count);
            Assert.AreEqual("first string value", targets[0].StringProperty1);
            Assert.AreEqual("second string value", targets[1].StringProperty1);
        }

        [Test]
        public void Items_in_matching_collection_are_mapped_to_new_instances_and_added_to_the_target_collection()
        {
            var source = new SourceClassWithCollection();
            source.Children = new[] { new SourceChild1 { ChildId = 1 }, new SourceChild1 { ChildId = 2 } };

            var target = _mapper.Map<TargetClassWithCollection>(source);

            Assert.AreEqual("string value 1", target.StringProperty1);
            Assert.AreEqual(2, target.Children.Count);
            Assert.AreEqual(1, target.Children[0].ChildId);
            Assert.AreEqual(2, target.Children[1].ChildId);
        }
        
        [Test]
        public void Mapping_object_with_non_null_collection_to_object_with_null_collection_creates_list()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void Mapping_object_with_non_null_array_to_object_with_null_collection_creates_list()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void List_can_be_mapped_to_array()
        {
            var sources = new List<SourceClass>
                {
                    new SourceClass { StringProperty1 = "first string value" },
                    new SourceClass { StringProperty1 = "second string value"}
                };

            var targets = (TargetWithMatchingProperties[])_mapper.Map(typeof(TargetWithMatchingProperties[]), sources);

            Assert.AreEqual(2, targets.Length);
            Assert.AreEqual("first string value", targets[0].StringProperty1);
            Assert.AreEqual("second string value", targets[1].StringProperty1);
        }

        [Test]
        public void List_can_be_mapped_to_IList()
        {
            var sources = new List<SourceClass>
                {
                    new SourceClass { StringProperty1 = "first string value" },
                    new SourceClass { StringProperty1 = "second string value"}
                };

            var targets = (IList<TargetWithMatchingProperties>)_mapper.Map(typeof(IList<TargetWithMatchingProperties>), sources);

            Assert.AreEqual(2, targets.Count);
            Assert.AreEqual("first string value", targets[0].StringProperty1);
            Assert.AreEqual("second string value", targets[1].StringProperty1);
        }

        private class SourceClass
        {
            public SourceClass()
            {
                StringProperty1 = "string value 1";
                StringProperty2 = "string value 2";
                IntProperty = 1;
                BoolProperty = true;
            }

            public string StringProperty1 { get; set; }
            public string StringProperty2 { get; set; }
            public int IntProperty { get; set; }
            public bool BoolProperty { get; set; }
        }

        private class SourceClassWithCollection
        {
            public SourceClassWithCollection()
            {
                StringProperty1 = "string value 1";
            }

            public string StringProperty1 { get; set; }
            public IList<SourceChild1> Children { get; set; }
        }

        private class SourceChild1
        {
            public int ChildId { get; set; } 
        }

        private class TargetWithMatchingProperties
        {
            public string StringProperty1 { get; set; }
            public string StringProperty2 { get; set; }
            public int IntProperty { get; set; }
        }

        private class TargetWithMorePropertiesThanSource
        {
            public string StringProperty1 { get; set; }
            public string StringProperty2 { get; set; }
            public int IntProperty { get; set; }
            public bool BoolProperty { get; set; }
            public double DoubleProperty { get; set; }
        }

        private class TargetWithMatchingSingleProperty
        {
            public string StringProperty1 { get; set; }
        }

        private class TargetWithDifferentNames
        {
            public string SomeOtherNameForStringProperty1 { get; set; }
            public string SomeOtherNameForStringProperty2 { get; set; }
        }

        private class TargetClassWithCollection
        {
            public string StringProperty1 { get; set; }
            public List<TargetChild1> Children { get; set; }
        }

        private class TargetChild1
        {
            public int ChildId { get; set; } 
        }
    }
}