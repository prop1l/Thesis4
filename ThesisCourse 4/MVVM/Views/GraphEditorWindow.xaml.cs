using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ThesisCourse_4.MVVM.Models;

namespace ThesisCourse_4.MVVM.Views
{
    public partial class GraphEditorWindow : Window
    {
        private bool isDragging = false;
        private Ellipse? draggedEllipse = null;
        private Point mouseOffset;

        public GraphEditorWindow()
        {
            InitializeComponent();
        }

        private Canvas? GetParentCanvas(DependencyObject child)
        {
            DependencyObject? parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is Canvas))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as Canvas;
        }

        private void Ellipse_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedEllipse = sender as Ellipse;
            if (draggedEllipse != null)
            {
                isDragging = true;

                var canvas = GetParentCanvas(draggedEllipse);

                if (canvas == null)
                    return;

                var mousePosition = e.GetPosition(canvas);
                if (draggedEllipse.DataContext is Node node)
                {
                    mouseOffset = new Point(mousePosition.X - node.X, mousePosition.Y - node.Y);
                }

                draggedEllipse.CaptureMouse();
                e.Handled = true;
            }
        }

        private void Ellipse_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && draggedEllipse != null && draggedEllipse.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            {
                var canvas = GetParentCanvas(draggedEllipse);
                if (canvas == null)
                    return;

                var mousePosition = e.GetPosition(canvas);
                if (draggedEllipse.DataContext is Node node)
                {
                    node.X = mousePosition.X - mouseOffset.X;
                    node.Y = mousePosition.Y - mouseOffset.Y;
                }
                e.Handled = true;
            }
        }

        private void Ellipse_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging && draggedEllipse != null)
            {
                draggedEllipse.ReleaseMouseCapture();
                isDragging = false;
                draggedEllipse = null;
                e.Handled = true;
            }
        }
    }
}
