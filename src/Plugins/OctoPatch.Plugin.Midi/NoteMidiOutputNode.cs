using System;
using OctoPatch.Descriptions;

namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Attached note to monitor a note message
    /// </summary>
    public sealed class NoteMidiOutputNode : AttachedOutputNode
    {
        #region Type description

        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription NodeDescription => AttachedNodeDescription.CreateAttached<NoteMidiOutputNode, MidiDeviceNode>(
                Guid.Parse(MidiPlugin.PluginId),
                "Note MIDI Output",
                "Output for a specific note message")
            .AddOutputDescription(ValueOutputDescription)
            .AddOutputDescription(FlagOutputDescription)
            .AddOutputDescription(EnabledOutputDescription)
            .AddOutputDescription(DisabledOutputDescription);

        #endregion

        public NoteMidiOutputNode(Guid nodeId, MidiDeviceNode parentNode) : base(nodeId, parentNode)
        {
        }

        /// <inheritdoc />
        protected override int? OnHandle(MidiMessage message)
        {
            switch (message.MessageType)
            {
                case MidiDeviceNode.NoteOnMessageType:
                    // Take value of NoteOn
                    return message.Value;
                case MidiDeviceNode.NoteOffMessageType:
                    // Disable when NoteOff was sent
                    return 0;
                default:
                    return null;
            }
        }
    }
}
