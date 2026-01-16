using System.Windows.Input;
using ThesisCourse_4.MVVM.Commands;
using ThesisCourse_4.Services;

namespace ThesisCourse_4.MVVM.ViewModels
{
    public class SmallAuthViewModel : ThemedViewModelBase
    {
        private readonly ILocalizationService _localizationService;
        private readonly INavigationService _navigationService;

        public ICommand OpenRegWindowCommand { get; }

        public SmallAuthViewModel(IThemeService themeService, ILocalizationService localizationService, INavigationService navigationService) : base(themeService)
        {
            _localizationService = localizationService;
            _navigationService = navigationService;
            OpenRegWindowCommand = new RelayCommand(OnOpenRegWindow);
            ChangeLanguageCommand = new RelayCommand<string>(ChangeLanguage);
        }
        public ICommand ChangeLanguageCommand { get; }

        private void ChangeLanguage(string lang) => _localizationService.SetLanguage(lang);

        private void OnOpenRegWindow()
        {
            _navigationService.CloseCurrent();
            _navigationService.ShowWindow<SmallRegViewModel>();
        }
    }
}