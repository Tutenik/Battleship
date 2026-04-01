using System.Globalization;
using System.Windows.Data;

namespace Battleship.MVVM.Converters
{
    public class AddConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double a = System.Convert.ToDouble(values[0]);
            double b = System.Convert.ToDouble(values[1]);
            return a + b;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
