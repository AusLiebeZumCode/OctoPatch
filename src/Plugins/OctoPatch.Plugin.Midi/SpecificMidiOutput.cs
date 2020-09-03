using System;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;

namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Attached node for a specific output type
    /// </summary>
    public sealed class SpecificMidiOutput : AttachedNode<SpecificMidiOutput.NodeConfiguration, IEnvironment, MidiDevice>
    {
        #region Type description

        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription NodeDescription => AttachedNodeDescription.CreateAttached<SpecificMidiOutput, MidiDevice>(
                Guid.Parse(MidiPlugin.PluginId),
                "Specific MIDI Output",
                "This is our first plugin to see how it works")
            .AddOutputDescription(MidiOutputDescription);

        /// <summary>
        /// Description of the MIDI output connector
        /// </summary>
        public static ConnectorDescription MidiOutputDescription => new ConnectorDescription(
            "MidiOutput", "MIDI Output", "MIDI output signal", 
            ComplexContentType.Create<MidiMessage>(Guid.Parse(MidiPlugin.PluginId)));

        #endregion

        public SpecificMidiOutput(Guid nodeId, MidiDevice parentNode) : base(nodeId, parentNode)
        {
        }

        protected override Task OnInitialize(NodeConfiguration configuration, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task OnStart(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task OnStop(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task OnDeinitialize(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task OnInitializeReset(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task OnReset(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #region Nested configuration

        public sealed class NodeConfiguration : IConfiguration
        {

        }

        #endregion
    }
}
