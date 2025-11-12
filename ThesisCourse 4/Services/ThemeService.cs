using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ThesisCourse_4.Services
{
    public interface IThemeService : INotifyPropertyChanged
    {
        bool IsLight { get; set; }
    }

    public class ThemeService : IThemeService
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isLight = true;
        public bool IsLight
        {
            get => _isLight;
            set
            {
                if (SetProperty(ref _isLight, value))
                {
                    ApplyTheme(value);
                }
            }
        }

        public ThemeService()
        {
            ApplyTheme(_isLight);
        }

        private void ApplyTheme(bool isLight)
        {
            var path = isLight
                ? "Resources/Themes/LightTheme.xaml"
                : "Resources/Themes/DarkTheme.xaml";

            var theme = (ResourceDictionary)Application.LoadComponent(new Uri(path, UriKind.Relative));
            var dicts = Application.Current.Resources.MergedDictionaries;

            if (dicts.Count > 0)
                dicts[0] = theme;
            else
                dicts.Add(theme);
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}