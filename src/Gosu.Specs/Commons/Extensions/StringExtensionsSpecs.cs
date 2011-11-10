using Gosu.Commons.Extensions;
using NUnit.Framework;

namespace Gosu.Specs.Commons.Extensions
{
    [TestFixture]
    public class StringExtensionsSpecs
    {
        [Test]
        public void IsNullOrEmpty_is_true_for_null()
        {
            string value = null;

            Assert.IsTrue(value.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_is_true_for_empty_string()
        {
            string value = "";

            Assert.IsTrue(value.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_is_false_for_non_empty_string()
        {
            string value = "value";

            Assert.IsFalse(value.IsNullOrEmpty());
        }

        [Test]
        public void TrimEnd_leaves_non_matching_string_intact()
        {
            Assert.AreEqual("some string", "some string".TrimEnd("non match"));
        }

        [Test]
        public void TrimEnd_removes_suffix_for_matching_string()
        {
            Assert.AreEqual("some st", "some string".TrimEnd("ring"));
        }
    }
}