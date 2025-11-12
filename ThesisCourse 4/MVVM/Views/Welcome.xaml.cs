using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ThesisCourse_4.MVVM.Views
{
    public partial class Welcome : Window
    {

        public Welcome() => InitializeComponent();


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
                WindowState = WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
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

        //#region Scroll Handling
        //private void OnScrollViewerMouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    if (sender is ScrollViewer scrollViewer)
        //    {
        //        const double scrollSpeedFactor = 2.75;
        //        e.Handled = true;
        //        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta * scrollSpeedFactor / 3);
        //    }
        //}
        //#endregion

        //private void AddButton_Click(object sender, RoutedEventArgs e)
        //{
        //    string name = GraphNameTextBox.Text;
        //    if (string.IsNullOrWhiteSpace(name) || name == "Введите название для графа")
        //    {
        //        MessageBox.Show("Вы не ввели название для графа");
        //        return;
        //    }

        //    _viewModel.AddButton(name);

        //    GraphNameTextBox.Text = "";
        //    GraphNameTextBox.Foreground = Brushes.Gray;
        //    GraphNameTextBox.Tag = "Введите название для графа";
        //}

        //protected override void OnClosed(EventArgs e)
        //{
        //    _viewModel.SaveButtons();
        //    base.OnClosed(e);
        //}
    }
}