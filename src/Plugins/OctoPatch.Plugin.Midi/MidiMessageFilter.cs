using System;
using System.Threading;
using System.Threading.Tasks;

namespace OctoPatch.Plugin.Midi
{
    public sealed class MidiMessageFilter : Node<MidiMessageFilter.FilterConfiguration>
    {
        #region Type description

        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription NodeDescription => NodeDescription.Create<MidiMessageFilter>(
                Guid.Parse("{01F4AF51-E88D-401E-B02D-4D8B1164039C}"),
                "MIDI message filter",
                new Version(1, 0, 0), 
                "This block allows to filter the stream of midi messages")
            .AddInputDescription(MidiInputDescription)
            .AddOutputDescription(MidiOutputDescription);

        /// <summary>
        /// Description of the MIDI input connector
        /// </summary>
        public static InputDescription MidiInputDescription => new InputDescription(
            Guid.Parse("{BF46B8E6-B376-4C58-86C8-33796ABE9CE4}"),
            "MIDI input",
            ComplexContentType.Create<MidiMessage>());

        /// <summary>
        /// Description of the MIDI output connector
        /// </summary>
        public static OutputDescription MidiOutputDescription => new OutputDescription(
            Guid.Parse("{9A745B45-0D66-4F38-83E9-F2FAECC79F63}"),
            "MIDI Output",
            ComplexContentType.Create<MidiMessage>());

        #endregion

        private FilterConfiguration _configuration;

        private readonly IOutputConnector _output;

        public MidiMessageFilter(Guid nodeId) : base(nodeId)
        {
            _output = RegisterOutputConnector(MidiOutputDescription);

            // StreamNote: PatteKi "und auch hier war ich :D" (2020-06-30 22:25)
            // StreamNote: m4cx: "Da war ich auch dran ;)" (2020-06-30 22:26)

            RegisterInputConnector(MidiInputDescription)
                .HandleComplex<MidiMessage>(Handle);
        }

        private void Handle(MidiMessage message)
        {
            if (Valid(message))
            {
                _output.SendComplex(message);
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

        public sealed class FilterConfiguration : INodeConfiguration
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
