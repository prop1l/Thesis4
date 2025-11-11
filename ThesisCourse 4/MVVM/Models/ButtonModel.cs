using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ThesisCourse_4.MVVM.Models
{
    public class CustomButton : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public int Row { get; set; }
        public int Column { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        public override string ToString() => $"{Name}|{Row}|{Column}";

        public static CustomButton FromString(string line)
        {
            var parts = line.Split('|');
            return new CustomButton
            {
                Name = parts[0],
                Row = int.Parse(parts[1]),
                Column = int.Parse(parts[2])
            };
        }
    }
}