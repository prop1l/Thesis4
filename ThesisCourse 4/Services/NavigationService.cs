using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using ThesisCourse_4.MVVM.ViewModels;
using ThesisCourse_4.MVVM.Views;

namespace ThesisCourse_4.Services
{
    public interface INavigationService
    {
        void ShowWindow<TViewModel>() where TViewModel : class;
        void ShowWindow<TViewModel>(object parameter) where TViewModel : class;

        void CloseCurrent();
    }

    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void ShowWindow<TViewModel>() where TViewModel : class
        {
            var vmType = typeof(TViewModel);
            Type viewType = vmType switch
            {
                _ when vmType == typeof(WelcomeViewModel) => typeof(Welcome),
                _ when vmType == typeof(SmallAuthViewModel) => typeof(SmallAuthWind),
                _ => throw new ArgumentException($"Не найдено окно для {vmType.Name}")
            };

            var window = (Window)ActivatorUtilities.CreateInstance(_serviceProvider, viewType);
            var viewModel = _serviceProvider.GetRequiredService(vmType);
            window.DataContext = viewModel;
            window.Show();
        }

        public void ShowWindow<TViewModel>(object parameter) where TViewModel : class
        {
            var vmType = typeof(TViewModel);
            Type viewType = vmType switch
            {
                _ when vmType == typeof(WelcomeViewModel) => typeof(Welcome),
                _ when vmType == typeof(SmallAuthViewModel) => typeof(SmallAuthWind),
                _ when vmType == typeof(GraphEditorViewModel) => typeof(GraphEditorWindow),
                _ => throw new ArgumentException($"Не найдено окно для {vmType.Name}")
            };

            var window = (Window)ActivatorUtilities.CreateInstance(_serviceProvider, viewType);
            var viewModel = _serviceProvider.GetRequiredService(vmType);

            window.DataContext = viewModel;

            if (parameter is string title && !string.IsNullOrWhiteSpace(title))
            {
                window.Title = title;
            }

            window.Show();
        }


        public void CloseCurrent()
        {
            if (Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive) is { } current)
                current.Close();
        }
    }
}

