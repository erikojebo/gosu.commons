using System;
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
            Assert.Throws<RuntimeBinderException>(() => { var unused = _viewModel.NonExistingProperty; });
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

        [Test]
        public void Setting_property_to_same_value_does_not_trigger_change_notification()
        {
            Assert.Throws<AssertionException>(
                () => _typedViewModel.ShouldFirePropertyChangedFor(
                    "Property",
                    x => { ((dynamic)x).Property = "value"; },
                    x => { ((dynamic)x).Property = "value"; }));
        }

        [Test]
        public void Reading_int_property_set_to_string_yields_converted_value()
        {
            _viewModel.IntProperty = "1";

            Assert.AreEqual(1, _viewModel.IntProperty);
        }

        private class TestDynamicViewModel : DynamicViewModel
        {
            public TestDynamicViewModel()
            {
                CreateProperty<string>("Property");
                CreateProperty<string>("Property2");
                CreateProperty<int>("IntProperty");
            }
        }
    }
}