using System;
using System.ComponentModel;
using Gosu.NUnit.Extensions;
using NUnit.Framework;

namespace Gosu.Specs.NUnit
{
    [TestFixture]
    public class PropertyChangedAssertionSpecs
    {
        private ChangeNotifyingClass _instance;

        [SetUp]
        public void SetUp()
        {
            _instance = new ChangeNotifyingClass();
        }

        [Test]
        public void Correctly_asserts_property_changed_for_bool_property()
        {
            AssertPropertyChangedIsAssertedCorrectlyFor("BoolProperty");
        }

        [Test]
        public void Correctly_asserts_property_changed_for_int_property()
        {
            AssertPropertyChangedIsAssertedCorrectlyFor("IntProperty");
        }

        [Test]
        public void Correctly_asserts_property_changed_for_double_property()
        {
            AssertPropertyChangedIsAssertedCorrectlyFor("DoubleProperty");
        }

        [Test]
        public void Correctly_asserts_property_changed_for_short_property()
        {
            AssertPropertyChangedIsAssertedCorrectlyFor("ShortProperty");
        }

        [Test]
        public void Correctly_asserts_property_changed_for_long_property()
        {
            AssertPropertyChangedIsAssertedCorrectlyFor("LongProperty");
        }

        [Test]
        public void Correctly_asserts_property_changed_for_decimal_property()
        {
            AssertPropertyChangedIsAssertedCorrectlyFor("DecimalProperty");
        }

        [Test]
        public void Correctly_asserts_property_changed_for_float_property()
        {
            AssertPropertyChangedIsAssertedCorrectlyFor("FloatProperty");
        }

        [Test]
        public void Correctly_asserts_property_changed_for_date_time_property()
        {
            AssertPropertyChangedIsAssertedCorrectlyFor("DateTimeProperty");
        }

        [Test]
        public void Correctly_asserts_property_changed_for_time_span_property()
        {
            AssertPropertyChangedIsAssertedCorrectlyFor("TimeSpanProperty");
        }

        [Test]
        public void Correctly_asserts_property_changed_for_nullable_int_property()
        {
            AssertPropertyChangedIsAssertedCorrectlyFor("NullableIntProperty");
        }

        [Test]
        public void Correctly_asserts_property_changed_for_nullable_double_property()
        {
            AssertPropertyChangedIsAssertedCorrectlyFor("NullableDoubleProperty");
        }

        [Test]
        public void Correctly_asserts_property_changed_for_nullable_date_time_property()
        {
            AssertPropertyChangedIsAssertedCorrectlyFor("NullableDateTimeProperty");
        }

        [Test]
        public void Correctly_asserts_property_changed_for_nullable_time_span_property()
        {
            AssertPropertyChangedIsAssertedCorrectlyFor("NullableTimeSpanProperty");
        }

        [Test]
        public void Correctly_asserts_property_changed_for_nullable_bool_property()
        {
            AssertPropertyChangedIsAssertedCorrectlyFor("NullableBoolProperty");
        }

        [Test]
        public void Correctly_asserts_property_changed_for_custom_property()
        {
            _instance.IsPropertyChangedEnabled = true;

            _instance.ShouldFirePropertyChangedFor(
                "CustomProperty",
                x => x.CustomProperty = new CustomClass(),
                x => x.CustomProperty = new CustomClass());

            _instance.IsPropertyChangedEnabled = false;

            Assert.Throws<AssertionException>(
                () => _instance.ShouldFirePropertyChangedFor(
                    "CustomProperty",
                    x => x.CustomProperty = new CustomClass(),
                    x => x.CustomProperty = new CustomClass()));
        }

        private void AssertPropertyChangedIsAssertedCorrectlyFor(string propertyName)
        {
            _instance.IsPropertyChangedEnabled = true;

            _instance.ShouldFirePropertyChangedFor(propertyName);

            _instance.IsPropertyChangedEnabled = false;

            Assert.Throws<AssertionException>(() => _instance.ShouldFirePropertyChangedFor(propertyName));
        }

        private class ChangeNotifyingClass : INotifyPropertyChanged
        {
            public bool IsPropertyChangedEnabled = true;

            private long _longProperty;
            private decimal _decimalProperty;
            private DateTime _dateTimeProperty;
            private Guid _guidProperty;
            private TimeSpan _timeSpanProperty;
            private bool _boolProperty;
            private int _intProperty;
            private double _doubleProperty;
            private short _shortProperty;
            private CustomClass _customProperty;
            private float _floatProperty;

            public bool BoolProperty
            {
                get { return _boolProperty; }
                set
                {
                    _boolProperty = value;
                    FirePropertyChanged("BoolProperty");
                }
            }

            public int IntProperty
            {
                get { return _intProperty; }
                set
                {
                    _intProperty = value;
                    FirePropertyChanged("IntProperty");
                }
            }
            public double DoubleProperty
            {
                get { return _doubleProperty; }
                set
                {
                    _doubleProperty = value;
                    FirePropertyChanged("DoubleProperty");
                }
            }

            public float FloatProperty
            {
                get { return _floatProperty; }
                set
                {
                    _floatProperty = value;
                    FirePropertyChanged("FloatProperty");
                }
            }

            public short ShortProperty
            {
                get { return _shortProperty; }
                set
                {
                    _shortProperty = value;
                    FirePropertyChanged("ShortProperty");
                }
            }
            public long LongProperty
            {
                get { return _longProperty; }
                set
                {
                    _longProperty = value;
                    FirePropertyChanged("LongProperty");
                }
            }
            public decimal DecimalProperty
            {
                get { return _decimalProperty; }
                set
                {
                    _decimalProperty = value;
                    FirePropertyChanged("DecimalProperty");
                }
            }
            public DateTime DateTimeProperty
            {
                get { return _dateTimeProperty; }
                set
                {
                    _dateTimeProperty = value;
                    FirePropertyChanged("DateTimeProperty");
                }
            }

            public TimeSpan TimeSpanProperty
            {
                get { return _timeSpanProperty; }
                set
                {
                    _timeSpanProperty = value;
                    FirePropertyChanged("TimeSpanProperty");
                }
            }
            public Guid GuidProperty
            {
                get { return _guidProperty; }
                set
                {
                    _guidProperty = value;
                    FirePropertyChanged("GuidProperty");
                }
            }

            private int _nullableIntProperty;
            public int NullableIntProperty
            {
                get { return _nullableIntProperty; }
                set
                {
                    _nullableIntProperty = value;
                    FirePropertyChanged("NullableIntProperty");
                }
            }

            private double? _nullableDoubleProperty;
            public double? NullableDoubleProperty
            {
                get { return _nullableDoubleProperty; }
                set
                {
                    _nullableDoubleProperty = value;
                    FirePropertyChanged("NullableDoubleProperty");
                }
            }

            private bool? _nullableBoolProperty;
            public bool? NullableBoolProperty
            {
                get { return _nullableBoolProperty; }
                set
                {
                    _nullableBoolProperty = value;
                    FirePropertyChanged("NullableBoolProperty");
                }
            }

            private TimeSpan? _nullableTimeSpanProperty;
            public TimeSpan? NullableTimeSpanProperty
            {
                get { return _nullableTimeSpanProperty; }
                set
                {
                    _nullableTimeSpanProperty = value;
                    FirePropertyChanged("NullableTimeSpanProperty");
                }
            }

            private DateTime? _nullableDateTimeProperty;
            public DateTime? NullableDateTimeProperty
            {
                get { return _nullableDateTimeProperty; }
                set
                {
                    _nullableDateTimeProperty = value;
                    FirePropertyChanged("NullableDateTimeProperty");
                }
            }

            public CustomClass CustomProperty
            {
                get { return _customProperty; }
                set
                {
                    _customProperty = value;
                    FirePropertyChanged("CustomProperty");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public void FirePropertyChanged(string propertyName)
            {
                if (PropertyChanged != null && IsPropertyChangedEnabled)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        private class CustomClass {}
    }
}