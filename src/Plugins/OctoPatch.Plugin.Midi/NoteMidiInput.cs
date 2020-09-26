
using System;

namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Attached note for receiving note input
    /// </summary>
    public sealed class NoteMidiInput : AttachedNode<AttachedNodeConfiguration, EmptyEnvironment, MidiDeviceNode>
    {
        public NoteMidiInput(Guid nodeId, MidiDeviceNode parentNode) : base(nodeId, parentNode)
        {
        }

        protected override AttachedNodeConfiguration DefaultConfiguration => new AttachedNodeConfiguration();
    }
}
