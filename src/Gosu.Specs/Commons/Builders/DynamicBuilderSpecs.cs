using System;
using System.Collections.Generic;
using System.Linq;
using Gosu.Commons.Builders;
using Gosu.Commons.Builders.Exceptions;
using NUnit.Framework;

namespace Gosu.Specs.Commons.Builders
{
    [TestFixture]
    public class DynamicBuilderSpecs
    {
        private dynamic _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new DynamicBuilder<SomeClass>();
        }

        [Test]
        public void Sets_property_with_given_name_to_given_value()
        {
            var actual = _builder
                .SomeInt(1)
                .Build();

            Assert.AreEqual(1, actual.SomeInt);
        }

        [Test]
        public void Throws_when_trying_to_set_non_existing_property()
        {
            Assert.Throws<MissingPropertyException>(() => _builder.NonExistingProperty(1));
        }

        [Test]
        public void Adds_child_when_calling_singular_collection_name_method_with_child_instance()
        {
            SomeClass actual = _builder
                .SubclassObject(new PropertyValueCollector())
                .Build();

            Assert.AreEqual(1, actual.SubclassObjects.Count);
        }

        [Test]
        public void Adds_child_when_calling_singular_collection_name_method_with_property_value_collector()
        {
            var expectedChild = new SubclassObject();

            SomeClass actual = _builder
                .SubclassObject(expectedChild)
                .Build();

            var actualChild = actual.SubclassObjects.FirstOrDefault();

            Assert.AreEqual(1, actual.SubclassObjects.Count);
            Assert.AreSame(expectedChild, actualChild);
        }

        [Test]
        public void Adds_child_with_properties_set_on_added_instance_when_adding_plural_s_results_in_collection_name()
        {
            dynamic collector = new PropertyValueCollector();

            SomeClass actual = _builder
                .SubclassObject(collector.SomeString("string").SomeDouble(1.23))
                .Build();

            var actualChild = actual.SubclassObjects.FirstOrDefault();

            Assert.AreEqual(1, actual.SubclassObjects.Count);
            Assert.AreEqual("string", actualChild.SomeString);
            Assert.AreEqual(1.23, actualChild.SomeDouble);
        }

        [Test]
        public void Can_set_date_property_to_date_value()
        {
            var actual = _builder
                .SomeDate(new DateTime(2010, 1, 2, 3, 4, 5))
                .Build();

            Assert.AreEqual(new DateTime(2010, 1, 2, 3, 4, 5), actual.SomeDate);
        }

        [Test]
        public void Can_set_date_property_to_string_representation_of_date()
        {
            var actual = _builder
                .SomeDate("2010-01-02 03:04:05")
                .Build();

            Assert.AreEqual(new DateTime(2010, 1, 2, 3, 4, 5), actual.SomeDate);
        }

        [Test]
        public void Uses_add_method_when_adding_child_instance_to_enumerable_collection()
        {
            var expectedChild = new SubclassObject();

            SomeClass actual = _builder
                .EnumerableObject(expectedChild)
                .Build();

            var actualChild = actual.EnumerableObjects.FirstOrDefault();

            Assert.AreEqual(1, actual.EnumerableObjects.Count());
            Assert.AreSame(expectedChild, actualChild);
        }

        [Test]
        public void Adds_created_and_initialized_child_when_adding_with_property_collector_to_enumerable_collection()
        {
            dynamic collector = new PropertyValueCollector();

            SomeClass actual = _builder
                .EnumerableObject(collector.SomeString("string").SomeDouble(1.23))
                .Build();

            var actualChild = actual.EnumerableObjects.FirstOrDefault();

            Assert.AreEqual(1, actual.EnumerableObjects.Count());
            Assert.AreEqual("string", actualChild.SomeString);
            Assert.AreEqual(1.23, actualChild.SomeDouble);
        }

        private class SomeClass
        {
            private IList<SubclassObject> _enumerableObjects;

            public SomeClass()
            {
                SubclassObjects = new List<SubclassObject>();
                _enumerableObjects = new List<SubclassObject>();
            }

            public int SomeInt { get; set; }
            public DateTime SomeDate { get; set; }
            public IList<SubclassObject> SubclassObjects { get; private set; }

            public IEnumerable<SubclassObject> EnumerableObjects
            {
                get { return _enumerableObjects; }
            }

            public void AddEnumerableObject(SubclassObject o)
            {
                _enumerableObjects.Add(o);
            }
        }

        private class SubclassObject
        {
            public string SomeString { get; set; }
            public double SomeDouble { get; set; }
        }
    }
}