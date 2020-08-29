using System;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;

namespace OctoPatch.Plugin.Midi
{
    public sealed class MidiMessageFilter : Node<MidiMessageFilter.FilterConfiguration, IEnvironment>
    {
        #region Type description

        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription NodeDescription => NodeDescription.Create<MidiMessageFilter>(
                Guid.Parse(MidiPlugin.PluginId),
                "MIDI filter",
                "This block allows to filter the stream of midi messages")
            .AddInputDescription(MidiInputDescription)
            .AddOutputDescription(MidiOutputDescription);

        /// <summary>
        /// Description of the MIDI input connector
        /// </summary>
        public static ConnectorDescription MidiInputDescription => 
            new ConnectorDescription("MidiInput", "MIDI Input", "MIDI input signal", 
                ComplexContentType.Create<MidiMessage>(Guid.Parse(MidiPlugin.PluginId)));

        /// <summary>
        /// Description of the MIDI output connector
        /// </summary>
        public static ConnectorDescription MidiOutputDescription => 
            new ConnectorDescription("MidiOutput", "MIDI Output", "MIDI output signal", 
                ComplexContentType.Create<MidiMessage>(Guid.Parse(MidiPlugin.PluginId)));

        #endregion

        private FilterConfiguration _configuration;

        private readonly IOutputConnectorHandler _output;

        public MidiMessageFilter(Guid nodeId) : base(nodeId)
        {
            _output = RegisterOutputConnector(MidiOutputDescription);

            // StreamNote: PatteKi "und auch hier war ich :D" (2020-06-30 22:25)
            // StreamNote: m4cx: "Da war ich auch dran ;)" (2020-06-30 22:26)

            RegisterInputConnector(MidiInputDescription)
                .Handle<MidiMessage>(Handle);
        }

        private void Handle(MidiMessage message)
        {
            if (Valid(message))
            {
                _output.Send(message);
            }
        }

        private bool Valid(MidiMessage message)
        {
            if (_configuration != null)
            {
                if (_configuration.MessageType.HasValue && _configuration.MessageType != message.MessageType)
                    return false;

                if (_configuration.Channel.HasValue && _configuration.Channel != message.Channel)
                    return false;

                if (_configuration.Key.HasValue && _configuration.MessageType != message.MessageType)
                    return false;

                if (_configuration.MessageType.HasValue && _configuration.MessageType != message.MessageType)
                    return false;
            }

            return true;
        }

        public sealed class FilterConfiguration : IConfiguration
        {
            public int? MessageType { get; set; }

            public int? Channel { get; set; }

            public int? Key { get; set; }

            public int? Value { get; set; }
        }

        protected override Task OnInitialize(FilterConfiguration configuration, CancellationToken cancellationToken)
        {
            _configuration = configuration;
            return Task.CompletedTask;
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
    }
}
