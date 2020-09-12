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

            _hubConnection.On<NodeSetup, NodeState, string>(nameof(OnNodeAdded), OnNodeAdded);
            _hubConnection.On<Guid>(nameof(OnNodeRemoved), OnNodeRemoved);
            _hubConnection.On<NodeSetup>(nameof(OnNodeUpdated), OnNodeUpdated);
            _hubConnection.On<WireSetup>(nameof(OnWireAdded), OnWireAdded);
            _hubConnection.On<Guid>(nameof(OnWireRemoved), OnWireRemoved);

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
        public Task SetNodeDescription(Guid nodeId, string name, string description,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

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

        public Task StartNode(Guid nodeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopNode(Guid nodeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<NodeSetup> AddNode(string key, Guid? parentId, string connectorKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveNode(Guid nodeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        public Task<WireSetup> AddWire(Guid outputNodeId, string outputConnectorKey, Guid inputNodeId, string intputConnectorKey,
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
        public void OnNodeAdded(NodeSetup instance, NodeState state, string environment)
        {
            NodeAdded?.Invoke(instance, state, environment);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void OnNodeStateChanged(Guid nodeId, NodeState state)
        {
            NodeStateChanged?.Invoke(nodeId, state);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void OnNodeEnvironmentChanged(Guid nodeId, string environment)
        {
            NodeEnvironmentChanged?.Invoke(nodeId, environment);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void OnNodeRemoved(Guid instanceGuid)
        {
            NodeRemoved?.Invoke(instanceGuid);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void OnNodeUpdated(NodeSetup nodeSetup)
        {
            NodeUpdated?.Invoke(nodeSetup);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void OnWireAdded(WireSetup instance)
        {
            WireAdded?.Invoke(instance);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void OnWireRemoved(Guid instanceGuid)
        {
            WireRemoved?.Invoke(instanceGuid);
        }

        #endregion

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public event Action<NodeSetup, NodeState, string> NodeAdded;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public event Action<Guid> NodeRemoved;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public event Action<NodeSetup> NodeUpdated;
        
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public event Action<Guid, NodeState> NodeStateChanged;
        
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public event Action<Guid, string> NodeEnvironmentChanged;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public event Action<WireSetup> WireAdded;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public event Action<Guid> WireRemoved;
    }
}
