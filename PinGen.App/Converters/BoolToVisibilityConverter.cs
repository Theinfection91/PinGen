using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PinGen.App.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = value is bool b && b;

            // Handle Opacity parameter for enabled/disabled visual feedback
            if (parameter is string paramStr && paramStr == "Opacity")
            {
                return boolValue ? 1.0 : 0.5;
            }

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string paramStr && paramStr == "Opacity")
            {
                return value is double d && d > 0.5;
            }

            return value is Visibility visibility && visibility == Visibility.Visible;
        }
    }
}