using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DetailsList.Infrastructure.Converters
{
    public class ModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var test = (FindMode)value;
            return (FindMode)value switch
            {
                FindMode.General => "Нормальный режим",
                FindMode.Mazak350 => "Mazak 350",
                _ => null,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
