using System.Collections.Generic;
using System.Linq;
using Gosu.Commons.Extensions;
using Gosu.NUnit.Extensions;
using NUnit.Framework;

namespace Gosu.Specs.Commons.Extensions
{
    [TestFixture]
    public class EnumerableExtensionsSpecs
    {
        private List<string> _collection;

        [SetUp]
        public void SetUp()
        {
            _collection = new List<string>();
        }

        [Test]
        public void Empty_collection_is_empty()
        {
            Enumerable.Empty<int>().IsEmpty().ShouldBeTrue();
        }

        [Test]
        public void Collection_with_items_is_not_empty()
        {
            var listSingleElement = new object().AsList();
            listSingleElement.IsEmpty().ShouldBeFalse();
        }

        [Test]
        public void AsList_creates_enumerable_with_object_as_single_element()
        {
            var original = new object();

            var actual = original.AsList();

            actual.ShouldBeEquivalentTo(new[] { original });
        }

        [Test]
        public void ResetTo_adds_items_to_collection()
        {
            var newItems = new[] { "a", "b" };

            _collection.ResetTo(newItems);

            CollectionAssert.AreEquivalent(newItems, _collection);
        }

        [Test]
        public void ResetTo_clears_original_collection()
        {
            _collection.Add("c");
            var newItems = new[] { "a", "b" };

            _collection.ResetTo(newItems);

            CollectionAssert.AreEquivalent(newItems, _collection);
        }
    }
}