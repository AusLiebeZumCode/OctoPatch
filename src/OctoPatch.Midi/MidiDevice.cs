using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using RtMidi.Core;
using RtMidi.Core.Devices;
using RtMidi.Core.Messages;

namespace OctoPatch.Midi
{
    /// <summary>
    /// Represents a single midi device
    /// </summary>
    public sealed class MidiDevice : Node<DeviceConfiguration>
    {
        private readonly Subject<MidiMessage> _output;

        private IMidiInputDevice _device;

        public MidiDevice(Guid nodeId) : base(nodeId)
        {
            _output = new Subject<MidiMessage>();
            _outputs.Add(new OutputConnector<MidiMessage>(_output, Guid.Parse("{3148D6F6-48CC-42D6-8A69-49536BDB2F8E}")));
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
            _output.OnNext(new MidiMessage
            {
                MessageType = 3,
                Channel = (int)msg.Channel,
                Key = msg.Control,
                Value = msg.Value,
            });
        }

        private void DeviceOnNoteOff(IMidiInputDevice sender, in NoteOffMessage msg)
        {
            _output.OnNext(new MidiMessage
            {
                MessageType = 1,
                Channel = (int)msg.Channel,
                Key = (int)msg.Key,
                Value = msg.Velocity,
            });
        }

        private void DeviceOnNoteOn(IMidiInputDevice sender, in NoteOnMessage msg)
        {
            _output.OnNext(new MidiMessage
            {
                MessageType = 2,
                Channel = (int)msg.Channel,
                Key = (int)msg.Key,
                Value = msg.Velocity
            });
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
