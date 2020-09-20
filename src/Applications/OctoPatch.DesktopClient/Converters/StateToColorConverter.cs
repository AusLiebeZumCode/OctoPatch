using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using Brush = System.Windows.Media.Brush;

namespace OctoPatch.DesktopClient.Converters
{
    [ValueConversion(typeof(NodeState), typeof(SolidBrush))]
    public sealed class StateToColorConverter : IValueConverter
    {
        public Brush Uninitialized { get; set; }
        
        public Brush Stopped { get; set; }
        
        public Brush Running { get; set; }

        public Brush Failed { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is NodeState))
            {
                return new SolidBrush(Color.Transparent);
            }

            switch ((NodeState)value)
            {
                case NodeState.Uninitialized: 
                case NodeState.Initializing:
                    return Uninitialized;
                case NodeState.Resetting:
                case NodeState.InitializationFailed:
                case NodeState.Failed:
                    return Failed;
                case NodeState.Stopped:
                case NodeState.Starting:
                case NodeState.Deinitializing:
                    return Stopped;
                case NodeState.Running:
                case NodeState.Stopping:
                    return Running;
                default:
                    return new SolidBrush(Color.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
