using System.Data;
using System.Globalization;
using System.Windows.Data;

namespace Battleship.MVVM.Converters
{
    public class ExpressionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null || parameter == null)
                    return value;

                string expr = parameter.ToString().Replace("x", value.ToString());

                var result = new DataTable().Compute(expr, null);
                return System.Convert.ChangeType(result, targetType);
            }
            catch
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
