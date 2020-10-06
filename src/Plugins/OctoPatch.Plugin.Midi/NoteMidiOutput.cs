using System;
using OctoPatch.Descriptions;

namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Attached note to monitor a note message
    /// </summary>
    public sealed class NoteMidiOutput : AttachedOutputNode
    {
        #region Type description

        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription NodeDescription => AttachedNodeDescription.CreateAttached<NoteMidiOutput, MidiDeviceNode>(
                Guid.Parse(MidiPlugin.PluginId),
                "Note MIDI Output",
                "Output for a specific note message")
            .AddOutputDescription(ValueOutputDescription)
            .AddOutputDescription(FlagOutputDescription)
            .AddOutputDescription(EnabledOutputDescription)
            .AddOutputDescription(DisabledOutputDescription);

        #endregion

        public NoteMidiOutput(Guid nodeId, MidiDeviceNode parentNode) : base(nodeId, parentNode)
        {
        }

        /// <inheritdoc />
        protected override int? OnHandle(MidiMessage message)
        {
            if (message.MessageType == 1)
            {
                // Take value of NoteOn
                return message.Value;
            }
            else if (message.MessageType == 2)
            {
                // Disable when NoteOff was sent
                return 0;
            }

            return null;
        }
    }
}
