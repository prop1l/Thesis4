using System.Collections.ObjectModel;
using System.Windows.Input;
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

        #endregion

        #region Constructor

        public WelcomeViewModel(IThemeService themeService, IStorageService storageService, INavigationService navigationService) : base(themeService)
        {
            _storageService = storageService;
            _navigationService = navigationService;

            var saved = _storageService.LoadButtons();
            foreach (var btn in saved) Buttons.Add(btn);

            UpdateGridState();

            AddGraphCommand = new RelayCommand(OnAddGraph, CanAddGraph);
            OpenAuthWindowCommand = new RelayCommand(OnOpenAuthWindow);
            OpenGraphEditorCommand = new RelayCommand<ButtonModel>(OpenGraphEditor);
        }


        #endregion

        #region Methods

        private void OnAddGraph()
        {
            if (string.IsNullOrWhiteSpace(GraphName)) return;

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

        private void UpdateGridState()
        {
            int minRows = 3;
            int neededRows = Buttons.Count == 0 ? 1 : (Buttons.Count - 1) / 3 + 1;
            int newRowCount = Math.Max(minRows, neededRows);

            if (_gridState.RowCount != newRowCount)
            {
                var newState = new GridState { RowCount = newRowCount };
                for (int i = 0; i < newRowCount; i++) newState.RowHeights.Add(new GridRowHeight());
                GridState = newState;
            }
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
