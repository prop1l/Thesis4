using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using ThesisCourse_4.MVVM.ViewModels;
using ThesisCourse_4.MVVM.Views;
using ThesisCourse_4.Services;

namespace ThesisCourse_4
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            // Сервисы
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<IStorageService, FileStorageService>();
            services.AddSingleton<INavigationService>(sp => new NavigationService(sp));

            // ViewModels
            services.AddTransient<WelcomeViewModel>();
            services.AddTransient<SmallAuthViewModel>();

            // Views (если понадобится в NavigationService)
            services.AddTransient<Welcome>();
            services.AddTransient<SmallAuthWind>();

            _serviceProvider = services.BuildServiceProvider();

            // Запуск
            var mainWindow = _serviceProvider.GetRequiredService<Welcome>();
            mainWindow.DataContext = _serviceProvider.GetRequiredService<WelcomeViewModel>();
            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}