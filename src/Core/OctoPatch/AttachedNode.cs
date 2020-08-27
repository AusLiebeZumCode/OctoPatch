using System;
using System.Threading;
using System.Threading.Tasks;

namespace OctoPatch
{
    /// <summary>
    /// Special node to be attached directly to a specified node
    /// </summary>
    public sealed class AttachedNode : Node<INodeConfiguration>
    {
        public AttachedNode(Guid nodeId) : base(nodeId)
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
