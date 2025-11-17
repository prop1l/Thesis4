using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using ThesisCourse_4.MVVM.ViewModels;
using ThesisCourse_4.MVVM.Views;

namespace ThesisCourse_4.Services
{
    public interface INavigationService
    {
        TViewModel ShowWindow<TViewModel>() where TViewModel : class;
        TViewModel ShowWindow<TViewModel>(object parameter) where TViewModel : class;
        void CloseCurrent();
    }

    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TViewModel ShowWindow<TViewModel>() where TViewModel : class
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
            return (TViewModel)viewModel;
        }

        public TViewModel ShowWindow<TViewModel>(object parameter) where TViewModel : class
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

                if (viewModel is GraphEditorViewModel graphVm)
                    graphVm.SetGraphFileName(title);
            }

            window.Show();
            return (TViewModel)viewModel;
        }

        public void CloseCurrent()
        {
            if (Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive) is { } current)
                current.Close();
        }
    }
}
