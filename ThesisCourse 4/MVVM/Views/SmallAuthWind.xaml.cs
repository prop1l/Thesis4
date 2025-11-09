using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ThesisCourse_4.Services;
using ThesisCourse_4.MVVM.ViewModels;

namespace ThesisCourse_4.MVVM.Views
{
    public partial class SmallAuthWind : Window
    {
        public SmallAuthWind()
        {
            InitializeComponent();
            DataContext = new SmallAuthWindViewModel(new ThemeService());
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void LoginFocus(object sender, RoutedEventArgs e)
        {
            var loginTextBox = sender as TextBox;
            loginTextBox.Foreground = Brushes.Black;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void OnLoginButtonClick(object sender, RoutedEventArgs e)
        {
            string login = Login.Text;
            string password = Password.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            try
            {
                bool isValid = await DatabaseService.ValidateUserAsync(login, password);

                if (isValid) MessageBox.Show("Успешный вход!");
                else MessageBox.Show("Неверный логин или пароль");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
            }
        }
    }
}