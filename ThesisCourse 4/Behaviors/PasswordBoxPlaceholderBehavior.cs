using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace ThesisCourse_4.Behaviors
{
    public class PasswordBoxPlaceholderBehavior : Behavior<PasswordBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var placeholder = FindVisualChild<TextBlock>(AssociatedObject, "Placeholder");
            if (placeholder == null) return;

            UpdatePlaceholderVisibility(placeholder);

            AssociatedObject.PasswordChanged += (s, _) => UpdatePlaceholderVisibility(placeholder);
            AssociatedObject.GotFocus += (s, _) => placeholder.Visibility = Visibility.Collapsed;
            AssociatedObject.LostFocus += (s, _) => UpdatePlaceholderVisibility(placeholder);
        }

        private void UpdatePlaceholderVisibility(TextBlock placeholder)
        {
            placeholder.Visibility = string.IsNullOrEmpty(AssociatedObject.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnLoaded;
            base.OnDetaching();
        }

        private static T FindVisualChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t && (string.IsNullOrEmpty(childName) || (child is FrameworkElement fe && fe.Name == childName)))
                    return t;
                var childOfChild = FindVisualChild<T>(child, childName);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }
    }
}