using System;
using OctoPatch.Descriptions;

namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Attached note to monitor a control message
    /// </summary>
    public sealed class ControlMidiOutputNode : AttachedOutputNode
    {
        #region Type description

        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription NodeDescription => AttachedNodeDescription.CreateAttached<ControlMidiOutputNode, MidiDeviceNode>(
                Guid.Parse(MidiPlugin.PluginId),
                "Control MIDI Output",
                "Output for a specific control message")
            .AddOutputDescription(ValueOutputDescription)
            .AddOutputDescription(FlagOutputDescription)
            .AddOutputDescription(EnabledOutputDescription)
            .AddOutputDescription(DisabledOutputDescription);

        #endregion

        public ControlMidiOutputNode(Guid nodeId, MidiDeviceNode parentNode) : base(nodeId, parentNode)
        {
            
        }

        /// <inheritdoc />
        protected override int? OnHandle(MidiMessage message)
        {
            // Only handles message type 3 (control changed)
            if (message.MessageType == MidiDeviceNode.ControlChangedMessageType)
            {
                return message.Value;
            }

            return null;
        }
    }
}
