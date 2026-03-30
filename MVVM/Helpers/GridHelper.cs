using System.Windows;
using System.Windows.Controls;

namespace Battleship.MVVM.Helpers
{
    public static class GridHelper
    {
        public static int GetRows(DependencyObject obj)
            => (int)obj.GetValue(RowsProperty);

        public static void SetRows(DependencyObject obj, int value)
            => obj.SetValue(RowsProperty, value);

        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.RegisterAttached(
                "Rows",
                typeof(int),
                typeof(GridHelper),
                new PropertyMetadata(0, OnRowsChanged));

        private static void OnRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Grid grid)
            {
                grid.RowDefinitions.Clear();

                int rows = (int)e.NewValue;
                for (int i = 0; i < rows; i++)
                {
                    grid.RowDefinitions.Add(new RowDefinition());
                }
            }
        }

        public static int GetColumns(DependencyObject obj)
            => (int)obj.GetValue(ColumnsProperty);

        public static void SetColumns(DependencyObject obj, int value)
            => obj.SetValue(ColumnsProperty, value);

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.RegisterAttached(
                "Columns",
                typeof(int),
                typeof(GridHelper),
                new PropertyMetadata(0, OnColumnsChanged));

        private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Grid grid)
            {
                grid.ColumnDefinitions.Clear();

                int cols = (int)e.NewValue;
                for (int i = 0; i < cols; i++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                }
            }
        }
    }
}
