using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using ThesisCourse_4.MVVM.Models;
using ThesisCourse_4.MVVM.Commands;
using ThesisCourse_4.MVVM.Views;
using System.Windows;

namespace ThesisCourse_4.MVVM.ViewModels
{
    public class GraphEditorViewModel : INotifyPropertyChanged
    {
        #region Fields

        private int nodeCounter = 1;
        private int heightCnt = 0;
        private int widthCnt = 0;
        private int nodeIdToRemove;

        #endregion

        #region Properties

        public ObservableCollection<Node> Nodes { get; } = new();
        public ObservableCollection<Edge> Edges { get; } = new();

        public ICommand AddNodeCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand RemoveNodeCommand { get; }
        public ICommand RemoveNodeByIdCommand { get; }
        public ICommand RemoveAllCommand { get; }
        public ICommand OpenMatrixCommand { get; }

        public int NodeIdToRemove
        {
            get => nodeIdToRemove;
            set { nodeIdToRemove = value; OnPropertyChanged(); }
        }

        #endregion

        #region Constructor

        public GraphEditorViewModel()
        {
            AddNodeCommand = new RelayCommand(AddNode);
            SaveCommand = new RelayCommand(SaveGraph);
            RemoveNodeCommand = new RelayCommand<Node>(RemoveNode);
            RemoveNodeByIdCommand = new RelayCommand<int>(RemoveNodeById);
            RemoveAllCommand = new RelayCommand(RemoveAll);
            OpenMatrixCommand = new RelayCommand(OpenMatrixWindow);
        }

        #endregion

        #region Methods

        private void AddNode()
        {
            if ((heightCnt + 1) % 6 == 0)
            {
                widthCnt++;
                heightCnt /= 6;
            }

            Nodes.Add(new Node
            {
                Id = nodeCounter,
                Label = $"{nodeCounter}",
                X = 100 + 70 * heightCnt,
                Y = 100 + 70 * widthCnt
            });
            nodeCounter++;
            heightCnt++;
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

        public void RemoveNode(Node node)
        {
            if (node == null)
                return;

            var edgesToRemove = Edges.Where(e => e.FromNodeId == node.Id || e.ToNodeId == node.Id).ToList();

            foreach (var edge in edgesToRemove)
            {
                Edges.Remove(edge);
            }

            Nodes.Remove(node);
        }

        public void RemoveNodeById(int nodeId)
        {
            var node = Nodes.FirstOrDefault(n => n.Id == nodeId);
            if (node != null)
            {
                RemoveNode(node);
            }
        }

        private void RemoveAll()
        {
            Nodes.Clear();
            Edges.Clear();
            nodeCounter = 1;
            heightCnt = 0;
            widthCnt = 0;
        }

        public int[,] GetAdjacencyMatrixParallelFast()
        {
            int n = Nodes.Count;
            int[,] matrix = new int[n, n];
            var nodeIndex = Nodes.Select((node, idx) => new { node.Id, idx }).ToDictionary(x => x.Id, x => x.idx);
            var edges = Edges.ToArray();

            Parallel.For(0, edges.Length, k =>
            {
                var edge = edges[k];
                if (nodeIndex.TryGetValue(edge.FromNodeId, out int i) &&
                    nodeIndex.TryGetValue(edge.ToNodeId, out int j))
                {
                    matrix[i, j] = 1;
                    matrix[j, i] = 1;
                }
            });

            return matrix;
        }




        private void OpenMatrixWindow()
        {
            var window = new AdjacencyMatrixWindow(GetAdjacencyMatrixParallelFast(), Nodes.Select(n => n.Label).ToList());
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        #endregion
    }
}
