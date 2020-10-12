using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OctoPatch.Descriptions;
using OctoPatch.Logging;
using OctoPatch.Setup;

namespace OctoPatch.Server
{
    /// <summary>
    /// Local implementation of the OctoPatch runtime
    /// </summary>
    public sealed class Runtime : IRuntime
    {
        private static readonly ILogger<Runtime> logger = LogManager.GetLogger<Runtime>();

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

        /// <summary>
        /// Collection of existing nodes
        /// </summary>
        private readonly ConcurrentDictionary<Guid, (INode node, NodeSetup setup)> _nodeMapping;

        /// <summary>
        /// Collection of existing wires
        /// </summary>
        private readonly ConcurrentDictionary<Guid, (IWire wire, WireSetup setup)> _wireMapping;

        public Runtime(IRepository repository)
        {
            _repository = repository;
            var nodes = new List<NodeDescription>();
            nodes.AddRange(repository.GetNodeDescriptions());

            _descriptions = nodes.ToArray();

            _nodeMapping = new ConcurrentDictionary<Guid, (INode node, NodeSetup setup)>();
            _wireMapping = new ConcurrentDictionary<Guid, (IWire wire, WireSetup setup)>();

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
            _wireMapping.TryRemove(wire.Id, out _);
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

        public async Task<NodeSetup> AddNode(string key, Guid? parentId, string connectorKey, int x, int y, CancellationToken cancellationToken)
        {
            var description = _descriptions.First(d => d.Key == key);

            var setup = new NodeSetup
            {
                NodeId = Guid.NewGuid(),
                Key = key,
                ParentNodeId = parentId,
                ParentConnector = connectorKey,
                Name = description.DisplayName,
                Description = description.DisplayDescription,
                PositionX = x,
                PositionY = y
            };

            return await AddNode(setup, cancellationToken) != null ? setup : null;
        }

        private async Task<INode> AddNode(NodeSetup setup, CancellationToken cancellationToken)
        {
            INode parent = null;
            if (setup.ParentNodeId.HasValue)
            {
                if (!_nodeMapping.TryGetValue(setup.ParentNodeId.Value, out var parentSetup))
                {
                    throw new ArgumentException("parent does not exist");
                }

                parent = parentSetup.node;
            }

            var node = _repository.CreateNode(setup.Key, setup.NodeId, parent, setup.ParentConnector);
            if (node == null)
            {
                return null;
            }

            _nodeMapping.TryAdd(node.Id, (node, setup));
            await _patch.AddNode(node, cancellationToken);

            // automatic configure
            if (setup.Configuration != null)
            {
                await node.Initialize(setup.Configuration, cancellationToken);
            }
            else
            {
                await node.Initialize(cancellationToken);
            }

            // automatic start
            await node.Start(cancellationToken);

            return node;
        }

        public async Task RemoveNode(Guid nodeId, CancellationToken cancellationToken)
        {
            if (!_nodeMapping.TryGetValue(nodeId, out _))
            {
                return;
            }

            await _patch.RemoveNode(nodeId, cancellationToken);
            _nodeMapping.TryRemove(nodeId, out _);
        }

        public async Task<WireSetup> AddWire(Guid outputNodeId, string outputConnectorKey, Guid inputNodeId,
            string inputConnectorKey, CancellationToken cancellationToken)
        {
            var setup = new WireSetup
            {
                OutputNodeId = outputNodeId,
                OutputConnectorKey = outputConnectorKey,
                InputNodeId = inputNodeId,
                InputConnectorKey = inputConnectorKey,
                WireId = Guid.NewGuid()
            };

            await AddWire(setup, cancellationToken);

            return setup;
        }

        private async Task AddWire(WireSetup setup, CancellationToken cancellationToken)
        {
            // Lookup Output Connector
            if (!_nodeMapping.TryGetValue(setup.OutputNodeId, out var outputSetup))
            {
                throw new ArgumentException("output node does not exist");
            }

            var outputConnector = outputSetup.node.Outputs.FirstOrDefault(c => c.Key == setup.OutputConnectorKey);
            if (outputConnector == null)
            {
                throw new ArgumentException("output connector could not be found");
            }

            // Lookup Input Connector
            if (!_nodeMapping.TryGetValue(setup.InputNodeId, out var inputSetup))
            {
                throw new ArgumentException("input node does not exist");
            }

            var inputConnector = inputSetup.node.Inputs.FirstOrDefault(c => c.Key == setup.InputConnectorKey);
            if (inputConnector == null)
            {
                throw new ArgumentException("input connector could not be found");
            }

            var wire = new Wire(setup.WireId, inputConnector, outputConnector);

            _wireMapping.TryAdd(setup.WireId, (wire, setup));
            await _patch.AddWire(wire, cancellationToken);
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

        public Task<IEnumerable<AdapterDescription>> GetAdapterDescriptions(CancellationToken cancellationToken)
        {
            return Task.FromResult(_repository.GetAdapterDescriptions());
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
            return Task.FromResult(new GridSetup
            {
                NodeInstances = _nodeMapping.Values.Select(n => n.setup).ToList(),
                WireInstances = _wireMapping.Values.Select(w => w.setup).ToList()
            });
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

        public Task SetNodePosition(Guid nodeId, int x, int y, CancellationToken cancellationToken)
        {
            if (_nodeMapping.TryGetValue(nodeId, out var nodeSetup))
            {
                nodeSetup.setup.PositionX = x;
                nodeSetup.setup.PositionY = y;
                NodeUpdated?.Invoke(nodeSetup.setup);
            }

            return Task.CompletedTask;
        }

        public async Task SetConfiguration(GridSetup grid, CancellationToken cancellationToken)
        {
            #region Cleanup current grid

            // Remove wires in the first step
            foreach (var wireId in _wireMapping.Keys.ToArray())
            {
                await RemoveWire(wireId, cancellationToken);
            }

            // Remove all nodes
            var nodes = _nodeMapping.Values.Select(n => n.node).ToList();

            // Remove Splitter
            foreach (var splitterNode in nodes.OfType<SplitterNode>().ToArray())
            {
                await RemoveNode(splitterNode.Id, cancellationToken);
                nodes.Remove(splitterNode);
            }

            // Remove collectors
            foreach (var collectorNode in nodes.OfType<CollectorNode>().ToArray())
            {
                await RemoveNode(collectorNode.Id, cancellationToken);
                nodes.Remove(collectorNode);
            }

            // Remove regular nodes
            while (nodes.Any())
            {
                var freeNodes = nodes.Where(n => nodes.OfType<IAttachedNode>().All(node => node.ParentNode != n)).ToArray();
                foreach (var freeNode in freeNodes)
                {
                    await RemoveNode(freeNode.Id, cancellationToken);
                    nodes.Remove(freeNode);
                }
            }

            #endregion

            if (grid != null)
            {
                if (grid.NodeInstances == null || grid.WireInstances == null)
                {
                    throw new ArgumentException("grid setup is not complete");
                }

                // Create nodes
                foreach (var rootNode in grid.NodeInstances.Where(n => !n.ParentNodeId.HasValue))
                {
                    await CreateNode(rootNode, grid.NodeInstances, cancellationToken);
                }

                // Create wires
                foreach (var wireSetup in grid.WireInstances)
                {
                    await AddWire(wireSetup, cancellationToken);
                }
            }
        }

        private async Task CreateNode(NodeSetup setup, IEnumerable<NodeSetup> nodes, CancellationToken cancellationToken)
        {
            var node = await AddNode(setup, cancellationToken);
            if (node == null)
            {
                return;
            }

            // Create children
            var localNodes = nodes.ToArray();
            foreach (var nodeSetup in localNodes.Where(n => n.ParentNodeId == setup.NodeId))
            {
                await CreateNode(nodeSetup, localNodes, cancellationToken);
            }
        }

        public Task<string> GetNodeEnvironment(Guid nodeGuid, CancellationToken cancellationToken)
        {
            return !_nodeMapping.TryGetValue(nodeGuid, out var node) ?
                Task.FromResult<string>(null) : Task.FromResult(node.setup.Configuration);
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

        /// <inheritdoc />
        public event Action<NodeSetup, NodeState, string> NodeAdded;

        /// <inheritdoc />
        public event Action<Guid> NodeRemoved;

        /// <inheritdoc />
        public event Action<WireSetup> WireAdded;

        /// <inheritdoc />
        public event Action<Guid> WireRemoved;

        /// <inheritdoc />
        public event Action<WireSetup> WireUpdated;

        /// <inheritdoc />
        public event Action<NodeSetup> NodeUpdated;

        /// <inheritdoc />
        public event Action<Guid, NodeState> NodeStateChanged;

        /// <inheritdoc />
        public event Action<Guid, string> NodeEnvironmentChanged;
    }
}
