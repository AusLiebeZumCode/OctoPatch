using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using OctoPatch.DesktopClient.Models;

namespace OctoPatch.DesktopClient.Converters
{
    [ValueConversion(typeof(NodeModel), typeof(Visibility))]
    class NodeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is CommonNodeModel || value is AttachedNodeModel || value is SplitterNodeModel || value is CollectorNodeModel ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
