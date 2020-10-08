using System.Windows;
using System.Windows.Controls;
using OctoPatch.DesktopClient.Models;

namespace OctoPatch.DesktopClient
{
    public sealed class NodeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MidiDeviceTemplate { get; set; }

        public DataTemplate RestGetTemplate { get; set; }

        public DataTemplate MidiAttachedTemplate { get; set; }

        public DataTemplate KeyboardStringTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                MidiDeviceModel _ => MidiDeviceTemplate,
                RestGetModel _ => RestGetTemplate,
                MidiAttachedNodeModel _ => MidiAttachedTemplate,
                KeyboardStringModel _ => KeyboardStringTemplate,
                _ => base.SelectTemplate(item, container)
            };
        }
    }
}
