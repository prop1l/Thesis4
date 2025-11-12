using System.Windows;
using System.Windows.Controls;
using ThesisCourse_4.MVVM.Models;

namespace ThesisCourse_4.Behaviors
{
    public static class DynamicGridBehavior
    {
        #region GridState (Attached Property)
        public static readonly DependencyProperty GridStateProperty =
            DependencyProperty.RegisterAttached(
                "GridState",
                typeof(GridState),
                typeof(DynamicGridBehavior),
                new PropertyMetadata(null, OnGridStateChanged));

        public static void SetGridState(DependencyObject element, GridState value) =>
            element.SetValue(GridStateProperty, value);

        public static GridState? GetGridState(DependencyObject element) =>
            (GridState?)element.GetValue(GridStateProperty);
        #endregion

        private static void OnGridStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Grid grid && e.NewValue is GridState state)
            {
                SyncRowDefinitions(grid, state);
            }
        }

        private static void SyncRowDefinitions(Grid grid, GridState state)
        {
            var columnDefs = new List<ColumnDefinition>();
            foreach (var col in grid.ColumnDefinitions)
                columnDefs.Add(new ColumnDefinition { Width = col.Width });

            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            // Восстанавливаем колонки
            foreach (var col in columnDefs)
                grid.ColumnDefinitions.Add(col);

            // Доб строки
            for (int i = 0; i < state.RowCount; i++)
            {
                var height = i < state.RowHeights.Count
                    ? state.RowHeights[i]
                    : new GridRowHeight();

                var rowDef = new RowDefinition
                {
                    Height = double.IsNaN(height.Value)
                        ? GridLength.Auto
                        : new GridLength(height.Value, height.UnitType)
                };
                grid.RowDefinitions.Add(rowDef);
            }
        }
    }
}