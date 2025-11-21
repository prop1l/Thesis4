using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ThesisCourse_4.MVVM.Models;
using ThesisCourse_4.MVVM.ViewModels;

namespace ThesisCourse_4.MVVM.Views
{
    public partial class Welcome : Window
    {
        private ContextMenu _contextMenu;

        public Welcome()
        {
            InitializeComponent();

            PreviewMouseDoubleClick += OnPreviewMouseDoubleClick;
            PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;

            // TODO: AFTER RENAME DO NEW SAVE TO FILE WITH EDGE AND ВЕРШИНАМИ)
            ButtonsItemsControl.AddHandler(Button.PreviewMouseRightButtonDownEvent,
                new MouseButtonEventHandler(OnButtonRightClick)); 
        }

        #region Контекстное меню по правому клику с динамическим созданием

        private void OnButtonRightClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is DependencyObject source)
            {
                var button = FindAncestor<Button>(source);
                if (button != null)
                {
                    if (_contextMenu == null)
                    {
                        _contextMenu = new ContextMenu();

                        var renameItem = new MenuItem { Header = "Переименовать" };
                        renameItem.Click += RenameItem_Click;
                        _contextMenu.Items.Add(renameItem);

                        var deleteItem = new MenuItem { Header = "Удалить" };
                        deleteItem.Click += DeleteItem_Click;
                        deleteItem.Foreground = Brushes.Red;
                        _contextMenu.Items.Add(deleteItem);
                    }

                    _contextMenu.Tag = button.DataContext;

                    _contextMenu.PlacementTarget = button;
                    _contextMenu.IsOpen = true;

                    e.Handled = true;
                }
            }
        }

        private T? FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null && !(current is T))
                current = VisualTreeHelper.GetParent(current);
            return current as T;
        }

        private void RenameItem_Click(object sender, RoutedEventArgs e)
        {
            if (_contextMenu?.Tag is ButtonModel btn)
            {
                if (DataContext is WelcomeViewModel vm)
                {
                    vm.RenameGraphCommand.Execute(btn.Name);
                }
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (_contextMenu?.Tag is ButtonModel btn)
            {
                if (DataContext is WelcomeViewModel vm)
                {
                    vm.DeleteGraphCommand.Execute(btn.Name);
                }
            }
        }

        #endregion

        #region Drag & Resize

        private bool IsClickInHeaderButNotButtons(object source)
        {
            var current = source as DependencyObject;
            while (current != null)
            {
                if (current is FrameworkElement fe)
                {
                    if (fe.Name is "MinimizeButton" or "MaximizeButton" or "CloseButton")
                        return false;
                    if (fe.GetType().Name == "Header")
                        return true;
                }
                current = VisualTreeHelper.GetParent(current) as FrameworkElement;
            }
            return false;
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
    }
}
