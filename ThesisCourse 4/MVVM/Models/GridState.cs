// ThesisCourse_4/MVVM/Models/GridState.cs
using System.Collections.ObjectModel;
using System.Windows;

namespace ThesisCourse_4.MVVM.Models
{
    public class GridState
    {
        public int RowCount { get; set; }
        public ObservableCollection<GridRowHeight> RowHeights { get; } = new();
    }

    public class GridRowHeight
    {
        public GridUnitType UnitType { get; set; } = GridUnitType.Auto;
        public double Value { get; set; } = double.NaN; 
    }
}