using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using Gosu.Commons.Dynamics;

namespace Gosu.Wpf.Mvvm
{
    public class DynamicViewModel : HookableDynamicObject, INotifyPropertyChanged
    {
        private DynamicPropertyCollection _properties = new DynamicPropertyCollection();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void CreateProperty<T>(string propertyName)
        {
            _properties.CreateProperty<T>(propertyName);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var propertyName = binder.Name;

            if (!_properties.HasProperty(propertyName))
            {
                return false;
            }

            var property = _properties.GetProperty(propertyName);
            var oldValue = property.GetValue();

            if (oldValue == value)
            {
                return true;
            }

            property.SetValue(value);

            FirePropertyChanged(propertyName);

            return true;
        }


        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            var propertyName = binder.Name;

            if (!_properties.HasProperty(propertyName))
            {
                return false;
            }

            result = _properties.GetPropertyValue(propertyName);

            return true;
        }

        

        protected void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private abstract class DynamicProperty
        {
            protected DynamicProperty(string name)
            {
                Name = name;
            }

            public string Name { get; private set; }

            public abstract object GetValue();
            public abstract void SetValue(object value);
        }

        private class GenericDynamicProperty<T> : DynamicProperty
        {
            private T _propertyValue;

            public GenericDynamicProperty(string name) : base(name) {}

            public override object GetValue()
            {
                return _propertyValue;
            }

            public override void SetValue(object value)
            {
                _propertyValue = (T)Convert.ChangeType(value, typeof(T));
            }
        }

        private class DynamicPropertyCollection
        {
            private readonly IList<DynamicProperty> _properties = new List<DynamicProperty>();

            public void CreateProperty<T>(string propertyName)
            {
                _properties.Add(new GenericDynamicProperty<T>(propertyName));
            }

            public DynamicProperty GetProperty(string propertyName)
            {
                return _properties.First(x => x.Name == propertyName);
            }

            public bool HasProperty(string propertyName)
            {
                return _properties.Any(x => x.Name == propertyName);
            }

            public object GetPropertyValue(string propertyName)
            {
                return GetProperty(propertyName).GetValue();
            }
        }
    }
}