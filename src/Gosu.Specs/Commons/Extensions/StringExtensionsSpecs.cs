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
    }
}