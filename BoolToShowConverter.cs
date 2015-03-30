using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace HotMess
{
    public class BoolToShowConverter : IValueConverter
    {      
        public BoolToShowConverter()
        { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Hidden;
            Visibility vis = (bool)value ? Visibility.Visible : Visibility.Hidden;
            return vis;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }
}