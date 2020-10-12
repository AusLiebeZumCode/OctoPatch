﻿using System;
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

        public Task<IEnumerable<AdapterDescription>> GetAdapterDescriptions(CancellationToken cancellationToken)
        {
            return _hubConnection.InvokeAsync<IEnumerable<AdapterDescription>>(nameof(GetAdapterDescriptions), cancellationToken, cancellationToken);
        }

        /// <inheritdoc />
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

        public Task SetNodePosition(Guid nodeId, int x, int y, CancellationToken cancellationToken)
        {
            return _hubConnection.InvokeAsync(nameof(SetNodePosition), nodeId, x, y, cancellationToken);
        }

        public Task StartNode(Guid nodeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopNode(Guid nodeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<NodeSetup> AddNode(string key, Guid? parentId, string connectorKey, int x, int y, CancellationToken cancellationToken)
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

        #region Wire / Adapter configuration

        /// <inheritdoc />
        public Task SetAdapter(Guid wireId, string key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<string> GetAdapterEnvironment(Guid wireId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<string> GetAdapterConfiguration(Guid wireId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task SetAdapterConfiguration(Guid wireId, string configuration, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region IRuntimeCallbacks

        /// <inheritdoc />
        public void OnNodeAdded(NodeSetup setup, NodeState state, string environment)
        {
            NodeAdded?.Invoke(setup, state, environment);
        }

        /// <inheritdoc />
        public void OnNodeStateChanged(Guid nodeId, NodeState state)
        {
            NodeStateChanged?.Invoke(nodeId, state);
        }

        /// <inheritdoc />
        public void OnNodeEnvironmentChanged(Guid nodeId, string environment)
        {
            NodeEnvironmentChanged?.Invoke(nodeId, environment);
        }

        /// <inheritdoc />
        public void OnNodeRemoved(Guid instanceGuid)
        {
            NodeRemoved?.Invoke(instanceGuid);
        }

        /// <inheritdoc />
        public void OnNodeUpdated(NodeSetup setup)
        {
            NodeUpdated?.Invoke(setup);
        }

        /// <inheritdoc />
        public void OnWireAdded(WireSetup setup)
        {
            WireAdded?.Invoke(setup);
        }

        /// <inheritdoc />
        public void OnWireRemoved(Guid wireId)
        {
            WireRemoved?.Invoke(wireId);
        }

        /// <inheritdoc />
        public void OnWireUpdated(WireSetup setup)
        {
            WireUpdated?.Invoke(setup);
        }

        /// <inheritdoc />
        public void OnAdapterEnvironmentChanged(Guid nodeId, string environment)
        {
            AdapterEnvironmentChanged?.Invoke(nodeId, environment);
        }

        #endregion

        #region IRuntimeEvents

        /// <inheritdoc />
        public event Action<NodeSetup, NodeState, string> NodeAdded;

        /// <inheritdoc />
        public event Action<Guid> NodeRemoved;

        /// <inheritdoc />
        public event Action<NodeSetup> NodeUpdated;
        
        /// <inheritdoc />
        public event Action<Guid, NodeState> NodeStateChanged;
        
        /// <inheritdoc />
        public event Action<Guid, string> NodeEnvironmentChanged;

        /// <inheritdoc />
        public event Action<WireSetup> WireAdded;

        /// <inheritdoc />
        public event Action<Guid> WireRemoved;

        /// <inheritdoc />
        public event Action<WireSetup> WireUpdated;

        /// <inheritdoc />
        public event Action<Guid, string> AdapterEnvironmentChanged;

        #endregion
    }
}
