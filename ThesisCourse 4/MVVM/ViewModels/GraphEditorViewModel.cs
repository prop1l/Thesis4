using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using ThesisCourse_4.MVVM.Commands;
using ThesisCourse_4.MVVM.Models;
using ThesisCourse_4.MVVM.Views;
using System.Numerics;

namespace ThesisCourse_4.MVVM.ViewModels
{
    public class GraphEditorViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly Random _rand = new();

        private int nodeCnt = 1;
        private int heightCnt;
        private int widthCnt;

        private const double CanvasWidth = 1200;
        private const double CanvasHeight = 600;
        private const double NodeRadius = 22;

        private string? _graphFilePath;
        private Node? _selectedNodeToRemove;

        #endregion

        #region Properties and Commands

        public ObservableCollection<Node> Nodes { get; } = new();
        public ObservableCollection<Edge> Edges { get; } = new();

        public ICommand AddNodeCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand RemoveNodeCommand { get; }
        public ICommand RemoveSelectedNodeCommand { get; }
        public ICommand RemoveAllCommand { get; }
        public ICommand OpenMatrixCommand { get; }
        public ICommand RandomizeLayoutCommand { get; }
        public ICommand FullRandomizeLayoutCommand { get; }
        public ICommand TreeLayoutCommand { get; }
        public ICommand ClearSelectedNodeCommand { get; }

        public Node? SelectedNodeToRemove
        {
            get => _selectedNodeToRemove;
            set
            {
                if (_selectedNodeToRemove != value)
                {
                    _selectedNodeToRemove = value;
                    OnPropertyChanged();
                }
            }
        }

        private class GraphData
        {
            public List<Node> Nodes { get; set; } = new();
            public List<Edge> Edges { get; set; } = new();
        }

        #endregion

        #region Constructor

        public GraphEditorViewModel()
        {
            AddNodeCommand = new RelayCommand(AddNode);
            SaveCommand = new RelayCommand(SaveGraphSafe);
            RemoveNodeCommand = new RelayCommand<Node>(RemoveNode);
            RemoveSelectedNodeCommand = new RelayCommand(RemoveSelectedNode, () => SelectedNodeToRemove != null);
            RemoveAllCommand = new RelayCommand(RemoveAll);
            OpenMatrixCommand = new RelayCommand(OpenMatrixWindow);
            RandomizeLayoutCommand = new RelayCommand(RandomizeLayout);
            FullRandomizeLayoutCommand = new RelayCommand(FullRandomizeLayout);
            TreeLayoutCommand = new RelayCommand(TreeLayout);
            ClearSelectedNodeCommand = new RelayCommand(() => SelectedNodeToRemove = null);
        }

        #endregion

        #region Persistence

        public void SetGraphFileName(string fileName)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "ThesisCourse_4");
            Directory.CreateDirectory(folder);
            _graphFilePath = Path.Combine(folder, fileName + ".json");
            LoadGraph();
        }

        public void SaveGraphSafe()
        {
            if (string.IsNullOrEmpty(_graphFilePath))
                return;

            if (!File.Exists(_graphFilePath) && Nodes.Count == 0)
                return;

            SaveGraph();
        }

        public void SaveGraph()
        {
            if (string.IsNullOrEmpty(_graphFilePath))
                throw new InvalidOperationException("Graph file path is not set.");

            try
            {
                var saveModel = new
                {
                    Nodes = Nodes.Select(n => new { n.Id, n.Label, n.X, n.Y }).ToList(),
                    Edges = Edges.Select(e => new { e.FromNodeId, e.ToNodeId }).ToList()
                };

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(saveModel, options);
                File.WriteAllText(_graphFilePath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при сохранении графа: {ex.Message}");
            }
        }

        public void LoadGraph()
        {
            if (string.IsNullOrEmpty(_graphFilePath))
                return;

            try
            {
                if (!File.Exists(_graphFilePath))
                {
                    ResetCounters();
                    return;
                }

                string json = File.ReadAllText(_graphFilePath);
                var loaded = JsonSerializer.Deserialize<GraphData>(json);

                if (loaded == null)
                {
                    ResetCounters();
                    return;
                }

                Nodes.Clear();
                foreach (var node in loaded.Nodes)
                    Nodes.Add(node);

                Edges.Clear();
                foreach (var edge in loaded.Edges)
                    Edges.Add(edge);

                ReconnectEdgesToNodes();

                nodeCnt = Nodes.Count > 0 ? Nodes.Max(n => n.Id) + 1 : 1;

                int nodeCount = Nodes.Count;
                widthCnt = nodeCount / 6;
                heightCnt = nodeCount % 6;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при загрузке графа: {ex.Message}");
                ResetCounters();
            }
        }

        private void ResetCounters()
        {
            nodeCnt = 1;
            heightCnt = 0;
            widthCnt = 0;
        }

        private void ReconnectEdgesToNodes()
        {
            foreach (var edge in Edges)
            {
                edge.FromNode = Nodes.FirstOrDefault(n => n.Id == edge.FromNodeId);
                edge.ToNode = Nodes.FirstOrDefault(n => n.Id == edge.ToNodeId);
            }
        }

        #endregion

        #region Graph editing

        private void AddNode()
        {
            if ((heightCnt + 1) % 6 == 0)
            {
                widthCnt++;
                heightCnt = 0;
            }

            Nodes.Add(new Node
            {
                Id = nodeCnt,
                Label = $"{nodeCnt}",
                X = 100 + 70 * heightCnt,
                Y = 100 + 70 * widthCnt
            });

            nodeCnt++;
            heightCnt++;
        }

        public void AddEdgeByIds(int fromId, int toId)
        {
            var fromNode = Nodes.FirstOrDefault(n => n.Id == fromId);
            var toNode = Nodes.FirstOrDefault(n => n.Id == toId);

            if (fromNode == null || toNode == null)
                return;

            if (Edges.Any(e =>
                    (e.FromNodeId == fromId && e.ToNodeId == toId) ||
                    (e.FromNodeId == toId && e.ToNodeId == fromId)))
                return;

            Edges.Add(new Edge
            {
                FromNodeId = fromId,
                ToNodeId = toId,
                FromNode = fromNode,
                ToNode = toNode
            });
        }

        private void RemoveSelectedNode()
        {
            if (SelectedNodeToRemove == null)
                return;

            RemoveNode(SelectedNodeToRemove);
            SelectedNodeToRemove = null;
        }

        public void RemoveNode(Node? node)
        {
            if (node == null)
                return;

            var edgesToRemove = Edges
                .Where(e => e.FromNodeId == node.Id || e.ToNodeId == node.Id)
                .ToList();

            foreach (var edge in edgesToRemove)
                Edges.Remove(edge);

            Nodes.Remove(node);
        }

        private void RemoveAll()
        {
            Nodes.Clear();
            Edges.Clear();
            ResetCounters();
        }

        #endregion

        #region Matrix

        private void OpenMatrixWindow()
        {
            var window = new AdjacencyMatrixWindow(
                GetAdjacencyMatrixParallelFast(),
                Nodes.Select(n => n.Label).ToList());

            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        public int[,] GetAdjacencyMatrixParallelFast()
        {
            int n = Nodes.Count;
            int[,] matrix = new int[n, n];

            var nodeIndex = Nodes
                .Select((node, idx) => new { node.Id, idx })
                .ToDictionary(x => x.Id, x => x.idx);

            var edges = Edges.ToArray();

            Parallel.For(0, edges.Length, k =>
            {
                var edge = edges[k];
                if (!nodeIndex.TryGetValue(edge.FromNodeId, out int i)) return;
                if (!nodeIndex.TryGetValue(edge.ToNodeId, out int j)) return;
                matrix[i, j] = 1;
                matrix[j, i] = 1;
            });

            return matrix;
        }

        #endregion

        #region Layout algorithms

        private void FullRandomizeLayout()
        {
            if (Nodes.Count == 0) return;

            double minX = NodeRadius, maxX = CanvasWidth - NodeRadius;
            double minY = NodeRadius, maxY = CanvasHeight - NodeRadius;
            double minDist = NodeRadius * 4;

            var active = new List<(Vector2 pos, int nodeIndex)>();
            var placed = new bool[Nodes.Count];

            // Первый узел случайный
            int firstIndex = _rand.Next(Nodes.Count);
            Nodes[firstIndex].X = minX + _rand.NextDouble() * (maxX - minX);
            Nodes[firstIndex].Y = minY + _rand.NextDouble() * (maxY - minY);
            active.Add((new Vector2((float)Nodes[firstIndex].X, (float)Nodes[firstIndex].Y), firstIndex));
            placed[firstIndex] = true;

            const int maxTries = 30;
            while (active.Count > 0)
            {
                int idx = _rand.Next(active.Count);
                var current = active[idx]; // var current вместо деструктуризации

                bool found = false;
                for (int i = 0; i < maxTries; i++)
                {
                    double angle = _rand.NextDouble() * 2 * Math.PI;
                    double radius = minDist * (0.7 + _rand.NextDouble() * 0.6);

                    float candidateX = current.pos.X + (float)(Math.Cos(angle) * radius);
                    float candidateY = current.pos.Y + (float)(Math.Sin(angle) * radius);

                    if (candidateX < minX || candidateX > maxX || candidateY < minY || candidateY > maxY)
                        continue;

                    bool ok = true;
                    for (int j = 0; j < Nodes.Count; j++)
                    {
                        if (!placed[j]) continue;
                        double dx = Nodes[j].X - candidateX;
                        double dy = Nodes[j].Y - candidateY;
                        if (dx * dx + dy * dy < minDist * minDist)
                        {
                            ok = false;
                            break;
                        }
                    }

                    if (ok)
                    {
                        int targetIndex = -1;
                        for (int j = 0; j < Nodes.Count; j++)
                        {
                            if (!placed[j])
                            {
                                targetIndex = j;
                                break;
                            }
                        }

                        if (targetIndex != -1)
                        {
                            Nodes[targetIndex].X = candidateX;
                            Nodes[targetIndex].Y = candidateY;
                            placed[targetIndex] = true;
                            active.Add((new Vector2(candidateX, candidateY), targetIndex));
                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                    active.RemoveAt(idx);
            }

            // Остальные узлы - обычный рандом
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (!placed[i])
                {
                    Nodes[i].X = minX + _rand.NextDouble() * (maxX - minX);
                    Nodes[i].Y = minY + _rand.NextDouble() * (maxY - minY);
                }
            }
        }

        private void RandomizeLayout()
        {
            if (Nodes.Count == 0)
                return;

            double minX = NodeRadius;
            double maxX = CanvasWidth - NodeRadius;
            double minY = NodeRadius;
            double maxY = CanvasHeight - NodeRadius;

            double centerX = (minX + maxX) / 2;
            double centerY = (minY + maxY) / 2;

            double minDist = NodeRadius * 3;
            double minDistSq = minDist * minDist;

            var adj = Nodes.ToDictionary(n => n.Id, _ => new List<int>());
            foreach (var e in Edges)
            {
                if (adj.ContainsKey(e.FromNodeId) && adj.ContainsKey(e.ToNodeId))
                {
                    adj[e.FromNodeId].Add(e.ToNodeId);
                    adj[e.ToNodeId].Add(e.FromNodeId);
                }
            }

            var root = Nodes.MinBy(n => n.Id)!;
            var placed = new HashSet<int>();
            var queue = new Queue<int>();

            double radiusRoot = 100;
            double angleRoot = 2 * Math.PI * _rand.NextDouble();
            double rRoot = radiusRoot * Math.Sqrt(_rand.NextDouble());

            double rootX = centerX + Math.Cos(angleRoot) * rRoot;
            double rootY = centerY + Math.Sin(angleRoot) * rRoot;

            root.X = Math.Clamp(rootX, minX, maxX);
            root.Y = Math.Clamp(rootY, minY, maxY);

            placed.Add(root.Id);
            queue.Enqueue(root.Id);

            while (queue.Count > 0)
            {
                int currentId = queue.Dequeue();
                var parentNode = Nodes.First(n => n.Id == currentId);

                if (!adj.TryGetValue(currentId, out var neigh))
                    continue;

                int degree = neigh.Count == 0 ? 1 : neigh.Count;
                double baseRad = minDist * 2.6;
                double angleStep = 2 * Math.PI / degree;

                int childIndex = 0;

                foreach (int toId in neigh)
                {
                    if (placed.Contains(toId))
                        continue;

                    var child = Nodes.First(n => n.Id == toId);

                    const int maxTries = 40;
                    bool okPlace = false;

                    for (int t = 0; t < maxTries; t++)
                    {
                        double angle = angleStep * childIndex +
                                        (_rand.NextDouble() - 0.5) * angleStep * 0.3;
                        double radius = baseRad * (0.9 + _rand.NextDouble() * 0.4);

                        double x = parentNode.X + Math.Cos(angle) * radius;
                        double y = parentNode.Y + Math.Sin(angle) * radius;

                        x = Math.Clamp(x, minX, maxX);
                        y = Math.Clamp(y, minY, maxY);

                        bool ok = true;
                        foreach (var other in Nodes)
                        {
                            if (!placed.Contains(other.Id) || other.Id == child.Id)
                                continue;

                            double dx = other.X - x;
                            double dy = other.Y - y;
                            double distSq = dx * dx + dy * dy;
                            if (distSq < minDistSq)
                            {
                                ok = false;
                                break;
                            }
                        }

                        if (!ok) continue;

                        child.X = x;
                        child.Y = y;
                        okPlace = true;
                        break;
                    }

                    if (!okPlace)
                    {
                        child.X = parentNode.X + minDist;
                        child.Y = parentNode.Y;
                    }

                    placed.Add(child.Id);
                    queue.Enqueue(child.Id);
                    childIndex++;
                }
            }

            var isolated = Nodes.Where(n => !placed.Contains(n.Id)).ToList();
            if (isolated.Count > 0)
            {
                double radius = Math.Min(maxX - minX, maxY - minY) / 2.5;
                for (int i = 0; i < isolated.Count; i++)
                {
                    double angle = 2 * Math.PI * i / isolated.Count;
                    double x = centerX + Math.Cos(angle) * radius;
                    double y = centerY + Math.Sin(angle) * radius;

                    isolated[i].X = Math.Clamp(x, minX, maxX);
                    isolated[i].Y = Math.Clamp(y, minY, maxY);
                }
            }
        }

        private void TreeLayout()
        {
            if (Nodes.Count == 0)
                return;

            var adj = Nodes.ToDictionary(n => n.Id, _ => new List<int>());
            foreach (var e in Edges)
            {
                if (adj.ContainsKey(e.FromNodeId) && adj.ContainsKey(e.ToNodeId))
                {
                    adj[e.FromNodeId].Add(e.ToNodeId);
                    adj[e.ToNodeId].Add(e.FromNodeId);
                }
            }

            var root = Nodes.MinBy(n => n.Id)!;

            var level = new Dictionary<int, int>();
            var parent = new Dictionary<int, int?>();
            var queue = new Queue<int>();

            level[root.Id] = 0;
            parent[root.Id] = null;
            queue.Enqueue(root.Id);

            while (queue.Count > 0)
            {
                int v = queue.Dequeue();
                int cur = level[v];

                foreach (var to in adj[v])
                {
                    if (level.ContainsKey(to)) continue;
                    level[to] = cur + 1;
                    parent[to] = v;
                    queue.Enqueue(to);
                }
            }

            int maxLevel = level.Values.Count > 0 ? level.Values.Max() : 0;
            foreach (var n in Nodes)
                if (!level.ContainsKey(n.Id))
                {
                    level[n.Id] = maxLevel + 1;
                    parent[n.Id] = null;
                }

            var orderedByLevel = new Dictionary<int, List<Node>>();
            foreach (var n in Nodes)
            {
                int l = level[n.Id];
                if (!orderedByLevel.ContainsKey(l))
                    orderedByLevel[l] = new List<Node>();
                orderedByLevel[l].Add(n);
            }

            for (int l = 1; l <= maxLevel; l++)
            {
                if (!orderedByLevel.ContainsKey(l)) continue;
                var current = orderedByLevel[l];

                orderedByLevel[l] = current
                    .Select(n => new
                    {
                        Node = n,
                        ParentId = parent.TryGetValue(n.Id, out var p) ? p : null
                    })
                    .OrderBy(x => x.ParentId ?? int.MaxValue)
                    .ThenBy(x => x.Node.Id)
                    .Select(x => x.Node)
                    .ToList();
            }

            double minX = NodeRadius;
            double maxX = CanvasWidth - NodeRadius;
            double levelHeight = 90;
            double minDist = NodeRadius * 2.7;
            double minDistSq = minDist * minDist;

            var groups = orderedByLevel
                .OrderBy(kv => kv.Key)
                .Select(kv => kv.Value)
                .ToList();

            for (int levelIndex = 0; levelIndex < groups.Count; levelIndex++)
            {
                var nodesOnLevel = groups[levelIndex];
                int count = nodesOnLevel.Count;
                if (count == 0) continue;

                double baseY = NodeRadius + levelIndex * levelHeight;
                double totalWidth = maxX - minX;
                double step = totalWidth / (count + 1);

                for (int i = 0; i < count; i++)
                {
                    var node = nodesOnLevel[i];
                    double bx = minX + step * (i + 1);

                    double jitterX = (_rand.NextDouble() - 0.5) * NodeRadius;
                    double jitterY = (_rand.NextDouble() - 0.5) * (NodeRadius * 0.6);

                    node.X = Math.Clamp(bx + jitterX, minX, maxX);
                    node.Y = baseY + jitterY;
                }
            }

            const int relaxIterations = 5;
            for (int iter = 0; iter < relaxIterations; iter++)
            {
                foreach (var nodesOnLevel in groups)
                {
                    int count = nodesOnLevel.Count;
                    for (int i = 0; i < count; i++)
                        for (int j = i + 1; j < count; j++)
                        {
                            var a = nodesOnLevel[i];
                            var b = nodesOnLevel[j];

                            double dx = b.X - a.X;
                            double dy = b.Y - a.Y;
                            double distSq = dx * dx + dy * dy;
                            if (distSq >= minDistSq || distSq <= 0.0001) continue;

                            double dist = Math.Sqrt(distSq);
                            double overlap = (minDist - dist) / 2.0;
                            double kx = dx / dist;

                            a.X = Math.Clamp(a.X - kx * overlap, minX, maxX);
                            b.X = Math.Clamp(b.X + kx * overlap, minX, maxX);
                        }
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        #endregion
    }
}
