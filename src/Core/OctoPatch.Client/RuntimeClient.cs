using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using OctoPatch.Descriptions;
using OctoPatch.Setup;

namespace OctoPatch.Client
{
    /// <summary>
    /// Implementation of the runtime hub
    /// </summary>
    public sealed class RuntimeClient : IRuntimeClient, IRuntimeCallbacks
    {
        private HubConnection _hubConnection;

        public RuntimeClient()
        {

        }

        public async Task Setup(Uri uri, CancellationToken cancellationToken)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(uri)
                .Build();

            _hubConnection.Closed += Connection_Closed;

            _hubConnection.On<NodeSetup>(nameof(NodeAdded), NodeAdded);
            _hubConnection.On<Guid>(nameof(NodeRemoved), NodeRemoved);
            _hubConnection.On<WireSetup>(nameof(WireAdded), WireAdded);
            _hubConnection.On<Guid>(nameof(WireRemoved), WireRemoved);

            await _hubConnection.StartAsync(cancellationToken);
        }

        private Task Connection_Closed(Exception arg)
        {
            return Task.CompletedTask;
        }

        #region IRuntimeMethods

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<GridSetup> GetConfiguration(CancellationToken cancellationToken)
        {
            return _hubConnection.InvokeAsync<GridSetup>(nameof(GetConfiguration), cancellationToken, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<IEnumerable<TypeDescription>> GetMessageDescriptions(CancellationToken cancellationToken)
        {
            return _hubConnection.InvokeAsync<IEnumerable<TypeDescription>>(nameof(GetMessageDescriptions), cancellationToken, cancellationToken);
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
        public Task<IEnumerable<NodeSetup>> GetNodes(CancellationToken cancellationToken)
        {
            return _hubConnection.InvokeAsync<IEnumerable<NodeSetup>>(nameof(GetNodes), cancellationToken, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<IEnumerable<WireSetup>> GetWires(CancellationToken cancellationToken)
        {
            return _hubConnection.InvokeAsync<IEnumerable<WireSetup>>(nameof(GetWires), cancellationToken, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task SetConfiguration(GridSetup grid, CancellationToken cancellationToken)
        {
            return _hubConnection.InvokeAsync(nameof(SetConfiguration), grid, cancellationToken, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task SetNodeConfiguration(Guid nodeGuid, string configuration, CancellationToken cancellationToken)
        {
            return _hubConnection.InvokeAsync(nameof(SetNodeConfiguration), nodeGuid, configuration, cancellationToken, cancellationToken);
        }

        public Task<NodeSetup> AddNode(string key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        public Task RemoveNode(Guid nodeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        public Task<WireSetup> AddWire(Guid outputNodeId, Guid outputConnectorId, Guid inputNodeId, Guid intputConnectorId,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        public Task RemoveWire(Guid wireId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRuntimeCallbacks

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void NodeAdded(NodeSetup instance)
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
        public void WireAdded(WireSetup instance)
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
        public event Action<NodeSetup> OnNodeAdded;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public event Action<Guid> OnNodeRemoved;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public event Action<WireSetup> OnWireAdded;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public event Action<Guid> OnWireRemoved;
    }
}
