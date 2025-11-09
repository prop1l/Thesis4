using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ThesisCourse_4.MVVM.Models;

namespace ThesisCourse_4.MVVM.Views
{
    public partial class Welcome : Window
    {
        private static string ButtonsFilePath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "buttons.txt");

        private int _buttonCounter = 1;

        public Welcome()
        {
            InitializeComponent();

            PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            PreviewMouseDoubleClick += OnPreviewMouseDoubleClick;
            MainScrollViewer.PreviewMouseWheel += OnScrollViewerMouseWheel;

            LoadButtons();
        }

        #region Заголовок

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
                current = VisualTreeHelper.GetParent(current);
            }
            return false;
        }

        private void OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsClickInHeaderButNotButtons(e.OriginalSource))
            {
                WindowState = WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
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

        private void OnScrollViewerMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                double scrollSpeedFactor = 2.75;

                e.Handled = true;

                scrollViewer.ScrollToVerticalOffset(
                    scrollViewer.VerticalOffset - e.Delta * scrollSpeedFactor / 3
                );
            }
        }
        #endregion

        #region Добавление кнопок

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            int index = ButtonsContainer.Children.Count; 
            int row = index / 3;
            int col = (index % 3) * 2; 

            while (ButtonsContainer.RowDefinitions.Count <= row)
            {
                ButtonsContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            var button = new Button
            {
                Content = $"Diamond Graph",
                Style = (Style)FindResource("RoundedButton"),
                Margin = new Thickness(5),
                Padding = new Thickness(10),
                Height = 100,
            };

            Grid.SetRow(button, row);
            Grid.SetColumn(button, col);
            button.Click += DynamicButton_Click;

            ButtonsContainer.Children.Add(button);

            SaveButtonData(new CustomButton
            {
                Name = button.Content.ToString(),
                Row = row,
                Column = col
            });

            _buttonCounter++;
        }

        private void DynamicButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn) MessageBox.Show($"Нажата кнопка: {btn.Content}", "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Сохранение и загрузка

        private void SaveButtonData(CustomButton btn)
        {
            try
            {
                File.AppendAllLines(ButtonsFilePath, new[] { btn.ToString() });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void LoadButtons()
        {
            if (!File.Exists(ButtonsFilePath)) return;

            try
            {
                var lines = File.ReadAllLines(ButtonsFilePath);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    try
                    {
                        var data = CustomButton.FromString(line);
                        while (ButtonsContainer.RowDefinitions.Count <= data.Row)
                        {
                            ButtonsContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        }

                        var button = new Button
                        {
                            Content = data.Name,
                            Style = (Style)FindResource("RoundedButton"),
                            Margin = new Thickness(5),
                            Padding = new Thickness(10),
                            Height = 100,
                        };
                        Grid.SetRow(button, data.Row);
                        Grid.SetColumn(button, data.Column);
                        button.Click += DynamicButton_Click;

                        ButtonsContainer.Children.Add(button);
                        _buttonCounter = Math.Max(_buttonCounter, ButtonsContainer.Children.Count + 1);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            try
            {
                var lines = new System.Collections.Generic.List<string>();
                for (int i = 0; i < ButtonsContainer.Children.Count; i++)
                {
                    if (ButtonsContainer.Children[i] is Button btn)
                    {
                        int row = Grid.GetRow(btn);
                        int col = Grid.GetColumn(btn);
                        lines.Add(new CustomButton
                        {
                            Name = btn.Content.ToString(),
                            Row = row,
                            Column = col
                        }.ToString());
                    }
                }
                File.WriteAllLines(ButtonsFilePath, lines);
            }
            catch { }
            base.OnClosed(e);
        }

        #endregion
    }
}