using System.Windows;
using System.Windows.Controls;

namespace ThesisCourse_4.Resources
{
    public partial class CloseButton : UserControl
    {
        public CloseButton() => InitializeComponent();

        private void CloseClick(object sender, RoutedEventArgs e) => Window.GetWindow(this).Close();
    }
}
