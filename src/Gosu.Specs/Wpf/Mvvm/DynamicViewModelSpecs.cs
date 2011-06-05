using System.Collections.Generic;
using System.Dynamic;
using Gosu.NUnit.Extensions;
using Gosu.Wpf.Mvvm;
using Microsoft.CSharp.RuntimeBinder;
using NUnit.Framework;

namespace Gosu.Specs.Wpf.Mvvm
{
    [TestFixture]
    public class DynamicViewModelSpecs
    {
        private dynamic _viewModel;
        private TestDynamicViewModel _typedViewModel;

        [SetUp]
        public void SetUp()
        {
            _viewModel = new TestDynamicViewModel();
            _typedViewModel = _viewModel;
        }

        [Test]
        public void Property_can_be_written_and_read_back_again()
        {
            _viewModel.Property = "expected value";
            Assert.AreEqual("expected value", _viewModel.Property);
        }

        [Test]
        public void Multiple_properties_can_be_written_and_read_back_again_simultaneously()
        {
            _viewModel.Property = "expected value";
            _viewModel.Property2 = "expected value 2";

            Assert.AreEqual("expected value", _viewModel.Property);
            Assert.AreEqual("expected value 2", _viewModel.Property2);
        }

        [Test]
        public void Getting_property_that_does_not_exist_throws_dynamic_invocation_exception()
        {
            Assert.Throws<RuntimeBinderException>(() =>
                {
                    var unused = _viewModel.NonExistingProperty;
                });
        }

        [Test]
        public void Setting_property_that_does_not_exist_throws_dynamic_invocation_exception()
        {
            Assert.Throws<RuntimeBinderException>(() => _viewModel.NonExistingProperty = "value");
        }

        [Test]
        public void Setting_property_to_new_value_triggers_change_notification()
        {
            _typedViewModel.ShouldFirePropertyChangedFor(
                "Property",
                x => { ((dynamic)x).Property = "old value"; },
                x => { ((dynamic)x).Property = "new value"; });
        }

        private class TestDynamicViewModel : DynamicViewModel
        {
            private readonly IDictionary<string, object> _value = new Dictionary<string, object>();

            public TestDynamicViewModel()
            {
                CreateProperty("Property");
                CreateProperty("Property2");
            }

            private void CreateProperty(string propertyName)
            {
                _value.Add(propertyName, null);
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                var propertyName = binder.Name;

                if (!HasProperty(propertyName))
                {
                    return false;
                }

                _value[binder.Name] = value;
                FirePropertyChanged(propertyName);
                
                return true;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = null;

                var propertyName = binder.Name;

                if (!HasProperty(propertyName))
                {
                    return false;
                }

                result = _value[propertyName];

                return true;
            }

            private bool HasProperty(string propertyName)
            {
                return _value.ContainsKey(propertyName);
            }
        }
    }
}