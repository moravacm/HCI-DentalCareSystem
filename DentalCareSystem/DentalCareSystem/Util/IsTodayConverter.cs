﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace DentalCareSystem.Util
{
    public class IsTodayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                return date.Date == DateTime.Today.Date;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}