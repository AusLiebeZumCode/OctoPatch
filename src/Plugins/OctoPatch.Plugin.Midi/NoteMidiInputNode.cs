using System;
using OctoPatch.Descriptions;

namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Attached note for receiving note input
    /// </summary>
    public sealed class NoteMidiInputNode : AttachedInputNode
    {
        #region Type description

        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription NodeDescription => AttachedNodeDescription.CreateAttached<NoteMidiInputNode, MidiDeviceNode>(
                Guid.Parse(MidiPlugin.PluginId),
                "Note MIDI Input",
                "Output for a specific note message")
            .AddInputDescription(ValueInputDescription)
            .AddInputDescription(FlagInputDescription)
            .AddInputDescription(EnabledInputDescription)
            .AddInputDescription(DisabledInputDescription);

        #endregion

        public NoteMidiInputNode(Guid nodeId, MidiDeviceNode parentNode) : base(nodeId, parentNode)
        {
        }

        /// <inheritdoc />
        protected override MidiMessage OnHandle(int value)
        {
            return value == 0 
                ? new MidiMessage(2, Configuration.Channel, Configuration.Key, 127) 
                : new MidiMessage(1, Configuration.Channel, Configuration.Key, value);
        }
    }
}
