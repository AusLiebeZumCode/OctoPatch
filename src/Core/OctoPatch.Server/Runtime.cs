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
        private readonly NodeDescription[] _nodeDescriptions;

        /// <summary>
        /// LIst of all available adapter descriptions
        /// </summary>
        private readonly AdapterDescription[] _adapterDescriptions;

        /// <summary>
        /// Collection of existing nodes
        /// </summary>
        private readonly ConcurrentDictionary<Guid, (INode node, NodeSetup setup)> _nodeMapping;

        /// <summary>
        /// Collection of existing wires
        /// </summary>
        private readonly ConcurrentDictionary<Guid, (IWire wire, IAdapter adapter, WireSetup setup)> _wireMapping;

        /// <summary>
        /// Collection of all adapters and fitting wire id
        /// </summary>
        private readonly ConcurrentDictionary<IAdapter, Guid> _adapterMapping;

        public Runtime(IRepository repository)
        {
            _repository = repository;
            _nodeDescriptions = repository.GetNodeDescriptions().ToArray();
            _adapterDescriptions = repository.GetAdapterDescriptions().ToArray();

            _nodeMapping = new ConcurrentDictionary<Guid, (INode node, NodeSetup setup)>();
            _wireMapping = new ConcurrentDictionary<Guid, (IWire wire, IAdapter adapter, WireSetup setup)>();
            _adapterMapping = new ConcurrentDictionary<IAdapter, Guid>();

            _patch = new Patch();
            _patch.NodeAdded += PatchOnNodeAdded;
            _patch.NodeRemoved += PatchOnNodeRemoved;
            _patch.WireAdded += PatchOnWireAdded;
            _patch.WireRemoved += PatchOnWireRemoved;
            _patch.AdapterAdded += PatchOnAdapterAdded;
            _patch.AdapterRemoved += PatchOnAdapterRemoved;
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

        /// <summary>
        /// Handles a new wire
        /// </summary>
        /// <param name="wire">added wire</param>
        private void PatchOnWireAdded(IWire wire)
        {
            // Find setup and report new node to the outside
            if (_wireMapping.TryGetValue(wire.Id, out var wireSetup))
            {
                WireAdded?.Invoke(wireSetup.setup);
            }
        }

        /// <summary>
        /// Handles removed wires
        /// </summary>
        /// <param name="wire">removed wire</param>
        private void PatchOnWireRemoved(IWire wire)
        {
            WireRemoved?.Invoke(wire.Id);
        }

        /// <summary>
        /// Handles added adapter
        /// </summary>
        /// <param name="wire">host wire</param>
        /// <param name="adapter">added adapter</param>
        private void PatchOnAdapterAdded(IWire wire, IAdapter adapter)
        {
            adapter.EnvironmentChanged += AdapterOnEnvironmentChanged;
            adapter.ConfigurationChanged += AdapterOnConfigurationChanged;

            _adapterMapping.TryAdd(adapter, wire.Id);
        }

        /// <summary>
        /// Handles removed adapter
        /// </summary>
        /// <param name="wire">host wire</param>
        /// <param name="adapter">removed adapter</param>
        private void PatchOnAdapterRemoved(IWire wire, IAdapter adapter)
        {
            adapter.EnvironmentChanged -= AdapterOnEnvironmentChanged;
            adapter.ConfigurationChanged -= AdapterOnConfigurationChanged;

            _adapterMapping.TryRemove(adapter, out _);
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
            if (_nodeMapping.TryGetValue(node.Id, out var nodeSetup))
            {
                nodeSetup.setup.Configuration = configuration;
                NodeUpdated?.Invoke(nodeSetup.setup);
            }
        }

        #endregion

        #region Adapter events

        /// <summary>
        /// Handles a changed configuration of the adapter
        /// </summary>
        /// <param name="adapter">adapter</param>
        /// <param name="configuration">configuration</param>
        private void AdapterOnConfigurationChanged(IAdapter adapter, string configuration)
        {
            if (_adapterMapping.TryGetValue(adapter, out var wireId) &&
                _wireMapping.TryGetValue(wireId, out var wireSetup))
            {
                wireSetup.setup.AdapterConfiguration = configuration;
                WireUpdated?.Invoke(wireSetup.setup);
            }
        }

        /// <summary>
        /// Handles a changed environment of the adapter
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="environment"></param>
        private void AdapterOnEnvironmentChanged(IAdapter adapter, string environment)
        {
            if (_adapterMapping.TryGetValue(adapter, out var wireId) &&
                _wireMapping.TryGetValue(wireId, out var wireSetup))
            {
                AdapterEnvironmentChanged?.Invoke(wireSetup.wire.Id, environment);
            }
        }

        #endregion

        #region IRuntimeMethods

        public async Task<NodeSetup> AddNode(string key, Guid? parentId, string connectorKey, int x, int y, CancellationToken cancellationToken)
        {
            var description = _nodeDescriptions.First(d => d.Key == key);

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

            var outputConnector = outputSetup.node.Inputs.FirstOrDefault(c => c.Key == setup.OutputConnectorKey);
            if (outputConnector == null)
            {
                throw new ArgumentException("output connector could not be found");
            }

            // Lookup Input Connector
            if (!_nodeMapping.TryGetValue(setup.InputNodeId, out var inputSetup))
            {
                throw new ArgumentException("input node does not exist");
            }

            var inputConnector = inputSetup.node.Outputs.FirstOrDefault(c => c.Key == setup.InputConnectorKey);
            if (inputConnector == null)
            {
                throw new ArgumentException("input connector could not be found");
            }

            var wire = new Wire(setup.WireId, inputConnector, outputConnector);

            _wireMapping.TryAdd(setup.WireId, (wire, null, setup));
            await _patch.AddWire(wire, cancellationToken);
        }

        public Task RemoveWire(Guid wireId, CancellationToken cancellationToken)
        {
            _wireMapping.TryRemove(wireId, out _);
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

        public Task<IEnumerable<string>> GetSupportedAdapters(Guid wireId, CancellationToken cancellationToken)
        {
            var fittingAdapters = new List<string>();
            if (!_wireMapping.TryGetValue(wireId, out var wireSetup))
            {
                return Task.FromResult(fittingAdapters.AsEnumerable());
            }

            var input = wireSetup.wire.Output.ContentType.Type;
            var output = wireSetup.wire.Input.ContentType.Type;

            foreach (var adapterDescription in _adapterDescriptions)
            {
                // See if any of the supported combinations fits to the given situation
                if (adapterDescription.SupportedTypeCombinations.Any(d =>
                    string.Equals(d.input, input, StringComparison.InvariantCultureIgnoreCase) &&
                    string.Equals(d.output, output, StringComparison.InvariantCultureIgnoreCase)))
                {
                    fittingAdapters.Add(adapterDescription.Key);
                }
            }

            return Task.FromResult(fittingAdapters.AsEnumerable());
        }


        public async Task SetAdapter(Guid wireId, string key, CancellationToken cancellationToken)
        {
            if (!_wireMapping.TryGetValue(wireId, out var wireSetup))
            {
                throw new ArgumentException("wire does not exist");
            }

            // Try to create an adapter
            var adapter = _repository.CreateAdapter(key, wireSetup.wire);
            await _patch.AddAdapter(wireSetup.wire.Id, adapter, cancellationToken);

            wireSetup.adapter = adapter;
            wireSetup.setup.AdapterKey = key;
            wireSetup.setup.AdapterConfiguration = adapter.GetConfiguration();

            // Send out events
            WireUpdated?.Invoke(wireSetup.setup);
            AdapterEnvironmentChanged?.Invoke(wireId, adapter.GetEnvironment());
        }

        public Task<string> GetAdapterEnvironment(Guid wireId, CancellationToken cancellationToken)
        {
            if (_wireMapping.TryGetValue(wireId, out var wireSetup))
            {
                var adapter = wireSetup.adapter;
                if (adapter != null)
                {
                    return Task.FromResult(wireSetup.adapter.GetEnvironment());
                }
            }

            return Task.FromResult<string>(null);
        }

        public Task<string> GetAdapterConfiguration(Guid wireId, CancellationToken cancellationToken)
        {
            if (_wireMapping.TryGetValue(wireId, out var wireSetup))
            {
                var adapter = wireSetup.adapter;
                if (adapter != null)
                {
                    return Task.FromResult(wireSetup.adapter.GetConfiguration());
                }
            }

            return Task.FromResult<string>(null);
        }

        public Task SetAdapterConfiguration(Guid wireId, string configuration, CancellationToken cancellationToken)
        {
            if (_wireMapping.TryGetValue(wireId, out var wireSetup) && wireSetup.adapter != null)
            {
                var adapter = wireSetup.adapter;
                return wireSetup.adapter.SetConfiguration(configuration, cancellationToken);
            }

            return Task.CompletedTask;
        }

        #endregion

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

        /// <inheritdoc />
        public event Action<Guid, string> AdapterEnvironmentChanged;
    }
}
