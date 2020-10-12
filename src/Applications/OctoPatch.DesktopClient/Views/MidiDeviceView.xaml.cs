using System.Windows.Controls;
using OctoPatch.DesktopClient.Models;

namespace OctoPatch.DesktopClient.Views
{
    /// <summary>
    /// Interaction logic for MidiDeviceView.xaml
    /// </summary>
    [ConfigurationMap("12ea0035-45af-4da8-8b5d-e1b9d9484ba4:MidiDeviceNode", typeof(MidiDeviceModel))]
    public partial class MidiDeviceView : UserControl
    {
        public MidiDeviceView()
        {
            InitializeComponent();
        }
    }
}
