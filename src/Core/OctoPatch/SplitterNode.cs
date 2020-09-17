using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Descriptions;

namespace OctoPatch
{
    /// <summary>
    /// Special node to split up complex types
    /// </summary>
    public sealed class SplitterNode : Node<EmptyConfiguration, EmptyEnvironment>
    {
        private IDisposable _subscription;

        private readonly IOutputConnector _connector;

        private TypeDescription _description;

        private readonly Dictionary<string, IOutputConnectorHandler> _outputs;

        protected override EmptyConfiguration DefaultConfiguration => new EmptyConfiguration();

        public SplitterNode(Guid nodeId, TypeDescription description, IOutputConnector connector) : base(nodeId)
        {
            _description = description;
            _connector = connector;

            _outputs = new Dictionary<string, IOutputConnectorHandler>();
            foreach (var propertyDescription in description.PropertyDescriptions)
            {
                var output = RegisterOutputConnector(new ConnectorDescription(
                    propertyDescription.Key, 
                    propertyDescription.DisplayName, 
                    propertyDescription.DisplayDescription,
                    propertyDescription.ContentType));

                _outputs.Add(propertyDescription.Key, output);
            }
        }

        protected override Task OnInitialize(EmptyConfiguration configuration, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnStart(CancellationToken cancellationToken)
        {
            _subscription = _connector.Subscribe((msg) =>
            {
                // TODO
            });

            return Task.CompletedTask;
        }

        protected override Task OnStop(CancellationToken cancellationToken)
        {
            _subscription?.Dispose();
            _subscription = null;

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
    }
}
