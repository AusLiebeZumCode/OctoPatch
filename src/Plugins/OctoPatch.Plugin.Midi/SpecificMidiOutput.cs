using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;

namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Attached node for a specific output type
    /// </summary>
    public sealed class SpecificMidiOutput : AttachedNode<SpecificMidiOutput.NodeConfiguration, EmptyEnvironment, MidiDeviceNode>
    {
        #region Type description

        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription NodeDescription => AttachedNodeDescription.CreateAttached<SpecificMidiOutput, MidiDeviceNode>(
                Guid.Parse(MidiPlugin.PluginId),
                "Specific MIDI Output",
                "This is our first plugin to see how it works")
            .AddOutputDescription(OutputDescription);

        /// <summary>
        /// Description of the output connector
        /// </summary>
        public static ConnectorDescription OutputDescription => new ConnectorDescription(
            "Output", "Output", "output signal", 
            IntegerContentType.Create(minimumValue: 0, maximumValue: 127));

        #endregion

        private readonly IOutputConnectorHandler _output;

        private readonly IOutputConnector _input;

        private IDisposable _subscription;

        protected override NodeConfiguration DefaultConfiguration => new NodeConfiguration();

        public SpecificMidiOutput(Guid nodeId, MidiDeviceNode parentNode) 
            : base(nodeId, parentNode)
        {
            _input = parentNode.Outputs
                .First(o => o.Key == MidiDeviceNode.MidiOutputDescription.Key);

            _output = RegisterOutputConnector(OutputDescription);
        }

        protected override Task OnInitialize(NodeConfiguration configuration, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnStart(CancellationToken cancellationToken)
        {
            _subscription = _input.Subscribe<MidiMessage>(Handle);
            return Task.CompletedTask;
        }

        private void Handle(MidiMessage message)
        {
            // Send out only configured messages
            if (message.Channel == Configuration.Channel && 
                message.MessageType == Configuration.MessageType &&
                message.Key == Configuration.Key)
            {
                _output.Send(message.Value);
            }
        }

        protected override Task OnStop(CancellationToken cancellationToken)
        {
            _subscription?.Dispose();
            return Task.CompletedTask;
        }

        protected override Task OnDeinitialize(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnInitializeReset(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnReset(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #region Nested configuration

        /// <summary>
        /// Filter configuration
        /// </summary>
        public sealed class NodeConfiguration : IConfiguration
        {
            public int MessageType { get; set; }

            public int Channel { get; set; }

            public int Key { get; set; }
        }

        #endregion
    }
}
