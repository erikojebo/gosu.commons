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
    }
}