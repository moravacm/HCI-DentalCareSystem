using System.Globalization;
using System.Windows.Data;

namespace DentalCareSystem.Util
{
    public class LowStockConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] == null || values[1] == null)
            {
                return false;
            }

            if (decimal.TryParse(values[0].ToString(), out decimal currentStock) && decimal.TryParse(values[1].ToString(), out decimal minimumStock))
            {
                return currentStock < minimumStock;
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}