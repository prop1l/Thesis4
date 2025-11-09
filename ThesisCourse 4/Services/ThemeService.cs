using System.Windows;

namespace ThesisCourse_4.Services
{
    public class ThemeService : IThemeService
    {
        public void SwitchTheme(bool isLight)
        {
            var path = isLight ? "Resources/Themes/LightTheme.xaml" : "Resources/Themes/DarkTheme.xaml";
            var theme = (ResourceDictionary)Application.LoadComponent(new Uri(path, UriKind.Relative));
            var dicts = Application.Current.Resources.MergedDictionaries;

            if (dicts.Count > 0)
                dicts[0] = theme; 
            else
                dicts.Add(theme);
        }
    }
}