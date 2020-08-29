﻿using System;
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

        public async Task<NodeSetup> AddNode(string key, CancellationToken cancellationToken)
        {
            var description = _descriptions.First(d => d.Key == key);
            var node = _repository.CreateNode(key, Guid.NewGuid());
            if (node == null)
            {
                return null;
            }

            await _patch.AddNode(node, cancellationToken);

            var setup = new NodeSetup
            {
                NodeId = node.Id,
                Key = key,
                Name = description.DisplayName,
                Description = description.DisplayDescription
            };

            _nodeMapping.Add(node.Id, (node, setup));

            OnNodeAdded?.Invoke(setup);

            return setup;
        }

        public Task RemoveNode(Guid nodeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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

        public Task SetNodeDescription(Guid nodeGuid, string name, string description, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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

        public event Action<NodeSetup> OnNodeAdded = delegate { };
        public event Action<Guid> OnNodeRemoved = delegate { };
        public event Action<WireSetup> OnWireAdded = delegate { };
        public event Action<Guid> OnWireRemoved = delegate { };
    }
}
