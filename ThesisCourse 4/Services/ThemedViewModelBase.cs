using System.ComponentModel;
using System.Runtime.CompilerServices;
using ThesisCourse_4.Services;

namespace ThesisCourse_4.MVVM.ViewModels
{
    public abstract class ThemedViewModelBase : INotifyPropertyChanged
    {
        private readonly IThemeService _themeService;

        public ThemedViewModelBase(IThemeService themeService)
        {
            _themeService = themeService;
            _isLight = _themeService.IsLight;
        }

        private bool _isLight;
        public bool IsLight
        {
            get => _isLight;
            set
            {
                if (SetProperty(ref _isLight, value))
                {
                    _themeService.IsLight = value;
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}