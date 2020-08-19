using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Core;

namespace OctoPatch.Plugin.Midi
{
    public sealed class MidiMessageFilter : Node<MidiMessageFilter.FilterConfiguration>, IDisposable
    {
        private FilterConfiguration _configuration;

        private readonly Subject<MidiMessage> _subject;

        public MidiMessageFilter(Guid nodeId) : base(nodeId)
        {
            _subject = new Subject<MidiMessage>();
            var output = new OutputConnector<MidiMessage>(_subject.Where(Valid), Guid.Parse("{B7B1B419-1AEC-444F-8554-67415AB8F4F4}"));
            _outputs.Add(output);

            // StreamNote: PatteKi "und auch hier war ich :D" (2020-06-30 22:25)
            // StreamNote: m4cx: "Da war ich auch dran ;)" (2020-06-30 22:26)

            var input = new InputConnector<MidiMessage>(_subject, Guid.Parse("{482E6ABA-F11E-44CB-B945-D5DC6AE50286}"));
            _inputs.Add(input);
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

        public void Dispose()
        {
            _subject.Dispose();
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
