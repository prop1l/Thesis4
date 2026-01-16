using System.Windows.Input;
using ThesisCourse_4.MVVM.Commands;
using ThesisCourse_4.Services;

namespace ThesisCourse_4.MVVM.ViewModels
{
    public class SmallRegViewModel : ThemedViewModelBase
    {
        private readonly INavigationService _navigationService;
        public ICommand OpenAuthWindowCommand { get; }

        public SmallRegViewModel(IThemeService themeService, INavigationService navigationService) : base(themeService)
        {
            _navigationService = navigationService;

            OpenAuthWindowCommand = new RelayCommand(OnOpenAuthWindow);
        }

        private void OnOpenAuthWindow()
        {
            _navigationService.CloseCurrent();
            _navigationService.ShowWindow<SmallAuthViewModel>();
        }
    }
}
