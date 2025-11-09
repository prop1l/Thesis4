namespace ThesisCourse_4.MVVM.Models
{
    public class CustomButton
    {
        public string Name { get; set; }
        public int Row { get; set; } // строка
        public int Column { get; set; } // колонка

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
