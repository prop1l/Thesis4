using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ThesisCourse_4.MVVM.Models;
using ThesisCourse_4.MVVM.ViewModels;

namespace ThesisCourse_4.MVVM.Views
{
    public partial class GraphEditorWindow : Window
    {
        private bool isDraggingNode = false;
        private Ellipse? draggedNodeEllipse = null;
        private Point mouseOffsetNode;

        private bool isDraggingEdge = false;
        private Node? edgeStartNode = null;
        private Line? previewEdgeLine = null;
        private Ellipse? draggedEdgeEllipse = null;

        public GraphEditorWindow()
        {
            InitializeComponent();
        }

        private GraphEditorViewModel GetViewModel() =>
            DataContext as GraphEditorViewModel ?? throw new InvalidOperationException("DataContext должен быть GraphEditorViewModel");

        private Canvas? FindParentCanvas(DependencyObject child)
        {
            var parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is Canvas))
                parent = VisualTreeHelper.GetParent(parent);
            return parent as Canvas;
        }

        // Перетаскиваем узел ЛКМ
        private void Ellipse_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedNodeEllipse = sender as Ellipse;
            if (draggedNodeEllipse != null)
            {
                isDraggingNode = true;
                var canvas = FindParentCanvas(draggedNodeEllipse);
                if (canvas == null) return;

                var pos = e.GetPosition(canvas);
                if (draggedNodeEllipse.DataContext is Node node)
                {
                    mouseOffsetNode = new Point(pos.X - node.X, pos.Y - node.Y);
                }
                draggedNodeEllipse.CaptureMouse();
                e.Handled = true;
            }
        }

        private void Ellipse_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isDraggingNode && draggedNodeEllipse != null && draggedNodeEllipse.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            {
                var canvas = FindParentCanvas(draggedNodeEllipse);
                if (canvas == null) return;

                var pos = e.GetPosition(canvas);
                if (draggedNodeEllipse.DataContext is Node node)
                {
                    node.X = pos.X - mouseOffsetNode.X;
                    node.Y = pos.Y - mouseOffsetNode.Y;
                }
                e.Handled = true;
            }

            if (isDraggingEdge && previewEdgeLine != null)
            {
                var canvas = FindParentCanvas(draggedEdgeEllipse);
                if (canvas == null) return;

                var pos = e.GetPosition(canvas);
                previewEdgeLine.X2 = pos.X;
                previewEdgeLine.Y2 = pos.Y;
                e.Handled = true;
            }
        }

        private void Ellipse_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDraggingNode && draggedNodeEllipse != null)
            {
                draggedNodeEllipse.ReleaseMouseCapture();
                isDraggingNode = false;
                draggedNodeEllipse = null;
                e.Handled = true;
            }
        }

        // Создание ребра перетаскиванием правой кнопки мыши
        private void Ellipse_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedEdgeEllipse = sender as Ellipse;
            if (draggedEdgeEllipse == null) return;

            var canvas = FindParentCanvas(draggedEdgeEllipse);
            if (canvas == null) return;

            if (draggedEdgeEllipse.DataContext is Node node)
            {
                edgeStartNode = node;
                isDraggingEdge = true;

                var pos = e.GetPosition(canvas);
                previewEdgeLine = new Line()
                {
                    Stroke = Brushes.Gray,
                    StrokeThickness = 2,
                    X1 = node.X + 30,
                    Y1 = node.Y + 30,
                    X2 = pos.X,
                    Y2 = pos.Y
                };
                PreviewCanvas.Children.Add(previewEdgeLine);
            }
            draggedEdgeEllipse.CaptureMouse();
            e.Handled = true;
        }

        private void Ellipse_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var canvas = FindParentCanvas(draggedEdgeEllipse);
            if (canvas == null)
                return;

            var pos = e.GetPosition(canvas);

            HitTestResult hitResult = VisualTreeHelper.HitTest(canvas, pos);
            if (hitResult != null)
            {
                DependencyObject element = hitResult.VisualHit;
                while (element != null && !(element is Ellipse))
                    element = VisualTreeHelper.GetParent(element);

                if (element is Ellipse ellipse && ellipse.DataContext is Node endNode)
                {
                    if (edgeStartNode != null && edgeStartNode.Id != endNode.Id)
                    {
                        var vm = GetViewModel();
                        vm.AddEdgeByIds(edgeStartNode.Id, endNode.Id);
                    }
                }
            }
            if (previewEdgeLine != null)
            {
                PreviewCanvas.Children.Remove(previewEdgeLine);
                previewEdgeLine = null;
            }

            if (draggedEdgeEllipse != null)
            {
                draggedEdgeEllipse.ReleaseMouseCapture();
                draggedEdgeEllipse = null;
            }

            isDraggingEdge = false;
            edgeStartNode = null;

            e.Handled = true;
        }

    }
}
