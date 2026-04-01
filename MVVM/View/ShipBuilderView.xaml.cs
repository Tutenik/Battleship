using Battleship.MVVM.ViewModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Battleship.MVVM.View
{
    /// <summary>
    /// Interaction logic for ShipBuilderView.xaml
    /// </summary>
    public partial class ShipBuilderView : UserControl
    {
        public ShipBuilderView()
        {
            InitializeComponent();
        }
    }

    public class ToolToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return Binding.DoNothing;

            if ((bool)value)
                return Enum.Parse(targetType, parameter.ToString());

            return Enum.Parse(targetType, "None");
        }
    }
}
