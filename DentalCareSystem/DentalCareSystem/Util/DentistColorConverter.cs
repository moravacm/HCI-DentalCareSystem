using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DentalCareSystem.Util
{
    public class DentistColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string dentistJMBG && parameter is Dictionary<string, SolidColorBrush> dentistColors)
            {
                if (dentistColors.ContainsKey(dentistJMBG))
                {
                    return dentistColors[dentistJMBG];
                }
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}