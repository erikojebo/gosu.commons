using System.ComponentModel;
using Gosu.Commons.Dynamics;

namespace Gosu.Wpf.Mvvm
{
    public class DynamicViewModel : HookableDynamicObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}