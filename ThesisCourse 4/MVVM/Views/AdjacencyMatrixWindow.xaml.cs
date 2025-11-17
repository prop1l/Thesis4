using System.Windows;

namespace ThesisCourse_4.MVVM.Views
{
    public partial class AdjacencyMatrixWindow : Window
    {
        public AdjacencyMatrixWindow(int[,] matrix, List<string> labels)
        {
            InitializeComponent();

            int n = matrix.GetLength(0);
            var dt = new System.Data.DataTable();

            dt.Columns.Add("№\\№");
            foreach (var lbl in labels)
                dt.Columns.Add(lbl);

            string[,] temp = new string[n, n + 1];

            System.Threading.Tasks.Parallel.For(0, n, i =>
            {
                temp[i, 0] = labels[i];
                for (int j = 0; j < n; j++)
                {
                    temp[i, j + 1] = matrix[i, j].ToString();
                }
            });

            for (int i = 0; i < n; i++)
            {
                var row = dt.NewRow();
                for (int j = 0; j <= n; j++)
                {
                    row[j] = temp[i, j];
                }
                dt.Rows.Add(row);
            }

            MatrixGrid.ItemsSource = dt.DefaultView;
        }

        private void OnCloseClick(object sender, RoutedEventArgs e) => Close();

    }
}
