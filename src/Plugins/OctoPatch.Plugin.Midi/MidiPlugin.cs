﻿using System;

namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Implementation for the MIDI Plugin
    /// </summary>
    public sealed class MidiPlugin : Server.Plugin
    {
        /// <summary>
        ///  Plugin id
        /// </summary>
        internal const string PluginId = "{12EA0035-45AF-4DA8-8B5D-E1B9D9484BA4}";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override Guid Id => Guid.Parse(PluginId);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override string Name => "MIDI";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override string Description => "Adds MIDI functionality";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override Version Version => new Version(1, 0, 0);

        public MidiPlugin()
        {
            RegisterType<MidiMessage>(MidiMessage.TypeDescription);

            RegisterNode<MidiDeviceNode>(MidiDeviceNode.NodeDescription);
            RegisterNode<NoteMidiOutputNode>(NoteMidiOutputNode.NodeDescription);
            RegisterNode<ControlMidiOutputNode>(ControlMidiOutputNode.NodeDescription);
            RegisterNode<NoteMidiInputNode>(NoteMidiInputNode.NodeDescription);
            RegisterNode<ControlMidiInputNode>(ControlMidiInputNode.NodeDescription);
        }

        protected override IAdapter OnCreateAdapter(Type type)
        {
            return null;
        }
    }
}
