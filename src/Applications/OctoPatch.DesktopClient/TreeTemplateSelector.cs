using System.Windows;
using System.Windows.Controls;
using OctoPatch.DesktopClient.Models;

namespace OctoPatch.DesktopClient
{
    public sealed class TreeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CommonNodeTemplate { get; set; }

        public DataTemplate AttachedNodeTemplate { get; set; }

        public DataTemplate SplitterNodeTemplate { get; set; }

        public DataTemplate CollectorNodeTemplate { get; set; }

        public DataTemplate OutputTemplate { get; set; }

        public DataTemplate InputTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch (item)
            {
                case CommonNodeModel _:
                    return CommonNodeTemplate;
                case AttachedNodeModel _:
                    return AttachedNodeTemplate;
                case SplitterNodeModel _:
                    return SplitterNodeTemplate;
                case CollectorNodeModel _:
                    return CollectorNodeTemplate;
                case OutputNodeModel _:
                    return OutputTemplate;
                case InputNodeModel _:
                    return InputTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
