using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;
using RtMidi.Core;
using RtMidi.Core.Devices;
using RtMidi.Core.Messages;

namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Represents a single midi device
    /// </summary>
    public sealed class MidiDevice : Node<DeviceConfiguration, IEnvironment>
    {
        #region Type description

        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription NodeDescription => NodeDescription.Create<MidiDevice>(
                Guid.Parse(MidiPlugin.PluginId),
                "MIDI Device",
                "This is our first plugin to see how it works")
            .AddInputDescription(MidiInputDescription)
            .AddOutputDescription(MidiOutputDescription);

        /// <summary>
        /// Description of the MIDI input connector
        /// </summary>
        public static ConnectorDescription MidiInputDescription => new ConnectorDescription(
            "MidiInput", "MIDI Input", "MIDI input signal", 
            ComplexContentType.Create<MidiMessage>(Guid.Parse(MidiPlugin.PluginId)));

        /// <summary>
        /// Description of the MIDI output connector
        /// </summary>
        public static ConnectorDescription MidiOutputDescription => new ConnectorDescription(
            "MidiOutput", "MIDI Output", "MIDI output signal", 
            ComplexContentType.Create<MidiMessage>(Guid.Parse(MidiPlugin.PluginId)));

        #endregion

        private readonly IOutputConnectorHandler _output;

        private IMidiInputDevice _device;

        public MidiDevice(Guid nodeId) : base(nodeId)
        {
            _output = RegisterOutputConnector(MidiOutputDescription);
        }

        protected override Task OnInitialize(DeviceConfiguration configuration, CancellationToken cancellationToken)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var x = MidiDeviceManager.Default.InputDevices.ToArray();

            var deviceInfo = MidiDeviceManager.Default.InputDevices.FirstOrDefault(d => d.Name.StartsWith(configuration.DeviceName));
            if (deviceInfo == null)
            {
                throw new ArgumentException("configured device is not available", nameof(configuration));
            }

            _device = deviceInfo.CreateDevice();
            _device.NoteOn += DeviceOnNoteOn;
            _device.NoteOff += DeviceOnNoteOff;
            _device.ControlChange += DeviceOnControlChange;

            return Task.CompletedTask;
        }

        protected override Task OnStart(CancellationToken cancellationToken)
        {
            _device.Open();
            return Task.CompletedTask;
        }

        protected override Task OnStop(CancellationToken cancellationToken)
        {
            _device.Close();
            return Task.CompletedTask;
        }

        protected override Task OnDeinitialize(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task OnInitializeReset(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnReset(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void DeviceOnControlChange(IMidiInputDevice sender, in ControlChangeMessage msg)
        {
            _output.Send(new MidiMessage(3, (int)msg.Channel, msg.Control, msg.Value));
        }

        private void DeviceOnNoteOff(IMidiInputDevice sender, in NoteOffMessage msg)
        {
            _output.Send(new MidiMessage(1, (int)msg.Channel, (int)msg.Key, msg.Velocity));
        }

        private void DeviceOnNoteOn(IMidiInputDevice sender, in NoteOnMessage msg)
        {
            _output.Send(new MidiMessage(2, (int)msg.Channel, (int)msg.Key, msg.Velocity));
        }

        #region static helper methods

        /// <summary>
        /// Returns a list of available input devices
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetDevices()
        {
            return MidiDeviceManager.Default.InputDevices.Select(d => d.Name);
        }

        #endregion
    }
}
