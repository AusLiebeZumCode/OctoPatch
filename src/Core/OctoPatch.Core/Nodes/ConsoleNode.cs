using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;

namespace OctoPatch.Core.Nodes
{
    /// <summary>
    /// Console node for a common output of all kind of messages
    /// </summary>
    public sealed class ConsoleNode : Node<EmptyConfiguration, EmptyEnvironment>
    {
        #region Descriptions

        /// <summary>
        /// Node description
        /// </summary>
        public static NodeDescription Description =>
            CommonNodeDescription.Create<ConsoleNode>(Guid.Parse(CorePlugin.PluginId), 
                "Console", "Console output").AddInputDescription(InputDescription);

        /// <summary>
        /// Description of the input connector
        /// </summary>
        public static ConnectorDescription InputDescription => new ConnectorDescription(
            "Input", "Input", "Common input", new AllContentType());

        #endregion

        protected override EmptyConfiguration DefaultConfiguration => new EmptyConfiguration();

        public ConsoleNode(Guid id) : base(id)
        {
            RegisterInputConnector(InputDescription).HandleRaw(Handle);
        }

        private void Handle(Message message)
        {
            if (State == NodeState.Running)
            {
                Trace.WriteLine(message.ToString());
            }
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
