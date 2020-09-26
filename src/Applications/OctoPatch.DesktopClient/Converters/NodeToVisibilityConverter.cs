using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using OctoPatch.DesktopClient.Models;

namespace OctoPatch.DesktopClient.Converters
{
    /// <summary>
    /// Converts selected tree node type into visibility
    /// </summary>
    [ValueConversion(typeof(NodeModel), typeof(Visibility))]
    class NodeToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// show up when common node is selected
        /// </summary>
        public bool CommonNode { get; set; }

        /// <summary>
        /// show up when attached node is selected
        /// </summary>
        public bool AttachedNode { get; set; }

        /// <summary>
        /// show up when splitter node is selected
        /// </summary>
        public bool SplitterNode { get; set; }

        /// <summary>
        /// show up when collector node is selected
        /// </summary>
        public bool CollectorNode { get; set; }

        /// <summary>
        /// show up when input connector is selected
        /// </summary>
        public bool InputConnector { get; set; }

        /// <summary>
        /// show up when output connector is selected
        /// </summary>
        public bool OutputConnector { get; set; }

        /// <summary>
        /// show up when wire is selected
        /// </summary>
        public bool Wire { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visible = false;

            if (CommonNode && value is CommonNodeModel)
            {
                visible = true;
            }

            if (AttachedNode && value is AttachedNodeModel)
            {
                visible = true;
            }

            if (SplitterNode && value is SplitterNodeModel)
            {
                visible = true;
            }

            if (CollectorNode && value is CollectorNodeModel)
            {
                visible = true;
            }

            if (InputConnector && value is InputNodeModel)
            {
                visible = true;
            }

            if (OutputConnector && value is OutputNodeModel)
            {
                visible = true;
            }

            if (Wire && value is WireNodeModel)
            {
                visible = true;
            }

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
