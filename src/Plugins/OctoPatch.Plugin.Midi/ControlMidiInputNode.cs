using System;
using OctoPatch.Descriptions;

namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Attached note to receive input of a control channel
    /// </summary>
    public sealed class ControlMidiInputNode : AttachedInputNode
    {
        #region Type description

        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription NodeDescription => AttachedNodeDescription.CreateAttached<ControlMidiInputNode, MidiDeviceNode>(
                Guid.Parse(MidiPlugin.PluginId),
                "Control MIDI Input",
                "Input for a specific control message")
            .AddInputDescription(ValueInputDescription)
            .AddInputDescription(FlagInputDescription)
            .AddInputDescription(EnabledInputDescription)
            .AddInputDescription(DisabledInputDescription);

        #endregion

        public ControlMidiInputNode(Guid nodeId, MidiDeviceNode parentNode) : base(nodeId, parentNode)
        {
        }

        /// <inheritdoc />
        protected override MidiMessage OnHandle(int value)
        {
            return new MidiMessage(MidiDeviceNode.ControlChangedMessageType, Configuration.Channel, Configuration.Key, value);
        }
    }
}
