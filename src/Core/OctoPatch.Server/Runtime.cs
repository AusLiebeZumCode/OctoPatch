using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Descriptions;
using OctoPatch.Setup;

namespace OctoPatch.Server
{
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

        private Dictionary<Guid, (INode node, NodeSetup setup)> _nodeMapping;

        private Dictionary<IWire, WireSetup> _wireMapping;

        public Runtime(IRepository repository)
        {
            _patch = new Patch();

            _repository = repository;

            var nodes = new List<NodeDescription>();
            nodes.AddRange(repository.GetNodeDescriptions());

            _descriptions = nodes.ToArray();

            _nodeMapping = new Dictionary<Guid, (INode node, NodeSetup setup)>();
            _wireMapping = new Dictionary<IWire, WireSetup>();
        }

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

            node.StateChanged += NodeOnStateChanged;
            node.EnvironmentChanged += NodeOnEnvironmentChanged;

            await _patch.AddNode(node, cancellationToken);

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

            OnNodeAdded?.Invoke(setup, node.State, node.GetEnvironment());

            return setup;
        }

        public async Task<NodeSetup> AddConnectorNode(string key, Guid parentId, string connectorKey,
            CancellationToken cancellationToken)
        {
            var description = _descriptions.First(d => d.Key == key);
            if (!_nodeMapping.TryGetValue(parentId, out var parent))
            {
                throw new ArgumentException("parent does not exist");
            }

            INode node;
            if (description is SplitterNodeDescription splitter)
            {
                // TODO: Check if types match (splitter description vs. output vs. type)

                var output = parent.node.Outputs.FirstOrDefault(o => o.Key == connectorKey);
                var type = _repository.GetTypeDescriptions().FirstOrDefault(t => t.Key == output.Description.Key);
                node = new SplitterNode(Guid.NewGuid(), type, output);
            }
            else if (description is CollectorNodeDescription collector)
            {
                // TODO: Check if types match (splitter description vs. output vs. type)

                var input = parent.node.Inputs.FirstOrDefault(o => o.Key == connectorKey);
                var type = _repository.GetTypeDescriptions().FirstOrDefault(t => t.Key == input.Description.Key);
                node = new CollectorNode(Guid.NewGuid(), type, input);
            }
            else
            {
                throw new ArgumentException("description key is not a connector node");
            }

            node.StateChanged += NodeOnStateChanged;
            node.EnvironmentChanged += NodeOnEnvironmentChanged;

            await _patch.AddNode(node, cancellationToken);

            var setup = new NodeSetup
            {
                NodeId = node.Id,
                Key = key,
                ParentNodeId = parentId,
                ParentConnector = null,
                Name = description.DisplayName,
                Description = description.DisplayDescription
            };

            _nodeMapping.Add(node.Id, (node, setup));

            OnNodeAdded?.Invoke(setup, node.State, node.GetEnvironment());

            return setup;
        }

        private void NodeOnStateChanged(object sender, NodeState e)
        {
            var node = (INode) sender;
            OnNodeStateChanged?.Invoke(node.Id, e);
        }

        private void NodeOnEnvironmentChanged(object sender, string e)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveNode(Guid nodeId, CancellationToken cancellationToken)
        {
            if (!_nodeMapping.TryGetValue(nodeId, out var x))
            {
                return;
            }

            x.node.EnvironmentChanged -= NodeOnEnvironmentChanged;
            x.node.StateChanged -= NodeOnStateChanged;

            await _patch.RemoveNode(nodeId, cancellationToken);
            _nodeMapping.Remove(nodeId);

            OnNodeRemoved?.Invoke(nodeId);
        }

        public Task<WireSetup> AddWire(Guid outputNodeId, Guid outputConnectorId, Guid inputNodeId,
            Guid intputConnectorId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveWire(Guid wireId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
            return Task.FromResult(_wireMapping.Values.AsEnumerable());
        }

        public Task<GridSetup> GetConfiguration(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNodeDescription(Guid nodeId, string name, string description, CancellationToken cancellationToken)
        {
            if (!_nodeMapping.TryGetValue(nodeId, out var x))
            {
                return Task.CompletedTask;
            }

            x.setup.Name = name;
            x.setup.Description = description;

            OnNodeUpdated?.Invoke(x.setup);
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
            _nodeMapping.TryGetValue(nodeGuid, out var node);
            return Task.FromResult(node.setup.Configuration);
        }

        public async Task SetNodeConfiguration(Guid nodeGuid, string configuration, CancellationToken cancellationToken)
        {
            _nodeMapping.TryGetValue(nodeGuid, out var node);
            await node.node.Initialize(configuration, cancellationToken);
            node.setup.Configuration = configuration;
        }

        public Task StartNode(Guid nodeId, CancellationToken cancellationToken)
        {
            _nodeMapping.TryGetValue(nodeId, out var node);
            return node.node.Start(cancellationToken);
        }

        public Task StopNode(Guid nodeId, CancellationToken cancellationToken)
        {
            _nodeMapping.TryGetValue(nodeId, out var node);
            return node.node.Stop(cancellationToken);
        }

        public event Action<NodeSetup, NodeState, string> OnNodeAdded = delegate { };
        public event Action<Guid> OnNodeRemoved = delegate { };
        public event Action<WireSetup> OnWireAdded = delegate { };
        public event Action<Guid> OnWireRemoved = delegate { };

        public event Action<NodeSetup> OnNodeUpdated = delegate {};

        public event Action<Guid, NodeState> OnNodeStateChanged = delegate {};
        public event Action<Guid, string> OnNodeEnvironmentChanged = delegate {};
    }
}
