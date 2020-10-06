using System;
using OctoPatch.Descriptions;

namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Attached note to monitor a control message
    /// </summary>
    public sealed class ControlMidiOutput : AttachedOutputNode
    {
        #region Type description

        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription NodeDescription => AttachedNodeDescription.CreateAttached<ControlMidiOutput, MidiDeviceNode>(
                Guid.Parse(MidiPlugin.PluginId),
                "Control MIDI Output",
                "Output for a specific control message")
            .AddOutputDescription(ValueOutputDescription)
            .AddOutputDescription(FlagOutputDescription)
            .AddOutputDescription(EnabledOutputDescription)
            .AddOutputDescription(DisabledOutputDescription);

        #endregion

        public ControlMidiOutput(Guid nodeId, MidiDeviceNode parentNode) : base(nodeId, parentNode)
        {
            
        }

        protected override int? OnHandle(MidiMessage message)
        {
            // Only handles message type 3 (control changed)
            if (message.MessageType == 3)
            {
                return message.Value;
            }

            return null;
        }
    }
}
