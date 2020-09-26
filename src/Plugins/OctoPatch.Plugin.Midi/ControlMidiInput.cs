using System;

namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Attached note to receive input of a control channel
    /// </summary>
    public sealed class ControlMidiInput : AttachedNode<AttachedNodeConfiguration, EmptyEnvironment, MidiDeviceNode>
    {
        public ControlMidiInput(Guid nodeId, MidiDeviceNode parentNode) : base(nodeId, parentNode)
        {
        }

        protected override AttachedNodeConfiguration DefaultConfiguration => new AttachedNodeConfiguration();
    }
}
