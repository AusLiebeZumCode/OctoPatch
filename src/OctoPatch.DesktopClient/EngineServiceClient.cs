using Microsoft.AspNetCore.SignalR.Client;
using OctoPatch.Communication;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OctoPatch.DesktopClient
{
    /// <summary>
    /// Implementation of the engine service client
    /// </summary>
    internal sealed class EngineServiceClient : IEngineServiceClient, IEngineServiceCallback
    {
        private HubConnection _hubConnection;

        public EngineServiceClient()
        {

        }

        public async Task Setup(Uri uri, CancellationToken cancellationToken)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(uri)
                .Build();

            _hubConnection.Closed += Connection_Closed;

            _hubConnection.On<NodeInstance>(nameof(NodeAdded), NodeAdded);
            _hubConnection.On<Guid>(nameof(NodeRemoved), NodeRemoved);
            _hubConnection.On<WireInstance>(nameof(WireAdded), WireAdded);
            _hubConnection.On<Guid>(nameof(WireRemoved), WireRemoved);

            await _hubConnection.StartAsync(cancellationToken);
        }

        private Task Connection_Closed(Exception arg)
        {
            return Task.CompletedTask;
        }

        #region IEngineService

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<Grid> GetEngineConfiguration(CancellationToken cancellationToken)
        {
            return _hubConnection.InvokeAsync<Grid>(nameof(GetEngineConfiguration), cancellationToken, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<IEnumerable<MessageDescription>> GetMessageDescriptions(CancellationToken cancellationToken)
        {
            return _hubConnection.InvokeAsync<IEnumerable<MessageDescription>>(nameof(GetMessageDescriptions), cancellationToken, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<string> GetNodeConfiguration(Guid nodeGuid, CancellationToken cancellationToken)
        {
            return _hubConnection.InvokeAsync<string>(nameof(GetNodeConfiguration), nodeGuid, cancellationToken, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<IEnumerable<NodeDescription>> GetNodeDescriptions(CancellationToken cancellationToken)
        {
            return _hubConnection.InvokeAsync<IEnumerable<NodeDescription>>(nameof(GetNodeDescriptions), cancellationToken, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<string> GetNodeEnvironment(Guid nodeGuid, CancellationToken cancellationToken)
        {
            return _hubConnection.InvokeAsync<string>(nameof(GetNodeEnvironment), nodeGuid, cancellationToken, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<IEnumerable<NodeInstance>> GetNodes(CancellationToken cancellationToken)
        {
            return _hubConnection.InvokeAsync<IEnumerable<NodeInstance>>(nameof(GetNodes), cancellationToken, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<IEnumerable<WireInstance>> GetWires(CancellationToken cancellationToken)
        {
            return _hubConnection.InvokeAsync<IEnumerable<WireInstance>>(nameof(GetWires), cancellationToken, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task SetEngineConfiguration(Grid grid, CancellationToken cancellationToken)
        {
            return _hubConnection.InvokeAsync(nameof(SetEngineConfiguration), grid, cancellationToken, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task SetNodeConfiguration(Guid nodeGuid, string configuration, CancellationToken cancellationToken)
        {
            return _hubConnection.InvokeAsync(nameof(SetNodeConfiguration), nodeGuid, configuration, cancellationToken, cancellationToken);
        }

        #endregion

        #region IEngineServiceCallback

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void NodeAdded(NodeInstance instance)
        {
            OnNodeAdded?.Invoke(instance);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void NodeRemoved(Guid instanceGuid)
        {
            OnNodeRemoved?.Invoke(instanceGuid);
        }



        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void WireAdded(WireInstance instance)
        {
            OnWireAdded?.Invoke(instance);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void WireRemoved(Guid instanceGuid)
        {
            OnWireRemoved?.Invoke(instanceGuid);
        }

        #endregion

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public event Action<NodeInstance> OnNodeAdded;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public event Action<Guid> OnNodeRemoved;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public event Action<WireInstance> OnWireAdded;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public event Action<Guid> OnWireRemoved;
    }
}
