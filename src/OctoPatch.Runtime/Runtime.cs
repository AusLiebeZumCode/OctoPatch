using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Communication;

namespace OctoPatch.Runtime
{
    public class Runtime : IEngineService
    {
        /// <summary>
        /// Engine instance
        /// </summary>
        private readonly IEngine _engine;

        /// <summary>
        /// Reference to the repository
        /// </summary>
        private readonly IRepository _repository;

        /// <summary>
        /// List of all available node descriptions
        /// </summary>
        private readonly NodeDescription[] _descriptions;

        private Dictionary<Guid, (INode node, NodeInstance instance)> _instanceMapping;

        private Dictionary<IWire, WireInstance> _wireMapping;

        public Runtime(IRepository repository)
        {
            _engine = new Engine();

            _repository = repository;
            _descriptions = repository.GetNodeDescriptions().ToArray();

            _instanceMapping = new Dictionary<Guid, (INode node, NodeInstance instance)>();
            _wireMapping = new Dictionary<IWire, WireInstance>();
        }

        public async Task<NodeInstance> AddNode(Guid nodeDescriptionGuid, CancellationToken cancellationToken)
        {
            var description = _descriptions.First(d => d.Guid == nodeDescriptionGuid);
            var node = await _repository.CreateNode(description.Guid, Guid.NewGuid(), cancellationToken);
            await _engine.AddNode(node, cancellationToken);

            var instance = new NodeInstance
            {
                Guid = node.NodeId,
                NodeDescription = nodeDescriptionGuid,
                Name = description.Name,
                Description = description.Description
            };

            _instanceMapping.Add(node.NodeId, (node, instance));

            return instance;
        }

        public Task RemoveNode(Guid nodeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<WireInstance> AddWire(Guid outputNodeId, Guid outputConnectorId, Guid inputNodeId,
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

        public Task<IEnumerable<MessageDescription>> GetMessageDescriptions(CancellationToken cancellationToken)
        {
            return Task.FromResult(_repository.GetMessageDescriptions());
        }

        public Task<IEnumerable<NodeInstance>> GetNodes(CancellationToken cancellationToken)
        {
            return Task.FromResult(_instanceMapping.Values.Select(i => i.instance).AsEnumerable());
        }

        public Task<IEnumerable<WireInstance>> GetWires(CancellationToken cancellationToken)
        {
            return Task.FromResult(_wireMapping.Values.AsEnumerable());
        }

        public Task<Grid> GetEngineConfiguration(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEngineConfiguration(Grid grid, CancellationToken cancellationToken)
        {
            //var description = _descriptions.First(d => d.Guid == nodeInstance.NodeDescription);
            //var node = await _repository.CreateNode(description.Guid, nodeInstance.Guid, cancellationToken);
            //await _engine.AddNode(node, cancellationToken);

            // _instanceMapping.Add(node.NodeId, (node, nodeInstance));

            throw new NotImplementedException();
        }

        public Task<string> GetNodeEnvironment(Guid nodeGuid, CancellationToken cancellationToken)
        {
            _instanceMapping.TryGetValue(nodeGuid, out var node);
            throw new NotImplementedException();
        }

        public Task<string> GetNodeConfiguration(Guid nodeGuid, CancellationToken cancellationToken)
        {
            _instanceMapping.TryGetValue(nodeGuid, out var node);
            return Task.FromResult(node.instance.Configuration);
        }

        public async Task SetNodeConfiguration(Guid nodeGuid, string configuration, CancellationToken cancellationToken)
        {
            _instanceMapping.TryGetValue(nodeGuid, out var node);
            await node.node.Initialize(configuration, cancellationToken);
            node.instance.Configuration = configuration;
        }
    }
}
