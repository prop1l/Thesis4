using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ThesisCourse_4.MVVM.Models
{
    public class ButtonModel : INotifyPropertyChanged
    {
        private int _row;
        private int _column;
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }
        public int Row
        {
            get => _row;
            set { if (_row != value) { _row = value; OnPropertyChanged(); } }
        }
        public int Column
        {
            get => _column;
            set { if (_column != value) { _column = value; OnPropertyChanged(); } }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));


        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        public override string ToString() => $"{Name}|{Row}|{Column}";

        public static ButtonModel FromString(string line)
        {
            var parts = line.Split('|');
            return new ButtonModel
            {
                Name = parts[0],
                Row = int.Parse(parts[1]),
                Column = int.Parse(parts[2])
            };
        }
    }
}