using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;

namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Attached note to monitor a note message
    /// </summary>
    public sealed class NoteMidiOutput : AttachedNode<AttachedNodeConfiguration, EmptyEnvironment, MidiDeviceNode>
    {
        #region Type description

        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription NodeDescription => AttachedNodeDescription.CreateAttached<NoteMidiOutput, MidiDeviceNode>(
                Guid.Parse(MidiPlugin.PluginId),
                "Note MIDI Output",
                "Output for a specific note message")
            .AddOutputDescription(ValueOutputDescription)
            .AddOutputDescription(FlagOutputDescription)
            .AddOutputDescription(EnabledOutputDescription)
            .AddOutputDescription(DisabledOutputDescription);

        /// <summary>
        /// Description of the value output connector
        /// </summary>
        public static ConnectorDescription ValueOutputDescription => new ConnectorDescription(
            "Value", "Value Output", "value output signal", 
            IntegerContentType.Create(minimumValue: 0, maximumValue: 127));

        /// <summary>
        /// Description of the flag output connector
        /// </summary>
        public static ConnectorDescription FlagOutputDescription => new ConnectorDescription(
            "Flag", "Output", "flag output signal", 
            new BoolContentType());

        /// <summary>
        /// Description of the enable trigger
        /// </summary>
        public static ConnectorDescription EnabledOutputDescription => new ConnectorDescription(
            "Enabled", "Enabled trigger", "Trigger when enabled", 
            new EmptyContentType());

        /// <summary>
        /// Description of the disable trigger
        /// </summary>
        public static ConnectorDescription DisabledOutputDescription => new ConnectorDescription(
            "Disabled", "Disabled trigger", "Trigger when disabled", 
            new EmptyContentType());

        #endregion

        private readonly IOutputConnectorHandler _valueOutput;

        private readonly IOutputConnectorHandler _flagOutput;

        private readonly IOutputConnectorHandler _enabledOutput;

        private readonly IOutputConnectorHandler _disabledOutput;

        private readonly IOutputConnector _input;

        private IDisposable _subscription;

        protected override AttachedNodeConfiguration DefaultConfiguration => new AttachedNodeConfiguration();

        public NoteMidiOutput(Guid nodeId, MidiDeviceNode parentNode) : base(nodeId, parentNode)
        {
            _input = parentNode.Outputs
                .First(o => o.Key == MidiDeviceNode.MidiOutputDescription.Key);

            _valueOutput = RegisterOutputConnector<int>(ValueOutputDescription);
            _flagOutput = RegisterOutputConnector<bool>(FlagOutputDescription);
            _enabledOutput = RegisterOutputConnector(EnabledOutputDescription);
            _disabledOutput = RegisterOutputConnector(DisabledOutputDescription);
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
                _valueOutput.Send(message.Value);
            }
        }

        protected override Task OnStop(CancellationToken cancellationToken)
        {
            _subscription?.Dispose();
            return Task.CompletedTask;
        }
    }
}
