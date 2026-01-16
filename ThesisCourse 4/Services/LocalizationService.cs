// Services/LocalizationService.cs
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

public interface ILocalizationService
{
    string CurrentLanguage { get; }
    void SetLanguage(string languageCode); // "ru", "en"
    void ChangeLangAuto();
}

public class LocalizationService : ILocalizationService
{
    public string CurrentLanguage { get; private set; } = "ru";

    public void SetLanguage(string languageCode)
    {
        if (languageCode != "ru" && languageCode != "en")
            languageCode = "ru";

        CurrentLanguage = languageCode;

        var dict = new ResourceDictionary();
        dict.Source = new Uri(
            languageCode == "ru"
                ? "Resources/Languages/Strings.ru.xaml"
                : "Resources/Languages/Strings.en.xaml",
            UriKind.Relative);

        // удаляем старый словарь строк, добавляем новый
        var oldDict = Application.Current.Resources.MergedDictionaries
            .FirstOrDefault(d => d.Source != null &&
                                 d.Source.OriginalString.Contains("Strings."));
        if (oldDict != null)
            Application.Current.Resources.MergedDictionaries.Remove(oldDict);

        Application.Current.Resources.MergedDictionaries.Add(dict);

        // можно также поменять Culture, если нужно
        CultureInfo.CurrentUICulture = new CultureInfo(languageCode);
    }

    public void ChangeLangAuto()
    {
        var languageCode = "";
        if (CurrentLanguage == "ru")
        {
            languageCode = "en";
            CurrentLanguage = languageCode;
        }
        else
        {
            languageCode = "ru";
            CurrentLanguage = languageCode;
        }

            var dict = new ResourceDictionary();
        dict.Source = new Uri(
            languageCode == "ru"
                ? "Resources/Languages/Strings.ru.xaml"
                : "Resources/Languages/Strings.en.xaml",
            UriKind.Relative);

        // удаляем старый словарь строк, добавляем новый
        var oldDict = Application.Current.Resources.MergedDictionaries
            .FirstOrDefault(d => d.Source != null &&
                                 d.Source.OriginalString.Contains("Strings."));
        if (oldDict != null)
            Application.Current.Resources.MergedDictionaries.Remove(oldDict);

        Application.Current.Resources.MergedDictionaries.Add(dict);

        // можно также поменять Culture, если нужно
        CultureInfo.CurrentUICulture = new CultureInfo(languageCode);
    }
}
