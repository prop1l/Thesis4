using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ThesisCourse_4.MVVM.Models
{
    public class Node : INotifyPropertyChanged
    {
        private double x;
        private double y;
        public double CenterX => X + 30;
        public double CenterY => Y + 30;

        public int Id { get; set; }
        public string Label { get; set; }

        public double X
        {
            get => x;
            set { x = value; OnPropertyChanged(); }
        }

        public double Y
        {
            get => y;
            set { y = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public class Edge : INotifyPropertyChanged
    {
        public int FromNodeId { get; set; }
        public int ToNodeId { get; set; }

        private Node? fromNode;
        public Node? FromNode
        {
            get => fromNode;
            set
            {
                if (fromNode != null)
                    fromNode.PropertyChanged -= Node_PropertyChanged;

                fromNode = value;

                if (fromNode != null)
                    fromNode.PropertyChanged += Node_PropertyChanged;

                OnPropertyChanged();
            }
        }

        private Node? toNode;
        public Node? ToNode
        {
            get => toNode;
            set
            {
                if (toNode != null)
                    toNode.PropertyChanged -= Node_PropertyChanged;

                toNode = value;

                if (toNode != null)
                    toNode.PropertyChanged += Node_PropertyChanged;

                OnPropertyChanged();
            }
        }

        private void Node_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Node.X) || e.PropertyName == nameof(Node.Y))
            {
                OnPropertyChanged(nameof(FromNode));
                OnPropertyChanged(nameof(ToNode));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
