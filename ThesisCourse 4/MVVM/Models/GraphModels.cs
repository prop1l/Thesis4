using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ThesisCourse_4.MVVM.Models
{
    public class Node : INotifyPropertyChanged
    {
        private int _id;
        private string _label = string.Empty;
        private double _x;
        private double _y;

        public int Id
        {
            get => _id;
            set { if (_id != value) { _id = value; OnPropertyChanged(); } }
        }

        public string Label
        {
            get => _label;
            set { if (_label != value) { _label = value; OnPropertyChanged(); } }
        }

        public double X
        {
            get => _x;
            set { if (_x != value) { _x = value; OnPropertyChanged(); OnPropertyChanged(nameof(CenterX)); } }
        }

        public double Y
        {
            get => _y;
            set { if (_y != value) { _y = value; OnPropertyChanged(); OnPropertyChanged(nameof(CenterY)); } }
        }

        public double CenterX => X + 30;
        public double CenterY => Y + 30;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class Edge : INotifyPropertyChanged
    {
        private int _fromNodeId;
        private int _toNodeId;
        private Node? _fromNode;
        private Node? _toNode;

        public int FromNodeId
        {
            get => _fromNodeId;
            set { if (_fromNodeId != value) { _fromNodeId = value; OnPropertyChanged(); } }
        }

        public int ToNodeId
        {
            get => _toNodeId;
            set { if (_toNodeId != value) { _toNodeId = value; OnPropertyChanged(); } }
        }

        public Node? FromNode
        {
            get => _fromNode;
            set
            {
                if (_fromNode != null)
                    _fromNode.PropertyChanged -= Node_PropertyChanged;

                if (_fromNode != value)
                {
                    _fromNode = value;
                    OnPropertyChanged();
                }

                if (_fromNode != null)
                    _fromNode.PropertyChanged += Node_PropertyChanged;
            }
        }

        public Node? ToNode
        {
            get => _toNode;
            set
            {
                if (_toNode != null)
                    _toNode.PropertyChanged -= Node_PropertyChanged;

                if (_toNode != value)
                {
                    _toNode = value;
                    OnPropertyChanged();
                }

                if (_toNode != null)
                    _toNode.PropertyChanged += Node_PropertyChanged;
            }
        }

        private void Node_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Node.X) || e.PropertyName == nameof(Node.Y) ||
                e.PropertyName == nameof(Node.CenterX) || e.PropertyName == nameof(Node.CenterY))
            {
                OnPropertyChanged(nameof(FromNode));
                OnPropertyChanged(nameof(ToNode));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
