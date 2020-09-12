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
    public sealed class MidiDeviceNode : Node<MidiDeviceNode.MidiDeviceConfiguration, MidiDeviceNode.MidiDeviceEnvironment>
    {
        #region Type description

        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription NodeDescription => CommonNodeDescription.Create<MidiDeviceNode>(
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

        private IMidiInputDevice _inputDevice;

        private IMidiOutputDevice _outputDevice;

        public MidiDeviceNode(Guid nodeId) : base(nodeId)
        {
            _output = RegisterOutputConnector(MidiOutputDescription);
            RegisterInputConnector(MidiInputDescription).Handle<MidiMessage>(HandleMessage);

            // Build environment
            UpdateEnvironment(new MidiDeviceEnvironment
            {
                InputDevices = MidiDeviceManager.Default.InputDevices.Select(d => d.Name).ToList(),
                OutputDevices = MidiDeviceManager.Default.OutputDevices.Select(d => d.Name).ToList()
            });
        }

        private void HandleMessage(MidiMessage message)
        {
            if (State == NodeState.Running)
            {
                // TODO: Send out
                // _outputDevice?.Send()
            }
        }

        protected override Task OnInitialize(MidiDeviceConfiguration configuration, CancellationToken cancellationToken)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var inputDeviceInfo = MidiDeviceManager.Default.InputDevices.FirstOrDefault(d => d.Name.StartsWith(configuration.InputDeviceName));
            if (inputDeviceInfo != null)
            {
                _inputDevice = inputDeviceInfo.CreateDevice();

                _inputDevice.NoteOn += DeviceOnNoteOn;
                _inputDevice.NoteOff += DeviceOnNoteOff;
                _inputDevice.ControlChange += DeviceOnControlChange;

            }

            var outputDeviceInfo = MidiDeviceManager.Default.OutputDevices.FirstOrDefault(d => d.Name.StartsWith(configuration.OutputDeviceName));
            if (outputDeviceInfo != null)
            {
                _outputDevice = outputDeviceInfo.CreateDevice();
            }

            return Task.CompletedTask;
        }

        protected override Task OnStart(CancellationToken cancellationToken)
        {
            _inputDevice?.Open();
            _outputDevice?.Open();
            return Task.CompletedTask;
        }

        protected override Task OnStop(CancellationToken cancellationToken)
        {
            _inputDevice?.Close();
            _outputDevice?.Close();
            return Task.CompletedTask;
        }

        protected override Task OnDeinitialize(CancellationToken cancellationToken)
        {
            _inputDevice?.Dispose();
            _outputDevice?.Dispose();
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

        #region nested classes

        /// <summary>
        /// Configuration set for the MIDI Devices
        /// </summary>
        public sealed class MidiDeviceConfiguration : IConfiguration
        {
            /// <summary>
            /// Gets or sets the name of the input device that's represented
            /// </summary>
            public string InputDeviceName { get; set; }

            /// <summary>
            /// Gets or sets the name of the output device that's represented
            /// </summary>
            public string OutputDeviceName { get; set; }
        }

        /// <summary>
        /// Environment of a MIDI device
        /// </summary>
        public sealed class MidiDeviceEnvironment : IEnvironment
        {
            /// <summary>
            /// List of available input devices
            /// </summary>
            public List<string> InputDevices { get; set; }

            /// <summary>
            /// List of available output devices
            /// </summary>
            public List<string> OutputDevices { get; set; }
        }

        #endregion
    }
}
