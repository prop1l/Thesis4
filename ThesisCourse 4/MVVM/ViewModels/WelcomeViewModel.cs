using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using ThesisCourse_4.MVVM.Commands;
using ThesisCourse_4.MVVM.Models;
using ThesisCourse_4.Services;

namespace ThesisCourse_4.MVVM.ViewModels
{
    public class WelcomeViewModel : ThemedViewModelBase
    {
        #region Fields

        private readonly IStorageService _storageService;
        private readonly INavigationService _navigationService;
        private readonly ILocalizationService _localizationService;

        private string _graphName = string.Empty;
        private GridState _gridState = new() { RowCount = 3 };

        #endregion

        #region Properties

        public ObservableCollection<ButtonModel> Buttons { get; } = new();

        public string GraphName
        {
            get => _graphName;
            set
            {
                if (SetProperty(ref _graphName, value)) AddGraphCommand?.CanExecute(null);
            }
        }

        public GridState GridState
        {
            get => _gridState;
            private set => SetProperty(ref _gridState, value);
        }

        public ICommand AddGraphCommand { get; }
        public ICommand OpenAuthWindowCommand { get; }
        public ICommand OpenGraphEditorCommand { get; }
        public ICommand DeleteGraphCommand { get; }
        public ICommand RenameGraphCommand { get; }
        public ICommand ChangeLanguageCommand { get; }
        public ICommand ToggleThemeCommand { get; }

        #endregion

        #region Constructor

        public WelcomeViewModel(
            IThemeService themeService,
            IStorageService storageService,
            INavigationService navigationService,
            ILocalizationService localizationService)
            : base(themeService)
        {
            _storageService = storageService;
            _navigationService = navigationService;
            _localizationService = localizationService;

            var saved = _storageService.LoadButtons();
            foreach (var btn in saved) Buttons.Add(btn);

            UpdateGridState();

            AddGraphCommand = new RelayCommand(OnAddGraph, CanAddGraph);
            OpenAuthWindowCommand = new RelayCommand(OnOpenAuthWindow);
            DeleteGraphCommand = new RelayCommand<string>(DeleteGraph);
            RenameGraphCommand = new RelayCommand<string>(RenameGraph);
            OpenGraphEditorCommand = new RelayCommand<ButtonModel>(OpenGraphEditor);
            ChangeLanguageCommand = new RelayCommand(ChangeLanguage);
            ToggleThemeCommand = new RelayCommand(ChangeTheme);
        }


        #endregion

        #region Methods

        private void OnAddGraph()
        {
            if (string.IsNullOrWhiteSpace(GraphName))
                return;

            if (Buttons.Any(b => b.Name.Equals(GraphName.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Такой граф уже существует");
                return;
            }

            int index = Buttons.Count;
            int row = index / 3;
            int col = (index % 3) * 2;

            var btn = new ButtonModel
            {
                Name = GraphName.Trim(),
                Row = row,
                Column = col
            };

            Buttons.Add(btn);
            GraphName = string.Empty;

            UpdateGridState();
            _storageService.SaveButtons(Buttons);
        }

        private void RenameGraph(string oldName)
        {
            var btn = Buttons.FirstOrDefault(x => x.Name == oldName);
            if (btn == null) return;

            var newName = ShowInputDialog("Введите новое имя графа", btn.Name);
            if (string.IsNullOrWhiteSpace(newName) || Buttons.Any(b => b.Name == newName))
                return;

            newName = newName.Trim();

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "ThesisCourse_4");

            string oldPath = Path.Combine(folder, $"{oldName}.json");
            string newPath = Path.Combine(folder, $"{newName}.json");

            try
            {
                if (File.Exists(oldPath) && !File.Exists(newPath))
                {
                    File.Move(oldPath, newPath);
                }

                btn.Name = newName;
                _storageService.SaveButtons(Buttons);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось переименовать файл графа:\n{ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string ShowInputDialog(string message, string defaultValue)
        {
            return Microsoft.VisualBasic.Interaction.InputBox(message, "Переименовать граф", defaultValue);
        }

        private void DeleteGraph(string graphName)
        {
            if (string.IsNullOrEmpty(graphName))
                return;

            var btn = Buttons.FirstOrDefault(x => x.Name == graphName);
            if (btn == null)
                return;

            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string folder = Path.Combine(appData, "ThesisCourse_4");
                string path = Path.Combine(folder, $"{graphName}.json");

                if (File.Exists(path)) File.Delete(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить файл графа:\n{ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Buttons.Remove(btn);

            for (int i = 0; i < Buttons.Count; i++)
            {
                Buttons[i].Row = i / 3;
                Buttons[i].Column = (i % 3) * 2;
            }

            UpdateGridState();
            _storageService.SaveButtons(Buttons);
        }

        private void UpdateGridState()
        {
            int minRows = 3;
            int neededRows = Buttons.Count == 0 ? 1 : ((Buttons.Count - 1) / 3) + 1;
            int newRowCount = Math.Max(minRows, neededRows);

            if (_gridState.RowCount != newRowCount)
            {
                var newState = new GridState { RowCount = newRowCount };
                for (int i = 0; i < newRowCount; i++)
                    newState.RowHeights.Add(new GridRowHeight());
                GridState = newState;
            }
        }

        private void ChangeLanguage() => _localizationService.ChangeLangAuto();
        private void ChangeTheme()
        {
            ThemeService.ToggleTheme();
            IsLight = ThemeService.CurrentTheme == "Light";
        }
        private bool CanAddGraph() => !string.IsNullOrWhiteSpace(GraphName);
        private void OnOpenAuthWindow() => _navigationService.ShowWindow<SmallAuthViewModel>();
        private void OpenGraphEditor(ButtonModel button)
        {
            var vm = _navigationService.ShowWindow<GraphEditorViewModel>(button.Name);
            if (vm != null)
            {
                vm.SetGraphFileName(button.Name);
            }
        }

        #endregion
    }
}
