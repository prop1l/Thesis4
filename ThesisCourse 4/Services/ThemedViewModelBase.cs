using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ThesisCourse_4.MVVM.ViewModels
{
    public abstract class ThemedViewModelBase : INotifyPropertyChanged
    {
        protected IThemeService ThemeService { get; }

        protected ThemedViewModelBase(IThemeService themeService)
        {
            ThemeService = themeService;
            _isLight = ThemeService.CurrentTheme == "Light";
        }

        private bool _isLight;
        public bool IsLight
        {
            get => _isLight;
            set
            {
                if (SetProperty(ref _isLight, value))
                {
                    ThemeService.SetTheme(value ? "Light" : "Dark");
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