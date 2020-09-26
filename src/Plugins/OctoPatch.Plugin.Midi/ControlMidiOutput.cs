using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;

namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Attached note to monitor a control message
    /// </summary>
    public sealed class ControlMidiOutput : AttachedNode<AttachedNodeConfiguration, EmptyEnvironment, MidiDeviceNode>
    {
        #region Type description

        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription NodeDescription => AttachedNodeDescription.CreateAttached<ControlMidiOutput, MidiDeviceNode>(
                Guid.Parse(MidiPlugin.PluginId),
                "Control MIDI Output",
                "Output for a specific control message")
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

        protected override AttachedNodeConfiguration DefaultConfiguration => new AttachedNodeConfiguration();
        
        public ControlMidiOutput(Guid nodeId, MidiDeviceNode parentNode) : base(nodeId, parentNode)
        {
            _input = parentNode.Outputs
                .First(o => o.Key == MidiDeviceNode.MidiOutputDescription.Key);

            _output = RegisterOutputConnector<int>(OutputDescription);
        }

        protected override Task OnStart(CancellationToken cancellationToken)
        {
            _subscription = _input.Subscribe<MidiMessage>(Handle);
            return Task.CompletedTask;
        }

        private void Handle(MidiMessage message)
        {
            // TODO: Make sure to filter for the right message type

            // Send out only configured messages
            if (message.Channel == Configuration.Channel && 
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
    }
}
