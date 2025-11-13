using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ThesisCourse_4.MVVM.Models
{
    public class Node : INotifyPropertyChanged
    {
        private double x;
        private double y;

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


    public class Edge
    {
        public int FromNodeId { get; set; }
        public int ToNodeId { get; set; }
    }

}
