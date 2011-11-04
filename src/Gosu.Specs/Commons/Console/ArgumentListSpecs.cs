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
    }
}