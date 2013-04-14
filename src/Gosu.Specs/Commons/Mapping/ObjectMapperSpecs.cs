using System;
using System.Collections.Generic;
using System.Linq;
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
                    new SourceClass { StringProperty1 = "second string value" }
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
            var source = new SourceClassWithCollection();
            source.Children = new[] { new SourceChild1 { ChildId = 1 }, new SourceChild1 { ChildId = 2 } };

            var target = new TargetClassWithCollection();
            target.Children = null;

            _mapper.Map(source, target);

            Assert.AreEqual("string value 1", target.StringProperty1);
            Assert.AreEqual(2, target.Children.Count);
            Assert.AreEqual(1, target.Children[0].ChildId);
            Assert.AreEqual(2, target.Children[1].ChildId);
        }

        [Test]
        public void Mapping_object_with_non_null_array_to_object_with_null_collection_creates_list()
        {
            var source = new SourceClassWithArray();
            source.Children = new[] { new SourceChild1 { ChildId = 1 }, new SourceChild1 { ChildId = 2 } };

            var target = _mapper.Map<TargetClassWithCollection>(source);

            Assert.AreEqual("string value 1", target.StringProperty1);
            Assert.AreEqual(2, target.Children.Count);
            Assert.AreEqual(1, target.Children[0].ChildId);
            Assert.AreEqual(2, target.Children[1].ChildId);
        }

        [Test]
        public void List_can_be_mapped_to_array()
        {
            var sources = new List<SourceClass>
                {
                    new SourceClass { StringProperty1 = "first string value" },
                    new SourceClass { StringProperty1 = "second string value" }
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
                    new SourceClass { StringProperty1 = "second string value" }
                };

            var targets = (IList<TargetWithMatchingProperties>)_mapper.Map(typeof(IList<TargetWithMatchingProperties>), sources);

            Assert.AreEqual(2, targets.Count);
            Assert.AreEqual("first string value", targets[0].StringProperty1);
            Assert.AreEqual("second string value", targets[1].StringProperty1);
        }

        [Test]
        public void IEnumerable_can_be_mapped_to_IEnumerable()
        {
            IEnumerable<SourceClass> sources = new List<SourceClass>
                {
                    new SourceClass { StringProperty1 = "first string value" },
                    new SourceClass { StringProperty1 = "second string value" }
                };

            var targets = (IEnumerable<TargetWithMatchingProperties>)_mapper.Map(typeof(IEnumerable<TargetWithMatchingProperties>), sources);

            Assert.AreEqual(2, targets.Count());
            Assert.AreEqual("first string value", targets.ElementAt(0).StringProperty1);
            Assert.AreEqual("second string value", targets.ElementAt(1).StringProperty1);
        }

        [Test]
        public void Mapping_null_collection_sets_target_collection_to_null()
        {
            IList<SourceClass> sources = null;
            var targets = (IEnumerable<TargetWithMatchingProperties>)_mapper.Map(typeof(IEnumerable<TargetWithMatchingProperties>), sources);

            Assert.IsNull(targets);
        }

        [Test]
        public void Enums_are_mapped_by_int_value()
        {
            var source = new RelatedEntity { Enum = SomeEnum.Value2 };
            var target = _mapper.Map<RelatedDTO>(source);

            Assert.AreEqual(SomeOtherEnum.OtherValue2, target.Enum);
        }

        [Test]
        public void Mapping_is_done_recursively()
        {
            var entity = new RootEntity
                {
                    String = "string 1",
                    Related = new RelatedEntity
                        {
                            Enum = SomeEnum.Value3
                        },
                    Children = new[]
                        {
                            new ChildEntity()
                                {
                                    Int = 1,
                                    GrandChildren = null,
                                },
                            new ChildEntity()
                                {
                                    Int = 2,
                                    GrandChildren = new List<GrandChildEntity>()
                                        {
                                            new GrandChildEntity
                                                {
                                                    Double = 1.23,
                                                    AnotherRelated = new RelatedEntity
                                                        {
                                                            Enum = SomeEnum.Value2
                                                        }
                                                },
                                            new GrandChildEntity
                                                {
                                                    Double = 2.34,
                                                    AnotherRelated = null
                                                }
                                        }
                                },
                        }
                };

            var rootDto = _mapper.Map<RootDTO>(entity);

            Assert.AreEqual("string 1", rootDto.String);
            Assert.IsNotNull(rootDto.Related);
            Assert.AreEqual(SomeOtherEnum.OtherValue3, rootDto.Related.Enum);
            Assert.AreEqual(2, rootDto.Children.Count());

            var firstChildDto = rootDto.Children.First();
            var secondChildDto = rootDto.Children.Last();
            
            Assert.AreEqual(1, firstChildDto.Int);
            Assert.IsNull(firstChildDto.GrandChildren);

            Assert.AreEqual(2, secondChildDto.Int);
            Assert.IsNotNull(secondChildDto.GrandChildren);
            Assert.AreEqual(2, secondChildDto.GrandChildren.Count());
            Assert.AreEqual(1.23, secondChildDto.GrandChildren.First().Double);
            Assert.IsNotNull(secondChildDto.GrandChildren.First().AnotherRelated);
            Assert.AreEqual(SomeOtherEnum.OtherValue2, secondChildDto.GrandChildren.First().AnotherRelated.Enum);
            Assert.IsNull(secondChildDto.GrandChildren.Last().AnotherRelated);
            Assert.AreEqual(2.34, secondChildDto.GrandChildren.Last().Double);
        }

        [Test]
        public void Configurations_can_be_added_beforehand()
        {
            // Config that should not be used:
            _mapper.ConfigureMap<SourceClass, TargetWithMatchingProperties>(x => x.Convention(p => "AnotherPrefixThatDoesntMatch" + p.Name));

            // Config that should be used:
            _mapper.ConfigureMap<SourceClass, TargetWithDifferentNames>(x => x.Convention(p => "SomeOtherNameFor" + p.Name));

            var target = new TargetWithDifferentNames();

            _mapper.Map(_source, target);

            Assert.AreEqual("string value 1", target.SomeOtherNameForStringProperty1);
            Assert.AreEqual("string value 2", target.SomeOtherNameForStringProperty2);
        }

        [Test]
        public void User_with_orders_example()
        {
            var user = new User
                {
                    Username = "User 1",
                    Password = "Password 1",
                    Id = 123,
                    Orders = new List<Order>
                        {
                            new Order { Id = 234 },
                            new Order { Id = 345 }
                        }
                };

            var userDto = _mapper.Map<UserDTO>(user);

            Assert.AreEqual("User 1", userDto.Username);
            Assert.AreEqual("Password 1", userDto.Password);
            Assert.AreEqual(123, userDto.Id);
            Assert.AreEqual(2, userDto.Orders.Length);
            Assert.AreEqual(234, userDto.Orders[0].Id);
            Assert.AreEqual(345, userDto.Orders[1].Id);
        }

        [Test]
        public void User_with_orders_mapped_to_exsisting_instance_example()
        {
            var user = new User
                {
                    Username = "User 1",
                    Password = "Password 1",
                    Id = 123,
                    Orders = new List<Order>
                        {
                            new Order { Id = 234 },
                            new Order { Id = 345 }
                        }
                };

            var userDto = new UserDTO();

            _mapper.Map(user, userDto);

            Assert.AreEqual("User 1", userDto.Username);
            Assert.AreEqual("Password 1", userDto.Password);
            Assert.AreEqual(123, userDto.Id);
            Assert.AreEqual(2, userDto.Orders.Length);
            Assert.AreEqual(234, userDto.Orders[0].Id);
            Assert.AreEqual(345, userDto.Orders[1].Id);
        }

        [Test]
        public void User_mapping_ignoring_properties_example()
        {
            var user = new User
                {
                    Username = "User 1",
                    Password = "Password 1",
                    Id = 123,
                    Orders = new List<Order>
                        {
                            new Order { Id = 234 },
                            new Order { Id = 345 }
                        }
                };

            var userDto = _mapper.Map<User, UserDTO>(user, x => x.Ignore(u => u.Password));

            Assert.IsNull(userDto.Password);
        }

        [Test]
        public void Naming_convention_example()
        {
            var ugly = new UglyThirdPartyClass
                {
                    Prop_IntId = 123,
                    Prop_StrName = "name"
                };

            var mapper = new ObjectMapper();
            var nice = mapper.Map<UglyThirdPartyClass, NiceClass>(ugly, x => x.Convention(propertyInfo => propertyInfo.Name.Substring(8)));

            Assert.AreEqual(123, nice.Id);
            Assert.AreEqual("name", nice.Name);
        }
        
        [Test]
        public void Naming_convention_example_with_preconfiguration()
        {
            var ugly = new UglyThirdPartyClass
                {
                    Prop_IntId = 123,
                    Prop_StrName = "name"
                };

            var mapper = new ObjectMapper();

            mapper.ConfigureMap<UglyThirdPartyClass, NiceClass>(x => x.Convention(propertyInfo => propertyInfo.Name.Substring(8)));
            
            var nice = mapper.Map<NiceClass>(ugly);

            Assert.AreEqual(123, nice.Id);
            Assert.AreEqual("name", nice.Name);
        }

        [Test]
        public void Custom_mapping_example_for_fullname()
        {
            var person = new Person
                {
                    Id = 123,
                    FirstName = "Steve",
                    LastName = "Smith"
                };

            var viewModel = _mapper.Map<Person, PersonViewModel>(person, x => x.Custom(p => p.FullName, p => p.FirstName + " " + p.LastName));

            Assert.AreEqual(123, viewModel.Id);
            Assert.AreEqual("Steve Smith", viewModel.FullName);
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

        private class SourceClassWithArray
        {
            public SourceClassWithArray()
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

        private class RootEntity
        {
            public string String { get; set; }
            public ChildEntity[] Children { get; set; }
            public RelatedEntity Related { get; set; }
        }

        private enum SomeEnum
        {
            Value1,
            Value2,
            Value3
        }

        private class RelatedEntity
        {
            public SomeEnum Enum { get; set; }
        }

        private class ChildEntity
        {
            public int Int { get; set; }
            public IEnumerable<GrandChildEntity> GrandChildren { get; set; }
        }

        private class GrandChildEntity
        {
            public double Double { get; set; }
            public RelatedEntity AnotherRelated { get; set; }
        }

        private class RootDTO
        {
            public string String { get; set; }
            public ChildDTO[] Children { get; set; }
            public RelatedDTO Related { get; set; }
        }

        private enum SomeOtherEnum { OtherValue1, OtherValue2, OtherValue3 }

        private class RelatedDTO
        {
            public SomeOtherEnum Enum { get; set; }
        }

        private class ChildDTO
        {
            public int Int { get; set; }
            public IEnumerable<GrandChildDTO> GrandChildren { get; set; }
        }

        private class GrandChildDTO
        {
            public double Double { get; set; }
            public RelatedDTO AnotherRelated { get; set; }
        }

        public class User
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public IList<Order> Orders { get; set; }
        }

        public class Order
        {
            public int Id { get; set; }
            // More properties here ...
        }
        
        public class UserDTO
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public OrderDTO[] Orders { get; set; }
        }

        public class OrderDTO
        {
            public int Id { get; set; }
            // More properties here ...
        }

        public class UglyThirdPartyClass
        {
            public int Prop_IntId { get; set; } 
            public string Prop_StrName { get; set; } 
        }

        public class NiceClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class Person
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class PersonViewModel
        {
            public int Id { get; set; }
            public string FullName { get; set; } 
        }
    }
}