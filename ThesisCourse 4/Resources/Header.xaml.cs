using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ThesisCourse_4.Resources
{
    public partial class Header : UserControl
    {
        public Header() => InitializeComponent();

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e){}

        private void TitleBar_MouseDoubleClick(object sender, MouseButtonEventArgs e){}

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => Window.GetWindow(this)?.WindowState = WindowState.Minimized;

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Window.GetWindow(this)?.Close();

    }
}