using System.Collections.Generic;

namespace OctoPatch.Plugin.Midi
{
    public sealed class MidiDeviceEnvironment : IEnvironment
    {
        /// <summary>
        /// List of available devices
        /// </summary>
        public List<string> Devices { get; set; }
    }
}
