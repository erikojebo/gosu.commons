using Gosu.Commons.Builders;
using NUnit.Framework;

namespace Gosu.Specs.Commons.Builders
{
    [TestFixture]
    public class PropertyValueCollectorSpecs
    {
        private dynamic _collector;

        [SetUp]
        public void SetUp()
        {
            _collector = new PropertyValueCollector();
        }

        [Test]
        public void Stores_properties_and_values_set_on_object()
        {
            _collector.Property1("string")
                .Property2(1.23);

            Assert.AreEqual(2, _collector.PropertyValues.Count);
        }

        [Test]
        public void Overwrites_property_value_if_set_multiple_times()
        {
            _collector.Property(1);
            _collector.Property(2);

            Assert.AreEqual(2, _collector.PropertyValues["Property"]);
        }

        [Test]
        public void Initialize_sets_collected_properties_on_given_object()
        {
            var instance = new SomeClass();

            _collector.Property1("string")
                .Property2(1.23);

            _collector.Initialize(instance);

            Assert.AreEqual("string", instance.Property1);
            Assert.AreEqual(1.23, instance.Property2);
        }

        private class SomeClass
        {
            public string Property1 { get; set; }
            public double Property2 { get; set; }
        }
    }
}