using System.Collections.Generic;
using Gosu.Commons.Reflection;
using NUnit.Framework;

namespace Gosu.Specs.Commons.Reflection
{
    [TestFixture]
    public class PropertyNameSpecs
    {
        [Test]
        public void Can_get_name_for_first_level_property()
        {
            string actualPropertyName = PropertyName.For<List<int>>(x => x.Count);

            Assert.AreEqual("Count", actualPropertyName);
        }
    }
}