using System.Collections.ObjectModel;
using System.IO;
using ThesisCourse_4.MVVM.Models;
using ThesisCourse_4.Services;

namespace ThesisCourse_4.MVVM.ViewModels
{
    public class WelcomeViewModels : ThemedViewModelBase
    {
        private const string ButtonsFilePath = "buttons.txt";

        public WelcomeViewModels(IThemeService themeService) : base(themeService){}

        public ObservableCollection<CustomButton> Buttons { get; } = new();

        public void Initialize() => LoadButtons();

        public void AddButton(string name)
        {
            string buttonName = string.IsNullOrWhiteSpace(name) ? "Default" : name;
            int index = Buttons.Count;

            var newButton = new CustomButton
            {
                Name = buttonName,
                Row = index / 3,
                Column = (index % 3) * 2
            };

            Buttons.Add(newButton);
            SaveButtons();
        }

        private void LoadButtons()
        {
            if (!File.Exists(ButtonsFilePath)) return;

            Buttons.Clear();
            try
            {
                foreach (var line in File.ReadLines(ButtonsFilePath))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        Buttons.Add(CustomButton.FromString(line));
                    }
                }
            }
            catch { }
        }

        public void SaveButtons()
        {
            try
            {
                File.WriteAllLines(ButtonsFilePath, Buttons.Select(b => b.ToString()));
            }
            catch { }
        }
    }
}