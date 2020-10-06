using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;

namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Base class for all kind of filtered output nodes
    /// </summary>
    public abstract class AttachedOutputNode : AttachedNode<AttachedNodeConfiguration, EmptyEnvironment, MidiDeviceNode>
    {
        #region Type description

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

        /// <summary>
        /// Connector handler for raw value
        /// </summary>
        private readonly IOutputConnectorHandler _valueOutput;

        /// <summary>
        /// Connector handler for On/Off flag
        /// </summary>
        private readonly IOutputConnectorHandler _flagOutput;

        /// <summary>
        /// Connector handler for a enable trigger
        /// </summary>
        private readonly IOutputConnectorHandler _enabledOutput;

        /// <summary>
        /// Connector handler for a disabled trigger
        /// </summary>
        private readonly IOutputConnectorHandler _disabledOutput;

        /// <summary>
        /// Input connector
        /// </summary>
        private readonly IOutputConnector _input;

        /// <summary>
        /// Open subscription
        /// </summary>
        private IDisposable _subscription;
        
        /// <summary>
        /// Returns an empty configuration
        /// </summary>
        protected override AttachedNodeConfiguration DefaultConfiguration => new AttachedNodeConfiguration();

        protected AttachedOutputNode(Guid nodeId, MidiDeviceNode parentNode) : base(nodeId, parentNode)
        {
            _input = parentNode.Outputs
                .First(o => o.Key == MidiDeviceNode.MidiOutputDescription.Key);

            _valueOutput = RegisterOutputConnector<int>(ValueOutputDescription);
            _flagOutput = RegisterOutputConnector<bool>(FlagOutputDescription);
            _enabledOutput = RegisterOutputConnector(EnabledOutputDescription);
            _disabledOutput = RegisterOutputConnector(DisabledOutputDescription);
        }

        /// <inheritdoc />
        protected override Task OnStart(CancellationToken cancellationToken)
        {
            _subscription = _input.Subscribe<MidiMessage>(Handle);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task OnStop(CancellationToken cancellationToken)
        {
            _subscription?.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles a single message from the input node
        /// </summary>
        /// <param name="message"></param>
        private void Handle(MidiMessage message)
        {
            // Send out only configured messages
            if (message.Channel == Configuration.Channel && 
                message.Key == Configuration.Key)
            {
                var value = OnHandle(message);
                if (value.HasValue)
                {
                    _valueOutput.Send(value.Value);
                    _flagOutput.Send(value.Value > 0);
                    if (value.Value > 0)
                    {
                        _enabledOutput.Send();
                    }
                    else
                    {
                        _disabledOutput.Send();
                    }
                }
            }
        }

        /// <summary>
        /// Gets a call when a message meets the configured filter
        /// </summary>
        /// <param name="message">midi message</param>
        /// <returns>null if message does not meet the requirements, otherwise the value</returns>
        protected abstract int? OnHandle(MidiMessage message);
    }
}
