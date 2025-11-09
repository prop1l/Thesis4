using System.Windows;
using ThesisCourse_4.MVVM.ViewModels;
using ThesisCourse_4.MVVM.Views;
using ThesisCourse_4.Services;

namespace ThesisCourse_4
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var themeService = new ThemeService();
            var viewModel = new WelcomeViewModels(themeService);

            var window = new Welcome
            {
                DataContext = viewModel
            };

            window.Show();
        }
    }
}