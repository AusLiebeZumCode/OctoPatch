using System.Collections.ObjectModel;
using OctoPatch.Plugin.Midi;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class MidiDeviceModel : NodeConfigurationModel<MidiDeviceNode.MidiDeviceConfiguration, MidiDeviceNode.MidiDeviceEnvironment>
    {
        /// <summary>
        /// List of available input devices
        /// </summary>
        public ObservableCollection<string> InputDevices { get; }

        /// <summary>
        /// List of available output devices
        /// </summary>
        public ObservableCollection<string> OutputDevices { get; }

        private string _selectedInputDevice;

        /// <summary>
        /// Gets or sets the selected input device
        /// </summary>
        public string SelectedInputDevice
        {
            get => _selectedInputDevice;
            set
            {
                _selectedInputDevice = value;
                OnPropertyChanged();
            }
        }

        private string _selectedOutputDevice;

        /// <summary>
        /// Gets or sets the selected output device
        /// </summary>
        public string SelectedOutputDevice
        {
            get => _selectedOutputDevice;
            set
            {
                _selectedOutputDevice = value;
                OnPropertyChanged();
            }
        }

        public MidiDeviceModel()
        {
            InputDevices = new ObservableCollection<string>();
            OutputDevices = new ObservableCollection<string>();
        }

        protected override void OnSetup(MidiDeviceNode.MidiDeviceEnvironment environment)
        {
            InputDevices.Clear();
            foreach (var device in environment.InputDevices)
            {
                InputDevices.Add(device);
            }

            OutputDevices.Clear();
            foreach (var device in environment.OutputDevices)
            {
                OutputDevices.Add(device);
            }
        }

        protected override void OnSetConfiguration(MidiDeviceNode.MidiDeviceConfiguration configuration)
        {
            SelectedInputDevice = configuration.InputDeviceName;
            SelectedOutputDevice = configuration.OutputDeviceName;
        }

        protected override MidiDeviceNode.MidiDeviceConfiguration OnGetConfiguration()
        {
            return new MidiDeviceNode.MidiDeviceConfiguration
            {
                InputDeviceName = SelectedInputDevice,
                OutputDeviceName = SelectedOutputDevice,
            };
        }
    }
}
