using System.Windows;
using System.Windows.Controls;
using OctoPatch.DesktopClient.Models;

namespace OctoPatch.DesktopClient
{
    public sealed class NodeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MidiDeviceTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is MidiDeviceModel)
            {
                return MidiDeviceTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
