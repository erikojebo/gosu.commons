using Gosu.Commons.Console;
using NUnit.Framework;

namespace Gosu.Specs.Commons.Console
{
    [TestFixture]
    public class ArgumentListSpecs
    {
        [Test]
        public void Empty_argument_list_has_no_values()
        {
            var arguments = new ArgumentList();
            Assert.IsFalse(arguments.HasValues);
        }

        [Test]
        public void Argument_list_with_one_value_has_values()
        {
            var arguments = new ArgumentList("value");
            Assert.IsTrue(arguments.HasValues);
        }

        [Test]
        public void Simple_strings_in_argument_list_are_considered_values()
        {
            var arguments = new ArgumentList("value1", "value2");

            Assert.AreEqual("value1", arguments.Values[0]);
            Assert.AreEqual("value2", arguments.Values[1]);
        }

        [Test]
        public void Flag_is_not_considered_value()
        {
            var arguments = new ArgumentList("-flag");

            Assert.IsFalse(arguments.HasValues);
        }

        [ExpectedException]
        [Test]
        public void Accessing_non_existing_value_yields_exception()
        {
            var arguments = new ArgumentList("value1");

            var unused = arguments.Values[1];
        }

        [Test]
        public void Strings_after_flag_are_not_considered_values()
        {
            var arguments = new ArgumentList("-flag", "string1", "string2");

            Assert.IsFalse(arguments.HasValues);
        }

        [Test]
        public void Strings_before_flag_are_considred_values()
        {
            var arguments = new ArgumentList("value", "-flag", "string");

            Assert.AreEqual(1, arguments.Values.Count);
            Assert.AreEqual("value", arguments.Values[0]);
        }

        [Test]
        public void String_beginning_with_dash_is_a_flag()
        {
            var arguments = new ArgumentList("-flag");

            var flag = arguments.GetFlag("flag");

            Assert.IsTrue(flag.HasValue);
            Assert.AreEqual("flag", flag.Value.Name);
        }

        [Test]
        public void Multiple_strings_beginning_with_dash_are_considered_multiple_flags()
        {
            var arguments = new ArgumentList("-flag1", "-flag2");

            var flag1 = arguments.GetFlag("flag1");
            var flag2 = arguments.GetFlag("flag2");

            Assert.IsTrue(flag1.HasValue);
            Assert.IsTrue(flag2.HasValue);
            Assert.AreEqual("flag1", flag1.Value.Name);
            Assert.AreEqual("flag2", flag2.Value.Name);
        }

        [Test]
        public void Getting_non_existing_flag_yeilds_nothing()
        {
            var arguments = new ArgumentList("-flag");

            Assert.IsTrue(arguments.GetFlag("nonexisting").IsNothing);
        }

        [Test]
        public void Strings_following_flag_are_values_of_that_flag()
        {
            var arguments = new ArgumentList("-flag", "value1", "value2");

            var flag = arguments.GetFlag("flag").Value;

            Assert.IsTrue(flag.HasValues);
            Assert.AreEqual(2, flag.Values.Count);
        }

        [Test]
        public void Flag_without_following_values_has_no_values()
        {
            var arguments = new ArgumentList("-flag");

            var flag = arguments.GetFlag("flag").Value;

            Assert.IsFalse(flag.HasValues);
            Assert.AreEqual(0, flag.Values.Count);
        }

        [Test]
        public void Flag_followed_by_flag_has_no_values()
        {
            var arguments = new ArgumentList("-flag1", "-flag2");

            var flag = arguments.GetFlag("flag1").Value;

            Assert.IsFalse(flag.HasValues);
        }

        [Test]
        public void Multiple_flags_with_values_can_appear_after_each_other()
        {
            var arguments = new ArgumentList("-flag1", "value1", "-flag2", "value2", "value3");

            var flag1 = arguments.GetFlag("flag1").Value;
            var flag2 = arguments.GetFlag("flag2").Value;

            Assert.AreEqual(1, flag1.Values.Count);
            Assert.AreEqual(2, flag2.Values.Count);

            Assert.AreEqual("value1", flag1.Values[0]);
            Assert.AreEqual("value2", flag2.Values[0]);
            Assert.AreEqual("value3", flag2.Values[1]);
        }

        [Test]
        public void Retrieving_flag_value_directly_returns_nothing_for_missing_flag()
        {
            var arguments = new ArgumentList();

            var value = arguments.GetFlagValue("missingflag");

            Assert.IsTrue(value.IsNothing);
        }

        [Test]
        public void Retrieving_flag_value_directly_returns_first_flag_value()
        {
            var arguments = new ArgumentList("-flag", "value1", "value2");

            var flagValue = arguments.GetFlagValue("flag");

            Assert.AreEqual("value1", flagValue.Value);
        }

        [Test]
        public void Retrieving_flag_value_directly_returns_nothing_for_flag_without_values()
        {
            var arguments = new ArgumentList("-flag");

            var flagValue = arguments.GetFlagValue("flag");

            Assert.IsTrue(flagValue.IsNothing);
        }

        [Test]
        public void Retrieving_flag_value_with_default_value_returns_first_flag_value()
        {
            var arguments = new ArgumentList("-flag", "value1", "value2");

            var value = arguments.GetFlagValueOrDefault("flag", "default");

            Assert.AreEqual("value1", value);
        }

        [Test]
        public void Retrieving_flag_value_with_default_returns_default_for_flag_without_values()
        {
            var arguments = new ArgumentList("-flag");

            var value = arguments.GetFlagValueOrDefault("flag", "default");

            Assert.AreEqual("default", value);
        }

        [Test]
        public void Retrieving_flag_value_with_default_returns_default_for_missing_flag()
        {
            var arguments = new ArgumentList();

            var value = arguments.GetFlagValueOrDefault("flag", "default");

            Assert.AreEqual("default", value);
        }

        [Test]
        public void Multiple_flag_names_can_be_used_when_fetching_flag_value()
        {
            var arguments1 = new ArgumentList("-flag", "value");
            var arguments2 = new ArgumentList("-f", "value");

            var value1 = arguments1.GetFlagValue("flag", "f");
            var value2 = arguments2.GetFlagValue("flag", "f");

            Assert.IsTrue(value1.HasValue);
            Assert.IsTrue(value2.HasValue);
            Assert.AreEqual("value", value1.Value);
            Assert.AreEqual("value", value2.Value);
        }

        [Test]
        public void Flag_value_is_nothing_when_fetching_value_for_missing_flag_aliases()
        {
            var arguments = new ArgumentList();

            Assert.IsTrue(arguments.GetFlagValue("flag", "f").IsNothing);
        }
    }
}