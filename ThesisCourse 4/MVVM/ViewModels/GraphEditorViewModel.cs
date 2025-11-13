using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using ThesisCourse_4.MVVM.Models;
using ThesisCourse_4.MVVM.Commands;

namespace ThesisCourse_4.MVVM.ViewModels
{
    public class GraphEditorViewModel : INotifyPropertyChanged
    {
        private int nodeCounter = 1;

        public ObservableCollection<Node> Nodes { get; } = new();
        public ObservableCollection<Edge> Edges { get; } = new();

        public ICommand AddNodeCommand { get; }
        public ICommand SaveCommand { get; }

        public GraphEditorViewModel()
        {
            AddNodeCommand = new RelayCommand(AddNode);
            SaveCommand = new RelayCommand(SaveGraph);
        }

        private void AddNode()
        {
            Nodes.Add(new Node
            {
                Id = nodeCounter,
                Label = $"Node {nodeCounter + 1}",
                X = 100 + 70 * nodeCounter,
                Y = 100
            });
            nodeCounter++;
        }

        private void SaveGraph()
        {
            var graph = new { Nodes, Edges };
            var json = JsonSerializer.Serialize(graph, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("graph.json", json);
        }

        public void AddEdgeByIds(int fromId, int toId)
        {
            var fromNode = Nodes.FirstOrDefault(n => n.Id == fromId);
            var toNode = Nodes.FirstOrDefault(n => n.Id == toId);

            if (fromNode == null || toNode == null)
                return;

            if (Edges.Any(e => (e.FromNodeId == fromId && e.ToNodeId == toId) || (e.FromNodeId == toId && e.ToNodeId == fromId)))
                return;

            Edges.Add(new Edge
            {
                FromNodeId = fromId,
                ToNodeId = toId,
                FromNode = fromNode,
                ToNode = toNode
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
