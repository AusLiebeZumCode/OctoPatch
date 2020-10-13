using System.Windows.Controls;
using OctoPatch.DesktopClient.Models;
using OctoPatch.Plugin.Midi;

namespace OctoPatch.DesktopClient.Views
{
    /// <summary>
    /// Interaction logic for MidiAttachedView.xaml
    /// </summary>
    [ConfigurationMap("12ea0035-45af-4da8-8b5d-e1b9d9484ba4:ControlMidiOutputNode", typeof(MidiAttachedNodeModel))]
    [ConfigurationMap("12ea0035-45af-4da8-8b5d-e1b9d9484ba4:ControlMidiInputNode", typeof(MidiAttachedNodeModel))]
    [ConfigurationMap("12ea0035-45af-4da8-8b5d-e1b9d9484ba4:NoteMidiOutputNode", typeof(MidiAttachedNodeModel))]
    [ConfigurationMap("12ea0035-45af-4da8-8b5d-e1b9d9484ba4:NoteMidiInputNode", typeof(MidiAttachedNodeModel))]
    public partial class MidiAttachedView : UserControl
    {
        public MidiAttachedView()
        {
            InitializeComponent();
        }
    }
}
