﻿using System;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Descriptions;

namespace OctoPatch
{
    /// <summary>
    /// Special node to collect different values to combine them into a complex type
    /// </summary>
    public sealed class CollectorNode : Node<EmptyConfiguration, EmptyEnvironment>, ICollectorNode
    {
        private readonly TypeDescription _description;

        public IInputConnector Connector { get; }

        protected override EmptyConfiguration DefaultConfiguration => new EmptyConfiguration();

        public CollectorNode(Guid nodeId, TypeDescription description, IInputConnector connector) : base(nodeId)
        {
            _description = description ?? throw new ArgumentNullException(nameof(description));
            Connector = connector ?? throw new ArgumentNullException(nameof(connector));
        }

        protected override Task OnInitialize(EmptyConfiguration configuration, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnStart(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnStop(CancellationToken cancellationToken)
        {
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
