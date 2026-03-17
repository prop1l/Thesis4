using System.Windows;

public interface IThemeService
{
    string CurrentTheme { get; }
    void SetTheme(string themeName);
    void ToggleTheme();
}

public class ThemeService : IThemeService
{
    public string CurrentTheme { get; private set; } = "Light";

    public void SetTheme(string themeName)
    {
        if (themeName != "Light" && themeName != "Dark")
            themeName = "Light";

        CurrentTheme = themeName;

        var dict = new ResourceDictionary
        {
            Source = new Uri(
                themeName == "Light"
                    ? "Resources/Themes/LightTheme.xaml"
                    : "Resources/Themes/DarkTheme.xaml",
                UriKind.Relative)
        };

        var oldDict = Application.Current.Resources.MergedDictionaries
            .FirstOrDefault(d => d.Source != null &&
                                 d.Source.OriginalString.Contains("Theme."));
        if (oldDict != null)
            Application.Current.Resources.MergedDictionaries.Remove(oldDict);

        Application.Current.Resources.MergedDictionaries.Add(dict);
    }

    public void ToggleTheme()
    {
        var themeName = CurrentTheme == "Light" ? "Dark" : "Light";
        SetTheme(themeName);
    }
}