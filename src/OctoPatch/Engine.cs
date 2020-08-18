using OctoPatch.Communication;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OctoPatch
{
    /// <summary>
    /// Implementation of the OctoPatch Engine
    /// </summary>
    public sealed class Engine : IEngine
    {
        private readonly SemaphoreSlim _localLock;

        /// <summary>
        /// Reference to the repository
        /// </summary>
        private readonly IRepository _repository;

        /// <summary>
        /// List of all available node descriptions
        /// </summary>
        private readonly NodeDescription[] _descriptions;

        /// <summary>
        /// Internal list of nodes
        /// </summary>
        private readonly ObservableCollection<INode> _nodes;

        /// <summary>
        /// Internal list of wires
        /// </summary>
        private readonly ObservableCollection<IWire> _wires;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public ReadOnlyObservableCollection<INode> Nodes { get; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public ReadOnlyObservableCollection<IWire> Wires { get; }

        public Engine(IRepository repository)
        {
            _localLock = new SemaphoreSlim(1);

            _repository = repository;
            _descriptions = repository.GetNodeDescriptions().ToArray();

            _nodes = new ObservableCollection<INode>();
            _wires = new ObservableCollection<IWire>();
            Nodes = new ReadOnlyObservableCollection<INode>(_nodes);
            Wires = new ReadOnlyObservableCollection<IWire>(_wires);
        }

        #region node management

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<INode> AddNode(Type type, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                var description = FindDescriptionByType(type);
                return await InternalAddNode(description, Guid.NewGuid(), cancellationToken);
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<INode> AddNode(Type type, Guid nodeId, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                var description = FindDescriptionByType(type);
                return await InternalAddNode(description, nodeId, cancellationToken);
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<INode> AddNode(Type type, string configuration, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                var description = FindDescriptionByType(type);
                var node = await InternalAddNode(description, Guid.NewGuid(), cancellationToken);
                await InternalInitializeNode(node, configuration, cancellationToken);
                return node;
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<INode> AddNode(Type type, Guid nodeId, string configuration, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                var description = FindDescriptionByType(type);
                var node = await InternalAddNode(description, nodeId, cancellationToken);
                await InternalInitializeNode(node, configuration, cancellationToken);
                return node;
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<INode> AddNode(Guid descriptionId, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                var description = FindDescriptionByNodeDescriptionId(descriptionId);
                return await InternalAddNode(description, Guid.NewGuid(), cancellationToken);
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<INode> AddNode(Guid descriptionId, Guid nodeId, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                var description = FindDescriptionByNodeDescriptionId(descriptionId);
                return await InternalAddNode(description, nodeId, cancellationToken);
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<INode> AddNode(Guid descriptionId, string configuration, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                var description = FindDescriptionByNodeDescriptionId(descriptionId);
                var node = await InternalAddNode(description, Guid.NewGuid(), cancellationToken);
                await InternalInitializeNode(node, configuration, cancellationToken);
                return node;
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<INode> AddNode(Guid descriptionId, Guid nodeId, string configuration, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                var description = FindDescriptionByNodeDescriptionId(descriptionId);
                var node = await InternalAddNode(description, nodeId, cancellationToken);
                await InternalInitializeNode(node, configuration, cancellationToken);
                return node;
            }
            finally
            {
                _localLock.Release();
            }
        }


        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<INode> AddNode<T>(CancellationToken cancellationToken) where T : INode
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                var description = FindDescriptionByType(typeof(T));
                return await InternalAddNode(description, Guid.NewGuid(), cancellationToken);
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<INode> AddNode<T>(Guid nodeId, CancellationToken cancellationToken) where T : INode
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                var description = FindDescriptionByType(typeof(T));
                return await InternalAddNode(description, nodeId, cancellationToken);
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<INode> AddNode<T>(string configuration, CancellationToken cancellationToken) where T : INode
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                var description = FindDescriptionByType(typeof(T));
                var node = await InternalAddNode(description, Guid.NewGuid(), cancellationToken);
                await InternalInitializeNode(node, configuration, cancellationToken);
                return node;
            }
            finally
            {
                _localLock.Release();
            }
        }


        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<INode> AddNode<T>(Guid nodeId, string configuration, CancellationToken cancellationToken) where T : INode
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                var description = FindDescriptionByType(typeof(T));
                var node = await InternalAddNode(description, nodeId, cancellationToken);
                await InternalInitializeNode(node, configuration, cancellationToken);
                return node;
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// Creates an instance of the given node type and adds it to the collection
        /// </summary>
        /// <param name="description">reference to the related description</param>
        /// <param name="nodeId">node id</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>node instance</returns>
        private async Task<INode> InternalAddNode(NodeDescription description, Guid nodeId, CancellationToken cancellationToken)
        {
            // Double check for node id collisions
            if (_nodes.ToArray().Any(n => n.NodeId == nodeId))
            {
                throw new ArgumentException("node with this id already exists", nameof(nodeId));
            }

            var node = await _repository.CreateNode(description.Guid, nodeId, cancellationToken);
            _nodes.Add(node);

            return node;
        }

        private Task InternalInitializeNode(INode node, string configuration, CancellationToken cancellationToken)
        {
            return node.Initialize(configuration, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task RemoveNode(INode node, CancellationToken cancellationToken)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            await _localLock.WaitAsync(cancellationToken);
            try
            {
                await InternalRemoveNode(node, cancellationToken);
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task RemoveNode(Guid nodeId, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                var node = _nodes.ToArray().FirstOrDefault(n => n.NodeId == nodeId);
                if (node == null)
                {
                    throw new ArgumentException("node is not part of the engine", nameof(nodeId));
                }

                await InternalRemoveNode(node, cancellationToken);
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// Removes the given node from the system
        /// - Stops and disposes it
        /// - Removes all wires
        /// - removes it
        /// </summary>
        /// <param name="node"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task InternalRemoveNode(INode node, CancellationToken cancellationToken)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (_nodes.ToArray().Contains(node))
            {
                throw new ArgumentException("node is not part of the engine", nameof(node));
            }

            // Shut down and dispose it
            await node.Deinitialize(cancellationToken);

            // Remove all wires
            foreach (var wire in _wires.ToArray()
                .Where(w => w.Instance.InputNode == node.NodeId || w.Instance.OutputNode == node.NodeId))
            {
                await InternalRemoveWire(wire, cancellationToken);
            }

            _nodes.Remove(node);
        }

        /// <summary>
        /// Finds the right <see cref="NodeDescription"/> for the given type
        /// </summary>
        /// <param name="type">requested type</param>
        /// <returns>description</returns>
        private NodeDescription FindDescriptionByType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var description = _descriptions.FirstOrDefault(d => d.TypeName == type.FullName);
            if (description == null)
            {
                throw new ArgumentException("unknown node type");
            }

            return description;
        }

        /// <summary>
        /// Finds the right <see cref="NodeDescription"/> for the given type
        /// </summary>
        /// <param name="nodeDescriptionId">id of the requested node</param>
        /// <returns>description</returns>
        private NodeDescription FindDescriptionByNodeDescriptionId(Guid nodeDescriptionId)
        {
            var description = _descriptions.FirstOrDefault(d => d.Guid == nodeDescriptionId);
            if (description == null)
            {
                throw new ArgumentException("unknown node id");
            }

            return description;
        }

        #endregion

        #region wire management

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<IWire> AddWire(Guid outputNode, Guid outputConnector, Guid inputNode, Guid inputConnector, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                return await InternalAddWire(outputNode, outputConnector, inputNode, inputConnector, cancellationToken);
            }
            finally
            {
                _localLock.Release();
            }
        }

        private Task<IWire> InternalAddWire(Guid outputNode, Guid outputConnector, Guid inputNode, Guid inputConnector,
            CancellationToken cancellationToken)
        {
            // Identify nodes and outputs
            // Create the wire
            // Attach it
            // Add it

            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task RemoveWire(Guid outputNode, Guid outputConnector, Guid inputNode, Guid inputConnector, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                // Lookup wire
                var wire = _wires.ToArray().FirstOrDefault(w =>
                    w.Instance.OutputNode == outputNode && w.Instance.OutputConnector == outputConnector &&
                    w.Instance.InputNode == inputNode && w.Instance.InputConnector == inputConnector);

                if (wire == null)
                {
                    throw new ArgumentException("there is no wire with this parameter");
                }

                await InternalRemoveWire(wire, cancellationToken);
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task RemoveWire(IWire wire, CancellationToken cancellationToken)
        {
            if (wire == null)
            {
                throw new ArgumentNullException(nameof(wire));
            }

            await _localLock.WaitAsync(cancellationToken);
            try
            {
                await InternalRemoveWire(wire, cancellationToken);
            }
            finally
            {
                _localLock.Release();
            }
        }

        private Task InternalRemoveWire(IWire wire, CancellationToken cancellationToken)
        {
            if (wire == null)
            {
                throw new ArgumentNullException(nameof(wire));
            }

            _wires.Remove(wire);
            return Task.CompletedTask;
        }

        #endregion
    }
}
