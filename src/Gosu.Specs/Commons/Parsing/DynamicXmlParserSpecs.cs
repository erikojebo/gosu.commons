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


        // FUTURE TEST CASES:

        /* Child elements that are simple strings:
         <Persons>
           <Person>person 0</Person>
           <Person>person 1</Person>
           <Person>person 2</Person>
         </Persons>
         */

        // Collections ending with 'y': <Root><Category /><Category /><Category /></Root>
        //                              root.Categories...

        // Implicit casting to int, double, datetime, timespan, decimal, bool, user defined type

        // Specifying custom conversions for different types
    }
}