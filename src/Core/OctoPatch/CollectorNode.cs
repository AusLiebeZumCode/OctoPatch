using System;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Descriptions;

namespace OctoPatch
{
    /// <summary>
    /// Special node to collect different values to combine them into a complex type
    /// </summary>
    public sealed class CollectorNode : Node<IConfiguration, IEnvironment>
    {
        public CollectorNode(Guid nodeId, TypeDescription typeDescription) : base(nodeId)
        {
        }

        protected override Task OnInitialize(IConfiguration configuration, CancellationToken cancellationToken)
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
