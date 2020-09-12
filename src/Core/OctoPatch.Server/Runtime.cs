﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Descriptions;
using OctoPatch.Setup;

namespace OctoPatch.Server
{
    /// <summary>
    /// Local implementation of the OctoPatch runtime
    /// </summary>
    public sealed class Runtime : IRuntime
    {
        /// <summary>
        /// patch instance
        /// </summary>
        private readonly IPatch _patch;

        /// <summary>
        /// Reference to the repository
        /// </summary>
        private readonly IRepository _repository;

        /// <summary>
        /// List of all available node descriptions
        /// </summary>
        private readonly NodeDescription[] _descriptions;

        private readonly Dictionary<Guid, (INode node, NodeSetup setup)> _nodeMapping;

        private readonly Dictionary<Guid, (IWire wire, WireSetup setup)> _wireMapping;

        public Runtime(IRepository repository)
        {
            _repository = repository;

            var nodes = new List<NodeDescription>();
            nodes.AddRange(repository.GetNodeDescriptions());

            _descriptions = nodes.ToArray();

            _nodeMapping = new Dictionary<Guid, (INode node, NodeSetup setup)>();
            _wireMapping = new Dictionary<Guid, (IWire wire, WireSetup setup)>();

            _patch = new Patch();
            _patch.NodeAdded += PatchOnNodeAdded;
            _patch.NodeRemoved += PatchOnNodeRemoved;
            _patch.WireAdded += PatchOnWireAdded;
            _patch.WireRemoved += PatchOnWireRemoved;
        }

        #region Patch events

        /// <summary>
        /// Handles a new node
        /// </summary>
        /// <param name="node">node</param>
        private void PatchOnNodeAdded(INode node)
        {
            node.ConfigurationChanged += NodeOnConfigurationChanged;
            node.EnvironmentChanged += NodeOnEnvironmentChanged;
            node.StateChanged += NodeOnStateChanged;

            // Find setup and report new node to the outside
            if (_nodeMapping.TryGetValue(node.Id, out var nodeSetup))
            {
                NodeAdded?.Invoke(nodeSetup.setup, node.State, node.GetEnvironment());
            }
        }

        /// <summary>
        /// Handles removed nodes
        /// </summary>
        /// <param name="node">node</param>
        private void PatchOnNodeRemoved(INode node)
        {
            node.ConfigurationChanged -= NodeOnConfigurationChanged;
            node.EnvironmentChanged -= NodeOnEnvironmentChanged;
            node.StateChanged -= NodeOnStateChanged;

            NodeRemoved?.Invoke(node.Id);
        }

        private void PatchOnWireRemoved(IWire wire)
        {
            _wireMapping.Remove(wire.Id);
            WireRemoved?.Invoke(wire.Id);
        }

        private void PatchOnWireAdded(IWire wire)
        {
            // Find setup and report new node to the outside
            if (_wireMapping.TryGetValue(wire.Id, out var wireSetup))
            {
                WireAdded?.Invoke(wireSetup.setup);
            }
        }

        #endregion

        #region Node events

        /// <summary>
        /// Handles a changed node state
        /// </summary>
        /// <param name="node">node</param>
        /// <param name="state">state</param>
        private void NodeOnStateChanged(INode node, NodeState state)
        {
            NodeStateChanged?.Invoke(node.Id, state);
        }

        /// <summary>
        /// Handles a changed environment of a node
        /// </summary>
        /// <param name="node">node</param>
        /// <param name="environment">new environment</param>
        private void NodeOnEnvironmentChanged(INode node, string environment)
        {
            NodeEnvironmentChanged?.Invoke(node.Id, environment);
        }

        /// <summary>
        /// Handles a changed configuration of a node
        /// </summary>
        /// <param name="node">node</param>
        /// <param name="configuration">new configuration</param>
        private void NodeOnConfigurationChanged(INode node, string configuration)
        {
        }

        #endregion

        public async Task<NodeSetup> AddNode(string key, Guid? parentId, string connectorKey, CancellationToken cancellationToken)
        {
            var description = _descriptions.First(d => d.Key == key);

            INode parent = null;
            if (parentId.HasValue)
            {
                if (!_nodeMapping.TryGetValue(parentId.Value, out var parentSetup))
                {
                    throw new ArgumentException("parent does not exist");
                }

                parent = parentSetup.node;
            }

            var node = _repository.CreateNode(key, Guid.NewGuid(), parent, connectorKey);
            if (node == null)
            {
                return null;
            }

            var setup = new NodeSetup
            {
                NodeId = node.Id,
                Key = key,
                ParentNodeId = parentId,
                ParentConnector = connectorKey,
                Name = description.DisplayName,
                Description = description.DisplayDescription
            };

            _nodeMapping.Add(node.Id, (node, setup));

            await _patch.AddNode(node, cancellationToken);

            return setup;
        }

        public async Task RemoveNode(Guid nodeId, CancellationToken cancellationToken)
        {
            if (!_nodeMapping.TryGetValue(nodeId, out var x))
            {
                return;
            }

            await _patch.RemoveNode(nodeId, cancellationToken);
            _nodeMapping.Remove(nodeId);
        }

        public async Task<WireSetup> AddWire(Guid outputNodeId, string outputConnectorKey, Guid inputNodeId,
            string inputConnectorKey, CancellationToken cancellationToken)
        {
            var wire = await _patch.AddWire(outputNodeId, outputConnectorKey, 
                inputNodeId, inputConnectorKey, cancellationToken);

            var setup = new WireSetup
            {
                WireId = wire.Id,
                InputNodeId = inputNodeId,
                InputConnectorKey = inputConnectorKey,
                OutputNodeId = outputNodeId,
                OutputConnectorKey = outputConnectorKey
            };

            _wireMapping.Add(wire.Id, (wire, setup));
            WireAdded?.Invoke(setup);

            return setup;
        }

        public Task RemoveWire(Guid wireId, CancellationToken cancellationToken)
        {
            return _patch.RemoveWire(wireId, cancellationToken);
        }

        public Task<IEnumerable<NodeDescription>> GetNodeDescriptions(CancellationToken cancellationToken)
        {
            return Task.FromResult(_repository.GetNodeDescriptions());
        }

        public Task<IEnumerable<TypeDescription>> GetMessageDescriptions(CancellationToken cancellationToken)
        {
            return Task.FromResult(_repository.GetTypeDescriptions());
        }

        public Task<IEnumerable<NodeSetup>> GetNodes(CancellationToken cancellationToken)
        {
            return Task.FromResult(_nodeMapping.Values.Select(i => i.setup).AsEnumerable());
        }

        public Task<IEnumerable<WireSetup>> GetWires(CancellationToken cancellationToken)
        {
            return Task.FromResult(_wireMapping.Values.Select(i => i.setup).AsEnumerable());
        }

        public Task<GridSetup> GetConfiguration(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNodeDescription(Guid nodeId, string name, string description, CancellationToken cancellationToken)
        {
            if (_nodeMapping.TryGetValue(nodeId, out var nodeSetup))
            {
                nodeSetup.setup.Name = name;
                nodeSetup.setup.Description = description;
                NodeUpdated?.Invoke(nodeSetup.setup);
            }

            return Task.CompletedTask;
        }

        public Task SetConfiguration(GridSetup grid, CancellationToken cancellationToken)
        {
            //var description = _descriptions.First(d => d.Guid == nodeInstance.NodeDescription);
            //var node = await _repository.CreateNode(description.Guid, nodeInstance.Guid, cancellationToken);
            //await _patch.AddNode(node, cancellationToken);

            // _instanceMapping.Add(node.NodeId, (node, nodeInstance));

            throw new NotImplementedException();
        }

        public Task<string> GetNodeEnvironment(Guid nodeGuid, CancellationToken cancellationToken)
        {
            _nodeMapping.TryGetValue(nodeGuid, out var node);
            throw new NotImplementedException();
        }

        public Task<string> GetNodeConfiguration(Guid nodeGuid, CancellationToken cancellationToken)
        {
            if (_nodeMapping.TryGetValue(nodeGuid, out var node))
            {
                return Task.FromResult(node.setup.Configuration);
            }

            return Task.FromResult<string>(null);
        }

        public async Task SetNodeConfiguration(Guid nodeGuid, string configuration, CancellationToken cancellationToken)
        {
            if (_nodeMapping.TryGetValue(nodeGuid, out var nodeSetup))
            {
                await nodeSetup.node.Initialize(configuration, cancellationToken);
                nodeSetup.setup.Configuration = configuration;
                NodeUpdated?.Invoke(nodeSetup.setup);
            }
        }

        public async Task StartNode(Guid nodeId, CancellationToken cancellationToken)
        {
            if (_nodeMapping.TryGetValue(nodeId, out var nodeSetup))
            {
                await nodeSetup.node.Start(cancellationToken);
            }
        }

        public async Task StopNode(Guid nodeId, CancellationToken cancellationToken)
        {
            if (_nodeMapping.TryGetValue(nodeId, out var nodeSetup))
            {
                await nodeSetup.node.Stop(cancellationToken);
            }
        }

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
        public event Action<WireSetup> WireAdded;
        
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public event Action<Guid> WireRemoved;
        
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
    }
}
