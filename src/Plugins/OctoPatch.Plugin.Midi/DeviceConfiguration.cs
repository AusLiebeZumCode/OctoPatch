namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Configuration set for the MIDI Devices
    /// </summary>
    public sealed class DeviceConfiguration : IConfiguration
    {
        /// <summary>
        /// Gets or sets the name of the device that's represented
        /// </summary>
        public string DeviceName { get; set; }
    }
}
