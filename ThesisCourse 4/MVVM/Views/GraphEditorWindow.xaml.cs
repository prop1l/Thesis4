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
        #region Fields

        private bool isDraggingNode = false;
        private Ellipse? draggedNodeEllipse = null;
        private Point mouseOffsetNode;

        private bool isDraggingEdge = false;
        private Node? edgeStartNode = null;
        private Line? previewEdgeLine = null;
        private Ellipse? draggedEdgeEllipse = null;

        #endregion

        #region Constructor

        public GraphEditorWindow()
        {
            InitializeComponent();
            PreviewMouseDoubleClick += OnPreviewMouseDoubleClick;
            PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            Closing += GraphEditorWindow_Closing;
        }

        #endregion

        #region Window Drag and Resize Helpers

        private bool IsClickInHeaderButNotButtons(object source)
        {
            var current = source as DependencyObject;
            while (current != null)
            {
                if (current is FrameworkElement fe)
                {
                    if (fe.Name is "MinimizeButton" or "MaximizeButton" or "CloseButton") return false;
                    if (fe.GetType().Name == "Header") return true;
                }
                current = VisualTreeHelper.GetParent(current) as FrameworkElement;
            }
            return false;
        }

        private void GraphEditorWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is GraphEditorViewModel vm) vm.SaveGraph();
        }

        private void OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsClickInHeaderButNotButtons(e.OriginalSource))
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                e.Handled = true;
            }
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsClickInHeaderButNotButtons(e.OriginalSource) && WindowState == WindowState.Normal)
            {
                DragMove();
                e.Handled = true;
            }
        }

        #endregion

        #region Helpers

        private GraphEditorViewModel GetViewModel() =>
            DataContext as GraphEditorViewModel ?? throw new InvalidOperationException("DataContext должен быть GraphEditorViewModel");

        private Canvas? FindParentCanvas(DependencyObject? child)
        {
            if (child == null)
                return null;

            var parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is Canvas))
                parent = VisualTreeHelper.GetParent(parent);
            return parent as Canvas;
        }


        #endregion

        #region Node Dragging Logic

        private void Ellipse_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedNodeEllipse = sender as Ellipse;
            if (draggedNodeEllipse == null)
                return;

            var canvas = FindParentCanvas(draggedNodeEllipse);
            if (canvas == null)
                return;

            if (draggedNodeEllipse.DataContext is Node node)
            {
                var pos = e.GetPosition(canvas);
                mouseOffsetNode = new Point(pos.X - node.X, pos.Y - node.Y);

                isDraggingNode = true;
                draggedNodeEllipse.CaptureMouse();
                e.Handled = true;
            }
        }

        private void Ellipse_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isDraggingNode && draggedNodeEllipse != null && draggedNodeEllipse.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            {
                var canvas = FindParentCanvas(draggedNodeEllipse);
                if (canvas == null)
                    return;

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
                if (canvas == null)
                    return;

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
                var canvas = FindParentCanvas(draggedNodeEllipse);
                if (canvas == null)
                    return;

                var pos = e.GetPosition(TrashCanvas);
                if (pos.X >= 0 && pos.X <= TrashCanvas.ActualWidth && pos.Y >= 0 && pos.Y <= TrashCanvas.ActualHeight)
                {
                    if (draggedNodeEllipse.DataContext is Node node)
                    {
                        var vm = GetViewModel();
                        vm.RemoveNode(node);
                    }
                }

                draggedNodeEllipse.ReleaseMouseCapture();
                isDraggingNode = false;
                draggedNodeEllipse = null;
                e.Handled = true;
            }
        }

        #endregion

        #region Edge Dragging Logic

        private void Ellipse_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((draggedEdgeEllipse = sender as Ellipse) == null ||
                FindParentCanvas(draggedEdgeEllipse) is not Canvas canvas ||
                draggedEdgeEllipse.DataContext is not Node node)
                return;

            edgeStartNode = node;
            isDraggingEdge = true;

            var pos = e.GetPosition(canvas);
            previewEdgeLine = new Line
            {
                Stroke = Brushes.Gray,
                StrokeThickness = 2,
                X1 = node.CenterX,
                Y1 = node.CenterY,
                X2 = pos.X,
                Y2 = pos.Y
            };
            PreviewCanvas.Children.Add(previewEdgeLine);
            draggedEdgeEllipse.CaptureMouse();
            e.Handled = true;
        }

        private void Ellipse_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var canvas = FindParentCanvas(draggedEdgeEllipse);
            if (canvas == null) return;

            if (VisualTreeHelper.HitTest(canvas, e.GetPosition(canvas))?.VisualHit is Ellipse ellipse &&
                ellipse.DataContext is Node endNode &&
                edgeStartNode != null &&
                edgeStartNode.Id != endNode.Id)
            {
                GetViewModel().AddEdgeByIds(edgeStartNode.Id, endNode.Id);
            }

            PreviewCanvas.Children.Remove(previewEdgeLine);
            draggedEdgeEllipse?.ReleaseMouseCapture();

            previewEdgeLine = null;
            draggedEdgeEllipse = null;
            isDraggingEdge = false;
            edgeStartNode = null;

            e.Handled = true;
        }


        #endregion
    }

}
