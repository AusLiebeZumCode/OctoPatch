using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OctoPatch.DesktopClient.Converters
{
    /// <summary>
    /// Checks if the given value is within the range
    /// </summary>
    [ValueConversion(typeof(int), typeof(Visibility))]
    public sealed class InRangeToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Minimum value
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        /// Maximum value
        /// </summary>
        public int MaxValue { get; set; }

        public InRangeToVisibilityConverter()
        {
            MinValue = int.MinValue;
            MaxValue = int.MaxValue;
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int input))
            {
                return Visibility.Collapsed;
            }

            return input >= MinValue && input <= MaxValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
