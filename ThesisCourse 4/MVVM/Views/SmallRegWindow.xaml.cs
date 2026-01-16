using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ThesisCourse_4.MVVM.Views
{
    public partial class SmallRegWindow : Window
    {
        public SmallRegWindow()
        {
            InitializeComponent();
        }

        #region Window Drag & Resize

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        #endregion

        #region Placeholder Logic

        private void LoginFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox loginTextBox) loginTextBox.Foreground = Brushes.Black;
        }

        private async void OnLoginButtonClickInRegistraton(object sender, RoutedEventArgs e)
        {
            string login = Login.Text;
            string password = Password.Password;
            string passwordAgain = PasswordAgain.Password;

            if (string.IsNullOrEmpty(login) && string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            if (password != passwordAgain)
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }

            var user = await DatabaseService.CreateUserAsync(login, password);

            MessageBox.Show($"{user?.UserName}   ");

            return;
        }

        #endregion
    }
}
