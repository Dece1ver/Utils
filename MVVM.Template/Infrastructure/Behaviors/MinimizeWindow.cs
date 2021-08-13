using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace MVVM.Template.Infrastructure.Behaviors
{
    internal class MinimizeWindow : Behavior<Button>
    {
        protected override void OnAttached() => AssociatedObject.Click += OnButtonClick;

        protected override void OnDetaching() => AssociatedObject.Click -= OnButtonClick;

        private void OnButtonClick(object Sender, RoutedEventArgs E)
        {
            var window = AssociatedObject.FindVisualRoot() as Window;
            if (window is null) return;

            window.WindowState = WindowState.Minimized;
        }
    }
}
