using System.Windows;
using System.Windows.Input;

namespace Gosu.Wpf.Extensions
{
    public static class FrameworkElementExtensions
    {
        public static void InitializeFocusToFirstControl(this FrameworkElement frameworkElement)
        {
            frameworkElement.Loaded +=
                (sender, e) => frameworkElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

    }
}