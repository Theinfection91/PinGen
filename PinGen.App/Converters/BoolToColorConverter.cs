using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PinGen.App.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isEnabled = value is bool b && b;
            // Dark text when enabled, gray when disabled
            return isEnabled ? Brushes.Black : Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}