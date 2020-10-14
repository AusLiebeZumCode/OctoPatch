using OctoPatch.Plugin.Midi;

namespace OctoPatch.DesktopClient.Models
{
    /// <summary>
    /// Configuration model for all kind of attached nodes of the MIDI Device
    /// </summary>
    public sealed class MidiAttachedNodeModel : NodeConfigurationModel<AttachedNodeConfiguration, EmptyEnvironment>
    {
        private int _channel;

        /// <summary>
        /// Gets or sets the channel of the message filter
        /// </summary>
        public int Channel
        {
            get => _channel;
            set
            {
                _channel = value;
                OnPropertyChanged();
            }
        }

        private int _key;

        /// <summary>
        /// Gets or sets the key of the message filter
        /// </summary>
        public int Key
        {
            get => _key;
            set
            {
                _key = value;
                OnPropertyChanged();
            }
        }

        protected override void OnSetup(EmptyEnvironment environment)
        {
        }

        protected override void OnSetConfiguration(AttachedNodeConfiguration configuration)
        {
            Channel = configuration?.Channel ?? 0;
            Key = configuration?.Key ?? 0;
        }

        protected override AttachedNodeConfiguration OnGetConfiguration()
        {
            return new AttachedNodeConfiguration
            {
                Channel = Channel,
                Key = Key
            };
        }
    }
}
