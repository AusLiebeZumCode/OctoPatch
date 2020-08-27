using System;
using System.Threading;
using System.Threading.Tasks;

namespace OctoPatch
{
    /// <summary>
    /// Special node to collect different values to combine them into a complex type
    /// </summary>
    public abstract class CollectorNode : Node<INodeConfiguration>
    {
        protected CollectorNode(Guid nodeId, ComplexTypeDescription typeDescription) : base(nodeId)
        {
        }

        protected override Task OnInitialize(INodeConfiguration configuration, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
