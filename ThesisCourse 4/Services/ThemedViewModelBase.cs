using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ThesisCourse_4.Services
{
    public abstract class ThemedViewModelBase : INotifyPropertyChanged
    {
        private readonly IThemeService _themeService;
        private bool _isLight = true;

        protected ThemedViewModelBase(IThemeService themeService)
        {
            _themeService = themeService;
            _themeService.SwitchTheme(_isLight); 
        }

        public bool IsLight
        {
            get => _isLight;
            set
            {
                if (_isLight != value)
                {
                    _isLight = value;
                    OnPropertyChanged();
                    _themeService.SwitchTheme(_isLight);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}