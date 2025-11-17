using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ThesisCourse_4.MVVM.Models;

namespace ThesisCourse_4.MVVM.Views
{
    public partial class Welcome : Window
    {

        public Welcome()
        {
            InitializeComponent();
            PreviewMouseDoubleClick += OnPreviewMouseDoubleClick;
            PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
        }


        #region Window Drag & Resize
        private bool IsClickInHeaderButNotButtons(object source)
        {
            var current = source as DependencyObject;
            while (current != null)
            {
                if (current is FrameworkElement fe)
                {
                    if (fe.Name is "MinimizeButton" or "MaximizeButton" or "CloseButton")
                        return false;
                    if (fe.GetType().Name == "Header")
                        return true;
                }
                current = VisualTreeHelper.GetParent(current) as FrameworkElement;
            }
            return false;
        }

        private void OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsClickInHeaderButNotButtons(e.OriginalSource))
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                e.Handled = true;
            }
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsClickInHeaderButNotButtons(e.OriginalSource) && WindowState == WindowState.Normal)
            {
                DragMove();
                e.Handled = true;
            }
        }
        #endregion
    }
}