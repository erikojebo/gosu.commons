using System;
using System.Collections.Generic;
using System.Linq;
using Gosu.Commons.Parsing;
using NUnit.Framework;

namespace Gosu.Specs.Commons.Parsing
{
    [TestFixture]
    public class DynamicXmlParserSpecs
    {
        private IDynamicXmlParser _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new DynamicXmlParser();
        }

        [ExpectedException(typeof(MissingValueException))]
        [Test]
        public void Throws_when_accessing_property_that_does_not_exist()
        {
            var document = _parser.Parse("<Person />");
            var value = document.NonExistingValue;
        }

        [ExpectedException(typeof(MissingValueException), MatchType = MessageMatch.Contains, ExpectedMessage = "NonExistingValue")]
        [Test]
        public void Throws_exception_containing_property_name_accesed_when_accessing_property_that_does_not_exist()
        {
            var document = _parser.Parse("<Person />");
            var value = document.NonExistingValue;
        }

        [Test]
        public void Attribute_of_element_can_be_read_by_accessing_property_with_same_name()
        {
            var document = _parser.Parse("<Person Name='Expected name' />");
            Assert.AreEqual("Expected name", document.Name);
        }

        [Test]
        public void Child_element_containing_string_can_be_read_by_accessing_property_with_same_name()
        {
            var document = _parser.Parse("<Person><Name>Expected name</Name></Person>");
            Assert.AreEqual("Expected name", document.Name);
        }

        [Test]
        public void Child_collection_can_be_accessed_through_property_with_pluralized_name()
        {
            var people = _parser.Parse(@"
<People>
    <Person Name='person 0' />
    <Person Name='person 1' />
    <Person Name='person 2' />
</People>
");

            Assert.AreEqual(3, people.Persons.Count);
            Assert.AreEqual("person 0", people.Persons[0].Name);
            Assert.AreEqual("person 1", people.Persons[1].Name);
            Assert.AreEqual("person 2", people.Persons[2].Name);
        }

        [Test]
        public void Category_child_elements_can_be_accessed_through_Categories_property()
        {
            var root = _parser.Parse(@"
<Root>
    <Category Name='category 0' />
    <Category Name='category 1' />
</Root>
");
    
            Assert.AreEqual(2, root.Categories.Count);
            Assert.AreEqual("category 0", root.Categories[0].Name);
            Assert.AreEqual("category 1", root.Categories[1].Name);
        }

        [Test]
        public void Class_child_elements_can_be_accessed_through_Classes_property()
        {
            var root = _parser.Parse(@"
<Root>
    <Class Name='class 0' />
    <Class Name='class 1' />
</Root>
");

            Assert.AreEqual(2, root.Classes.Count);
            Assert.AreEqual("class 0", root.Classes[0].Name);
            Assert.AreEqual("class 1", root.Classes[1].Name);
        }

        [Test]
        public void Tube_child_elements_can_be_accessed_through_Tubes_property()
        {
            var root = _parser.Parse(@"
<Root>
    <Tube Name='tube 0' />
    <Tube Name='tube 1' />
</Root>
");

            Assert.AreEqual(2, root.Tubes.Count);
            Assert.AreEqual("tube 0", root.Tubes[0].Name);
            Assert.AreEqual("tube 1", root.Tubes[1].Name);
        }

        [Test]
        public void Octupus_child_elements_can_be_accessed_through_OctupusElements_property()
        {
            var root = _parser.Parse(@"
<Root>
    <Octupus Name='octupus 0' />
    <Octupus Name='octupus 1' />
</Root>
");

            Assert.AreEqual(2, root.OctupusElements.Count);
            Assert.AreEqual("octupus 0", root.OctupusElements[0].Name);
            Assert.AreEqual("octupus 1", root.OctupusElements[1].Name);
        }

        [Test]
        public void Elements_with_names_colliding_with_pluralization_conventions_can_be_accessed_through_elements_method()
        {
            var root = _parser.Parse(@"
<Root>
    <Classes Name='classes 0' />
    <Categories Name='categories 0' />
    <Persons Name='persons 0' />
    <ChildElements Name='child elements 0' />
</Root>
");

            Assert.AreEqual(1, root.Elements("Classes").Count);
            Assert.AreEqual(1, root.Elements("Categories").Count);
            Assert.AreEqual(1, root.Elements("Persons").Count);
            Assert.AreEqual(1, root.Elements("ChildElements").Count);
        }

        [Test]
        public void String_child_elements_can_be_accessed_through_collection_property()
        {
            var root = _parser.Parse(@"
<Root>
    <Category>category 0</Category>
    <Category>category 1</Category>
</Root>
");

            Assert.AreEqual(2, root.Categories.Count);
            Assert.AreEqual("category 0", (string)root.Categories[0]);
            Assert.AreEqual("category 1", (string)root.Categories[1]);
        }

        [Test]
        public void String_representation_of_int_can_be_assigned_to_int_property()
        {
            Assert.Fail("Not implemented");
        }

    
        // Implicit casting to int, double, datetime, timespan, decimal, bool, user defined type

        // Specifying custom conversions for different types
    }
}