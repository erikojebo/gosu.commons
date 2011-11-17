using System;
using Gosu.Commons.Internationalization;
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
            Assert.AreEqual("Expected name", (string)document.Name);
        }

        [Test]
        public void Child_element_containing_string_can_be_read_by_accessing_property_with_same_name()
        {
            var document = _parser.Parse("<Person><Name>Expected name</Name></Person>");
            Assert.AreEqual("Expected name", (string)document.Name);
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
            Assert.AreEqual("person 0", (string)people.Persons[0].Name);
            Assert.AreEqual("person 1", (string)people.Persons[1].Name);
            Assert.AreEqual("person 2", (string)people.Persons[2].Name);
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
            Assert.AreEqual("category 0", (string)root.Categories[0].Name);
            Assert.AreEqual("category 1", (string)root.Categories[1].Name);
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
            Assert.AreEqual("class 0", (string)root.Classes[0].Name);
            Assert.AreEqual("class 1", (string)root.Classes[1].Name);
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
            Assert.AreEqual("tube 0", (string)root.Tubes[0].Name);
            Assert.AreEqual("tube 1", (string)root.Tubes[1].Name);
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
            Assert.AreEqual("octupus 0", (string)root.OctupusElements[0].Name);
            Assert.AreEqual("octupus 1", (string)root.OctupusElements[1].Name);
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
        public void String_representation_of_an_element_is_its_value()
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
        public void String_representation_of_int_can_be_assigned_to_int_variable()
        {
            var root = _parser.Parse("<Root>123</Root>");

            int intVariable = root;

            Assert.AreEqual(123, intVariable);
        }

        [Test]
        public void String_representation_of_double_can_be_assigned_to_double_variable()
        {
            using (new TemporaryCulture("en-US"))
            {
                var root = _parser.Parse("<Root>123.456</Root>");

                double doubleVariable = root;

                Assert.AreEqual(123.456, doubleVariable, 0.000001);
            }
        }

        [Test]
        public void String_representation_of_float_can_be_assigned_to_float_variable()
        {
            using (new TemporaryCulture("en-US"))
            {
                var root = _parser.Parse("<Root>123.456</Root>");

                float floatVariable = root;

                Assert.AreEqual(123.456, floatVariable, 0.00001);
            }
        }

        [Test]
        public void String_representation_of_decimal_can_be_assigned_to_decimal_variable()
        {
            using (new TemporaryCulture("en-US"))
            {
                var root = _parser.Parse("<Root>123.456</Root>");

                decimal decimalVariable = root;

                Assert.AreEqual(123.456, decimalVariable);
            }
        }

        [Test]
        public void String_representation_of_date_time_can_be_assigned_to_date_time_variable()
        {
            var root = _parser.Parse("<Root>2011-12-13T14:15:16</Root>");

            DateTime dateTimeVariable = root;

            Assert.AreEqual(new DateTime(2011, 12, 13, 14, 15, 16), dateTimeVariable);
        }

        [Test]
        public void String_representation_of_time_span_can_be_assigned_to_time_span_variable()
        {
            var root = _parser.Parse("<Root>1:02:03:04</Root>");

            TimeSpan timeSpanVariable = root;

            Assert.AreEqual(new TimeSpan(1, 2, 3, 4), timeSpanVariable);
        }

        [Test]
        public void String_representation_of_bool_can_be_assigned_to_bool_variable()
        {
            var root = _parser.Parse("<Root>true</Root>");

            bool boolVariable = root;

            Assert.IsTrue(boolVariable);
        }

        [Test]
        public void Element_can_be_assigned_to_string_variable()
        {
            var root = _parser.Parse("<Root>Value</Root>");

            string stringVariable = root;

            Assert.AreEqual("Value", stringVariable);
        }

        [ExpectedException]
        [Test]
        public void Throws_exception_when_trying_to_convert_element_to_type_without_custom_converter_set()
        {
            var root = _parser.Parse("<Root>Value</Root>");

            DynamicXmlParserSpecs x = root;
        }

        [Test]
        public void Custom_converter_can_be_registered_for_default_type()
        {
            _parser.SetConverter(x => 123);

            var root = _parser.Parse("<Root>1</Root>");

            int intVariable = root;

            Assert.AreEqual(123, intVariable);
        }

        [Test]
        public void Custom_converter_can_be_registered_for_custom_type()
        {
            // Register converter for the DynamicXmlParserSpecs type
            _parser.SetConverter(x => this);

            var root = _parser.Parse("<Root>1</Root>");

            DynamicXmlParserSpecs variableWithCustomType = root;

            Assert.AreEqual(this, variableWithCustomType);
        }

        [Test]
        public void Can_assign_int_attribute_value_to_int_variable()
        {
            var root = _parser.Parse("<Root Value='1' />");
            int intVariable = root.Value;

            Assert.AreEqual(1, intVariable);
        }

        [Test]
        public void Can_assign_name_of_enum_member_in_attribute_to_enum_property()
        {
            var root = _parser.Parse("<Root Value='Value2' />");
            SomeEnum enumValue = root.Value;

            Assert.AreEqual(SomeEnum.Value2, enumValue);
        }
        
        [Test]
        public void Can_assign_name_of_enum_member_in_element_to_enum_property()
        {
            var root = _parser.Parse("<Root><Value>Value2</Value></Root>");
            SomeEnum enumValue = root.Value;

            Assert.AreEqual(SomeEnum.Value2, enumValue);
        }

        [Test]
        public void Can_assign_int_value_in_attribute_to_enum_property()
        {
            var root = _parser.Parse("<Root Value='20' />");
            SomeEnum enumValue = root.Value;

            Assert.AreEqual(SomeEnum.Value2, enumValue);
        }
        
        [Test]
        public void Can_assign_int_value_in_element_to_enum_property()
        {
            var root = _parser.Parse("<Root><Value>20</Value></Root>");
            SomeEnum enumValue = root.Value;

            Assert.AreEqual(SomeEnum.Value2, enumValue);
        }

        [Test]
        public void Custom_parser_can_be_set_for_when_converting_attribute_value_to_enum_type()
        {
            _parser.SetConverter(x => x == "expected" ? SomeEnum.Value2 : SomeEnum.Value1);

            var root = _parser.Parse("<Root Value='expected' />");
            SomeEnum enumValue = root.Value;

            Assert.AreEqual(SomeEnum.Value2, enumValue);
        }

        [Test]
        public void Custom_parser_can_be_set_for_when_converting_element_value_to_enum_type()
        {
            _parser.SetConverter(x => x == "expected" ? SomeEnum.Value2 : SomeEnum.Value1);

            var root = _parser.Parse("<Root><Value>expected</Value></Root>");
            SomeEnum enumValue = root.Value;

            Assert.AreEqual(SomeEnum.Value2, enumValue);
        }

        [Test]
        public void Values_of_child_element_can_be_read_by_accessing_properties_on_the_child()
        {
            var root = _parser.Parse(@"
<Root>
    <Person>
        <Address ZipCode='12345' />
    </Person>
</Root>");

            Assert.AreEqual(12345, (int)root.Person.Address.ZipCode);
        }

        [Test]
        public void Attributes_can_be_converted_to_basic_types()
        {
            using (new TemporaryCulture("en-US"))
            {
                var root = _parser.Parse(@"
<Person 
    Int='1'
    Float='1.1'
    Bool='true'
    DateTime='2011-01-01'
    TimeSpan='1:02:03' />");

                Assert.AreEqual(1, (int)root.Int);
                Assert.AreEqual(1.1, (float)root.Float, 0.0001);
                Assert.AreEqual(1.1, (decimal)root.Float);
                Assert.AreEqual(1.1, (double)root.Float, 0.0001);
                Assert.IsTrue((bool)root.Bool);
                Assert.AreEqual(new DateTime(2011, 1, 1), (DateTime)root.DateTime);
                Assert.AreEqual(new TimeSpan(1, 2, 3), (TimeSpan)root.TimeSpan);
            }
        }

        [Test]
        public void Attribute_can_be_converted_to_custom_type()
        {
            _parser.SetConverter(x => this);

            var root = _parser.Parse("<Root Value='1' />");

            Assert.AreEqual(this, (DynamicXmlParserSpecs)root.Value);
        }

        [Test]
        public void Hierarchy_can_be_read_as_an_object_model()
        {
            var people = _parser.Parse(@"
<People>
    <Person Name='person 0'>
        <Address Street='some street'>
            <ZipCode>12345</ZipCode>
        </Address>
    </Person>
    <Person Name='person 1' />
    <Person Name='person 2' />
    <SuperPerson IsSuper='true'>
        <IsNormal>False</IsNormal>
    </SuperPerson>
</People>
");

            Assert.AreEqual(3, people.Persons.Count);

            Assert.AreEqual("person 0", (string)people.Persons[0].Name);

            var firstPersonAddress = people.Persons[0].Address;

            Assert.AreEqual("some street", (string)firstPersonAddress.Street);
            Assert.AreEqual(12345, (int)firstPersonAddress.ZipCode);

            Assert.AreEqual("person 1", (string)people.Persons[1].Name);
            Assert.AreEqual("person 2", (string)people.Persons[2].Name);

            Assert.IsTrue((bool)people.SuperPerson.IsSuper);
            Assert.IsFalse((bool)people.SuperPerson.IsNormal);
        }

        [Test]
        public void Elements_in_different_namespaces_can_be_accessed_by_prefixing_element_name_with_namespace()
        {
            var xml =
                @"<?xml version='1.0' encoding='UTF-8' ?>
<!--  Here comes some XML -->
<Root xmlns='http://www.somesite.org/xml/DefaultNamespace' 
      xmlns:IR='http://www.somesite.org/xml' 
      xmlns:OtherNamespace='http://www.somesite.org/xml/OtherNamespace' 
      xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' 
      RootElementAttribute='1.0'>
	<IR:Header>
		<IR:FileDate>2006-04-18T10:38:42</IR:FileDate>
	</IR:Header>
	<IR:FileId>TestId</IR:FileId>
	<OtherNamespace:Value>Value 1</OtherNamespace:Value>
	<MyEntity Version='1.0'>
		<IR:Header>
		    <IR:FileDate>2006-04-18T10:38:42</IR:FileDate>
    	</IR:Header>
		<IR:EntityId>Entity id</IR:EntityId>
		<SomeData>
			<IR:DataFormat>Test format</IR:DataFormat>
			<Number>5.500</Number>
		</SomeData>
	</MyEntity>
</Root>
";
            using (new TemporaryCulture("en-US"))
            {
                _parser.SetNamespaceAlias("http://www.somesite.org/xml", "IR");
                _parser.SetNamespaceAlias("http://www.somesite.org/xml/OtherNamespace", "Other");

                var root = _parser.Parse(xml);

                Assert.AreEqual(new DateTime(2006, 4, 18, 10, 38, 42), (DateTime)root.IRHeader.IRFileDate);
                Assert.AreEqual(5.5, (double)root.MyEntity.SomeData.Number, 0.00001);
                Assert.AreEqual("Value 1", (string)root.OtherValue);
            }
        }

        [Test]
        public void Child_element_collections_in_other_namespaces_can_be_accessed_as_collection_properties()
        {
            var xml = @"<?xml version='1.0' encoding='UTF-8' ?>
<!--  Here comes some XML -->
<Root xmlns='http://www.somesite.org/xml/DefaultNamespace' 
      xmlns:IR='http://www.somesite.org/xml' 
      xmlns:OtherNamespace='http://www.somesite.org/xml/OtherNamespace' 
      xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' 
      RootElementAttribute='1.0'>
	<MyEntity Version='1.0'>
        <IR:Class>Class 1</IR:Class>
		<IR:Class>Class 2</IR:Class>
		<IR:Class>Class 3</IR:Class>
        <Category Value='1' />
		<Category Value='2' />		
        <OtherNamespace:Car>
			<Number>5</Number>
		</OtherNamespace:Car>
	</MyEntity>
</Root>
";

            _parser.SetNamespaceAlias("http://www.somesite.org/xml", "IR");
            _parser.SetNamespaceAlias("http://www.somesite.org/xml/OtherNamespace", "Other");

            var root = _parser.Parse(xml);

            Assert.AreEqual(3, root.MyEntity.IRClasses.Count);
            Assert.AreEqual(2, root.MyEntity.Categories.Count);
            Assert.AreEqual(1, root.MyEntity.OtherCars.Count);
        }
    }

    enum SomeEnum
    {
        Value1 = 10,
        Value2 = 20
    }
}