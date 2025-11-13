using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.IO;
using System.Windows.Input;
using ThesisCourse_4.MVVM.Models;
using ThesisCourse_4.MVVM.Commands;
using ThesisCourse_4.Services;

namespace ThesisCourse_4.MVVM.ViewModels
{
    public class GraphEditorViewModel : ThemedViewModelBase
    {
        private int nodeCounter = 1;

        public ObservableCollection<Node> Nodes { get; } = new();

        public ICommand AddNodeCommand { get; }
        public ICommand SaveCommand { get; }

        public GraphEditorViewModel(IThemeService themeService) : base(themeService)
        {
            AddNodeCommand = new RelayCommand(AddNode);
            SaveCommand = new RelayCommand(SaveGraph);
        }

        private void AddNode()
        {
            var offset = 5;
            var node = new Node
            {
                Id = nodeCounter++,
                Label = $"Node {nodeCounter}",
                X = 100 + (nodeCounter - 1) * offset,
                Y = 100 + (nodeCounter - 1) * offset
            };
            Nodes.Add(node);
        }


        private void SaveGraph()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(Nodes, options);
            File.WriteAllText("graph.json", json);
        }
    }
}
