using System.Collections.ObjectModel;
using OctoPatch.Plugin.Midi;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class MidiDeviceModel : NodeConfigurationModel<MidiDeviceConfiguration, MidiDeviceEnvironment>
    {
        public ObservableCollection<string> Devices { get; }

        private string _selectedDevice;

        public string SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                OnPropertyChanged();
            }
        }

        public MidiDeviceModel()
        {
            Devices = new ObservableCollection<string>();
        }

        protected override void OnSetup(MidiDeviceEnvironment environment)
        {
            Devices.Clear();
            foreach (var device in environment.Devices)
            {
                Devices.Add(device);
            }
        }

        protected override void OnSetConfiguration(MidiDeviceConfiguration configuration)
        {
            SelectedDevice = configuration.DeviceName;
        }

        protected override MidiDeviceConfiguration OnGetConfiguration()
        {
            return new MidiDeviceConfiguration
            {
                DeviceName = SelectedDevice
            };
        }
    }
}
